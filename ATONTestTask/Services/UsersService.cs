using ATONTestTask.Abstractions;
using ATONTestTask.Data;
using ATONTestTask.Data.Entites;
using ATONTestTask.Exceptions;
using ATONTestTask.ViewModels.Request;
using ATONTestTask.ViewModels.Resposne;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace ATONTestTask.Services
{
    public class UsersService : IUsersService
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ApplicationContext _dbContext;

        public UsersService(
            IPasswordHasher passwordHasher,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            ApplicationContext dbContext)
        {
            _passwordHasher = passwordHasher;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<UserExtendedDto> Create(CreateUsereDto model)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Login == model.Login))
                throw new ServiceException("Users conflict", $"User with login {model.Login} already exist", StatusCodes.Status409Conflict);

            var creator = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = _mapper.Map<User>(model);
            user.CreatedBy = creator!;
            user.PasswordHash = _passwordHasher.HashPassword(model.Password);

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<UserExtendedDto>(user);
        }

        public async Task<UserExtendedDto> Get(string login)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (user == null)
                throw new ServiceException("User Not Found", $"User with login {login} not found", StatusCodes.Status404NotFound);

            return _mapper.Map<UserExtendedDto>(user);
        }

        public async Task<UserExtendedDto[]> Get(UsersFilter filter)
        {
            var isMinAgeNull = filter.MinAge == null;
            var isMaxAgeNull = filter.MaxAge == null;
            var isRevokedOnNull = filter.RevokedOn == null;

            Expression<Func<User, bool>> exp = u => ((isMinAgeNull || u.BirthDate <= DateTime.UtcNow - TimeSpan.FromDays(365 * filter.MinAge!.Value)) &&
                                                     (isMaxAgeNull || u.BirthDate >= DateTime.UtcNow - TimeSpan.FromDays(365 * filter.MaxAge!.Value)) &&
                                                     (isRevokedOnNull || u.RevokedOn == null != filter.RevokedOn)
                                                     );

            var users = await _dbContext.Users.AsNoTracking().Where(exp).ToArrayAsync();
            return _mapper.Map<UserExtendedDto[]>(users);
        }

        public async Task Delete(string login, bool isSoft)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (user == null)
                throw new ServiceException("User Not Found", $"User with login {login} not found", StatusCodes.Status404NotFound);

            if(isSoft)
            {
                user.RevokedBy = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                user.RevokedOn = DateTime.UtcNow;
            }    
            else
                _dbContext.Remove(user);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<UserExtendedDto> Recover(string login)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (user == null)
                throw new ServiceException("User Not Found", $"User with login {login} not found", StatusCodes.Status404NotFound);

            user.RevokedBy = null;
            user.RevokedOn = null;
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<UserExtendedDto>(user);
        }

        public async Task<UserDto> Update(string login, UpdateUserDto model)
        {
            var user = await CheckPermissions(login);

            user.BirthDate = model.BirthDate;
            user.Name = model.Name;
            user.Gender = model.Gender!.Value;
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateLogin(string oldLogin, string newLogin)
        {
            var user = await CheckPermissions(oldLogin);
            if (await _dbContext.Users.AnyAsync(u => u.Login == newLogin))
                throw new ServiceException("Users conflict", $"User with login {newLogin} already exist", StatusCodes.Status409Conflict);

            user.Login = newLogin;
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdatePassword(string login, string password)
        {
            var user = await CheckPermissions(login);
            user.PasswordHash = _passwordHasher.HashPassword(password);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        private async Task<User> CheckPermissions(string login)
        {
            var updaterRole = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role)!.Value;
            var updaterLogin = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (updaterRole != "Admin" && updaterLogin != login)
                throw new ServiceException("Access denied", $"Access to update user with login {login} denied", StatusCodes.Status403Forbidden);

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (user == null)
                throw new ServiceException("User Not Found", $"User with login {login} not found", StatusCodes.Status404NotFound);

            if (updaterRole != "Admin" && user.RevokedOn != null)
                throw new ServiceException("Access denied", $"Access to update user with login {login} denied", StatusCodes.Status403Forbidden);

            return user;
        }
    }
}
