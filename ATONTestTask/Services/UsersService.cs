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

        public async Task Delete(Guid id, bool isSoft)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                throw new ServiceException("User Not Found", $"User with id {id} not found", StatusCodes.Status404NotFound);

            if(isSoft)
            {
                user.RevokedBy = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                user.RevokedOn = DateTime.UtcNow;
            }    
            else
                _dbContext.Remove(user);

            await _dbContext.SaveChangesAsync();
        }

        public Task<UserExtendedDto> Recover(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> Update(Guid id, UpdateUserDto model)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> UpdateLogin(Guid id, string login)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> UpdatePassword(Guid id, string password)
        {
            throw new NotImplementedException();
        }
    }
}
