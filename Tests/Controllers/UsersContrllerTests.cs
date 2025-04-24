using ATONTestTask.Abstractions;
using ATONTestTask.Controllers;
using ATONTestTask.Data;
using ATONTestTask.Enums;
using ATONTestTask.Exceptions;
using ATONTestTask.Models;
using ATONTestTask.Services;
using ATONTestTask.ViewModels.Request;
using ATONTestTask.ViewModels.Resposne;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

namespace Tests.Controllers
{
    public class UsersContrllerTests
    {
        private readonly IMapper _mapper;
        private readonly ApplicationContext _dbContext;
        private readonly IPasswordHasher _passwordHasher = new PasswordHasher();

        public UsersContrllerTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "UsersContrllerTestsTestDB");
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));

            _dbContext = new ApplicationContext(optionsBuilder.Options);
            _mapper = new Mapper(mapperConfig);

            Common.SeedData(_dbContext, _passwordHasher);
        }

        #region Create Users Test

        [Fact]
        public async Task Create_User()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("admin", "Admin"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var createDto = new CreateUsereDto()
            {
                Login = "newuserlogin",
                Name = "newusername",
                Password = "1234",
                Admin = true,
                BirthDate = new DateTime(1970, 09, 20, 0, 0, 0, DateTimeKind.Utc),
                Gender = Gender.Male
            };

            // Act
            var result = await usersController.Create(createDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var userDto = Assert.IsType<UserExtendedDto>(okResult.Value);
            Assert.Equal(createDto.Name, userDto.Name);
            Assert.Equal(createDto.Login, userDto.Login);
            Assert.Equal(createDto.BirthDate, userDto.BirthDate);
            Assert.Equal(createDto.Admin, userDto.Admin);
            Assert.Equal(createDto.Gender, userDto.Gender);
            Assert.Equal("admin", userDto.CreatedBy);
        }

        [Fact]
        public async Task Create_User_With_Not_Free_Login()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("admin", "Admin"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var createDto = new CreateUsereDto()
            {
                Login = "user2",
                Name = "newusername",
                Password = "1234",
                Admin = true,
                BirthDate = new DateTime(1970, 09, 20, 0, 0, 0, DateTimeKind.Utc),
                Gender = Gender.Male
            };

            // Act
            Func<Task> act = async () => await usersController.Create(createDto);

            // Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(act);

            Assert.Equal("Users conflict", exception.Title);
            Assert.Equal($"User with login user2 already exist",  exception.Message);
            Assert.Equal(StatusCodes.Status409Conflict, exception.StatusCode);
        }

        #endregion

        #region Get Users Tests

        [Fact]
        public async Task Get_User()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("admin", "Admin"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);

            // Act
            var result = await usersController.Get("user1");

            // Assert
            var user = await _dbContext.Users.FirstAsync(u => u.Login == "user1");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var userDto = Assert.IsType<UserExtendedDto>(okResult.Value);
            Assert.Equal(user.Id, userDto.Id);
            Assert.Equal(user.Name, userDto.Name);
            Assert.Equal(user.Login, userDto.Login);
            Assert.Equal(user.Admin, userDto.Admin);
            Assert.Equal(user.Gender, userDto.Gender);
        }

        [Fact]
        public async Task Get_Not_Exist_User()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("admin", "Admin"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var login = "not_exist_user_login";

            // Act
            Func<Task> act = async () => await usersController.Get(login);

            // Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(act);

            Assert.Equal("User Not Found", exception.Title);
            Assert.Equal($"User with login {login} not found", exception.Message);
            Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        }

        #endregion

        private IHttpContextAccessor GetHttpContextAccessor(string login, string role)
        {
            var claims = new Claim[] { new(ClaimTypes.Role, role), new(ClaimTypes.NameIdentifier, login) };
            var claimsIdentity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(hc => hc.User).Returns(claimsPrincipal);

            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(_ => _.HttpContext).Returns(httpContext.Object);
            return contextAccessor.Object;
        }
    }
}
