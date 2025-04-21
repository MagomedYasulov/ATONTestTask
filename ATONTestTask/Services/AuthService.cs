using ATONTestTask.Abstractions;
using ATONTestTask.Data;
using ATONTestTask.Exceptions;
using ATONTestTask.Extentions;
using ATONTestTask.ViewModels.Request;
using ATONTestTask.ViewModels.Resposne;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ATONTestTask.Services
{
    public class AuthService : IAuthService
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ApplicationContext _dbContext;

        public AuthService(
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IMapper mapper,
            ApplicationContext dbContext)
        {
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<AuthResponse> Login(LoginDto model)
        {
            var user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Login == model.Login);
            if (user == null)
                throw new ServiceException("Invalid login", $"User with login {model.Login} not found", StatusCodes.Status401Unauthorized);

            if (!_passwordHasher.VerifyHashedPassword(user.PasswordHash, model.Password))
                throw new ServiceException("Invalid password", $"Password {model.Password} is invalid", StatusCodes.Status401Unauthorized);

            var userDto = _mapper.Map<UserDto>(user);
            var identity = userDto.GetIdentity();
            var accessToken = _tokenService.GenerateAccessToken(identity);

            return new AuthResponse() { User = userDto, AccessToken = accessToken };
        }
    }
}
