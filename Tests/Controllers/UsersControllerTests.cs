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
    public class UsersControllerTests
    {
        private readonly IMapper _mapper;
        private readonly ApplicationContext _dbContext;
        private readonly IPasswordHasher _passwordHasher = new PasswordHasher();

        public UsersControllerTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "UsersContrllerTestsTestDB");
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));

            _dbContext = new ApplicationContext(optionsBuilder.Options);
            _mapper = new Mapper(mapperConfig);

            Common.SeedData(_dbContext, _passwordHasher);
        }

        #region Create Test

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

            var user = await _dbContext.Users.FirstAsync(u => u.Login == createDto.Login);
            Assert.Equal(createDto.Name, user.Name);
            Assert.Equal(createDto.Login, user.Login);
            Assert.Equal(createDto.BirthDate, user.BirthDate);
            Assert.Equal(createDto.Admin, user.Admin);
            Assert.Equal(createDto.Gender, user.Gender);
            Assert.Equal("admin", user.CreatedBy);
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
        public async Task Get_All_Users()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("admin", "Admin"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var filter = new UsersFilter();


            // Act
            var result = await usersController.Get(filter);

            // Assert
            var user = await _dbContext.Users.FirstAsync(u => u.Login == "user1");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var usersDto = Assert.IsType<UserExtendedDto[]>(okResult.Value);

            var usersCount = await _dbContext.Users.CountAsync();

            Assert.Equal(usersCount, usersDto.Length);
            Assert.All(usersDto, userDto => Assert.NotNull(usersDto));
            for(int i=1; i< usersDto.Length;i++)
                Assert.True(usersDto[i].CreatedOn >= usersDto[i-1].CreatedOn);
        }

        [Fact]
        public async Task Get_Revoked_Users()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("admin", "Admin"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var filter = new UsersFilter()
            {
                RevokedOn = true
            };

            // Act
            var result = await usersController.Get(filter);

            // Assert
            var user = await _dbContext.Users.FirstAsync(u => u.Login == "user1");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var usersDto = Assert.IsType<UserExtendedDto[]>(okResult.Value);

            var revokedUsersCount = await _dbContext.Users.CountAsync(u => u.RevokedOn != null);

            Assert.Equal(revokedUsersCount, usersDto.Length);
            Assert.All(usersDto, userDto => Assert.NotNull(usersDto));
            for (int i = 1; i < usersDto.Length; i++)
                Assert.True(usersDto[i].CreatedOn >= usersDto[i - 1].CreatedOn);
        }

        [Fact]
        public async Task Get_Not_Revoked_Users()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("admin", "Admin"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var filter = new UsersFilter()
            {
                RevokedOn = false
            };

            // Act
            var result = await usersController.Get(filter);

            // Assert
            var user = await _dbContext.Users.FirstAsync(u => u.Login == "user1");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var usersDto = Assert.IsType<UserExtendedDto[]>(okResult.Value);

            var notRevokedUsersCount = await _dbContext.Users.CountAsync(u => u.RevokedOn == null);

            Assert.Equal(notRevokedUsersCount, usersDto.Length);
            Assert.All(usersDto, userDto => Assert.NotNull(usersDto));
            for (int i = 1; i < usersDto.Length; i++)
                Assert.True(usersDto[i].CreatedOn >= usersDto[i - 1].CreatedOn);
        }

        #endregion

        #region Get User By Login Tests

        [Fact]
        public async Task Get_User_By_Login()
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
        public async Task Get_Not_Exist_User_By_Login()
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

        #region Update Tests

        [Fact]
        public async Task Update_User_By_Admin()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("admin", "Admin"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var updateDto = new UpdateUserDto()
            {
                Name = "newusername",
                BirthDate = new DateTime(1970, 09, 20, 0, 0, 0, DateTimeKind.Utc),
                Gender = Gender.Male
            };

            // Act
            var result = await usersController.Update("revoked_user", updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var userDto = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(updateDto.Name, userDto.Name);
            Assert.Equal(updateDto.BirthDate, userDto.BirthDate);
            Assert.Equal(updateDto.Gender, userDto.Gender);

            var user = await _dbContext.Users.FirstAsync(u => u.Login == "revoked_user");
            Assert.Equal(updateDto.Name, user.Name);
            Assert.Equal(updateDto.BirthDate, user.BirthDate);
            Assert.Equal(updateDto.Gender, user.Gender);
            Assert.Equal("admin", user.ModifiedBy);
            Assert.NotNull(user.ModifiedOn);
        }

        [Fact]
        public async Task Update_User_Himself()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("user3", "User"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var updateDto = new UpdateUserDto()
            {
                Name = "newusername",
                BirthDate = new DateTime(1970, 09, 20, 0, 0, 0, DateTimeKind.Utc),
                Gender = Gender.Male
            };

            // Act
            var result = await usersController.Update("user3", updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var userDto = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(updateDto.Name, userDto.Name);
            Assert.Equal(updateDto.BirthDate, userDto.BirthDate);
            Assert.Equal(updateDto.Gender, userDto.Gender);

            var user = await _dbContext.Users.FirstAsync(u => u.Login == "user3");
            Assert.Equal(updateDto.Name, user.Name);
            Assert.Equal(updateDto.BirthDate, user.BirthDate);
            Assert.Equal(updateDto.Gender, user.Gender);
            Assert.Equal("user3", user.ModifiedBy);
            Assert.NotNull(user.ModifiedOn);
        }

        [Fact]
        public async Task Update_Revoked_User_Himself()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("revoked_user", "User"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var updateDto = new UpdateUserDto()
            {
                Name = "newusername",
                BirthDate = new DateTime(1970, 09, 20, 0, 0, 0, DateTimeKind.Utc),
                Gender = Gender.Male
            };

            // Act
            Func<Task> act = async () => await usersController.Update("revoked_user", updateDto);

            // Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(act);
            Assert.Equal("Access denied", exception.Title);
            Assert.Equal($"Access to update user with login revoked_user denied", exception.Message);
            Assert.Equal(StatusCodes.Status403Forbidden, exception.StatusCode);
        }

        [Fact]
        public async Task Update_User_By_User()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("user4", "User"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var updateDto = new UpdateUserDto()
            {
                Name = "newusername",
                BirthDate = new DateTime(1970, 09, 20, 0, 0, 0, DateTimeKind.Utc),
                Gender = Gender.Male
            };

            // Act
            Func<Task> act = async () => await usersController.Update("user5", updateDto);

            // Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(act);
            Assert.Equal("Access denied", exception.Title);
            Assert.Equal($"Access to update user with login user5 denied", exception.Message);
            Assert.Equal(StatusCodes.Status403Forbidden, exception.StatusCode);
        }

        [Fact]
        public async Task Update_Not_Exist_User()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("admin", "Admin"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var login = "not_exist_user_login";
            var updateDto = new UpdateUserDto()
            {
                Name = "newusername",
                BirthDate = new DateTime(1970, 09, 20, 0, 0, 0, DateTimeKind.Utc),
                Gender = Gender.Male
            };

            // Act
            Func<Task> act = async () => await usersController.Update(login, updateDto);

            // Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(act);
            Assert.Equal("User Not Found", exception.Title);
            Assert.Equal($"User with login {login} not found", exception.Message);
            Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        }

        #endregion

        #region Update Login Tests

        [Fact]
        public async Task Update_User_Login_By_Admin()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("admin", "Admin"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var updateDto = new UpdateLoginDto()
            {
                Login = "newuserlogin"
            };

            // Act
            var result = await usersController.UpdateLogin("user7", updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var userDto = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(updateDto.Login, userDto.Login);

            var user = await _dbContext.Users.FirstAsync(u => u.Login == updateDto.Login);
            Assert.Equal(updateDto.Login, user.Login);
            Assert.Equal("admin", user.ModifiedBy);
            Assert.NotNull(user.ModifiedOn);
        }

        [Fact]
        public async Task Update_User_Login_By_Himself()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("user7", "User"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var updateDto = new UpdateLoginDto()
            {
                Login = "newuserlogin"
            };

            // Act
            var result = await usersController.UpdateLogin("user7", updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var userDto = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(updateDto.Login, userDto.Login);

            var user = await _dbContext.Users.FirstAsync(u => u.Login == updateDto.Login);
            Assert.Equal(updateDto.Login, user.Login);
            Assert.Equal("user7", user.ModifiedBy);
            Assert.NotNull(user.ModifiedOn);
        }

        [Fact]
        public async Task Update_User_Login_By_Another_User()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("user7", "User"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var updateDto = new UpdateLoginDto()
            {
                Login = "newuserlogin"
            };

            // Act
            Func<Task> act = async () => await usersController.UpdateLogin("user8", updateDto);

            // Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(act);
            Assert.Equal("Access denied", exception.Title);
            Assert.Equal($"Access to update user with login user8 denied", exception.Message);
            Assert.Equal(StatusCodes.Status403Forbidden, exception.StatusCode);
        }

        [Fact]
        public async Task Update_Login_By_Revoked_User()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("revoked_user", "User"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var updateDto = new UpdateLoginDto()
            {
                Login = "newuserlogin"
            };

            // Act
            Func<Task> act = async () => await usersController.UpdateLogin("revoked_user", updateDto);

            // Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(act);
            Assert.Equal("Access denied", exception.Title);
            Assert.Equal($"Access to update user with login revoked_user denied", exception.Message);
            Assert.Equal(StatusCodes.Status403Forbidden, exception.StatusCode);
        }


        [Fact]
        public async Task Update_With_Not_Free_Login()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("admin", "Admin"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var updateDto = new UpdateLoginDto()
            {
                Login = "user4"
            };

            // Act
            Func<Task> act = async () => await usersController.UpdateLogin("user3", updateDto);

            // Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(act);
            Assert.Equal("Users conflict", exception.Title);
            Assert.Equal($"User with login {updateDto.Login} already exist", exception.Message);
            Assert.Equal(StatusCodes.Status409Conflict, exception.StatusCode);
        }


        [Fact]
        public async Task Update_Not_Exist_Login()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("admin", "Admin"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var login = "not_exist_user_login";
            var updateDto = new UpdateLoginDto()
            {
                Login = "newuserlogin"
            };

            // Act
            Func<Task> act = async () => await usersController.UpdateLogin(login, updateDto);

            // Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(act);
            Assert.Equal("User Not Found", exception.Title);
            Assert.Equal($"User with login {login} not found", exception.Message);
            Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        }


        #endregion

        #region Update Password Tests

        [Fact]
        public async Task Update_User_Password_By_Admin()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("admin", "Admin"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var updateDto = new UpdatePasswordDto()
            {
                Password = "newpassword"
            };

            // Act
            var result = await usersController.UpdatePassword("user7", updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var userDto = Assert.IsType<UserDto>(okResult.Value);

            var user = await _dbContext.Users.FirstAsync(u => u.Login == "user7");
            Assert.True(_passwordHasher.VerifyHashedPassword(user.PasswordHash, updateDto.Password));
            Assert.Equal("admin", user.ModifiedBy);
            Assert.NotNull(user.ModifiedOn);
        }

        [Fact]
        public async Task Update_User_Password_By_Himself()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("user7", "User"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var updateDto = new UpdatePasswordDto()
            {
                Password = "newpassword"
            };

            // Act
            var result = await usersController.UpdatePassword("user7", updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var userDto = Assert.IsType<UserDto>(okResult.Value);

            var user = await _dbContext.Users.FirstAsync(u => u.Login == "user7");
            Assert.True(_passwordHasher.VerifyHashedPassword(user.PasswordHash, updateDto.Password));
            Assert.Equal("user7", user.ModifiedBy);
            Assert.NotNull(user.ModifiedOn);
        }

        [Fact]
        public async Task Update_User_Password_By_Another_User()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("user7", "User"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var updateDto = new UpdatePasswordDto()
            {
                Password = "newpassword"
            };

            // Act
            Func<Task> act = async () => await usersController.UpdatePassword("user8", updateDto);

            // Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(act);
            Assert.Equal("Access denied", exception.Title);
            Assert.Equal($"Access to update user with login user8 denied", exception.Message);
            Assert.Equal(StatusCodes.Status403Forbidden, exception.StatusCode);
        }

        [Fact]
        public async Task Update_Password_By_Revoked_User()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("revoked_user", "User"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var updateDto = new UpdatePasswordDto()
            {
                Password = "newpassword"
            };

            // Act
            Func<Task> act = async () => await usersController.UpdatePassword("revoked_user", updateDto);

            // Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(act);
            Assert.Equal("Access denied", exception.Title);
            Assert.Equal($"Access to update user with login revoked_user denied", exception.Message);
            Assert.Equal(StatusCodes.Status403Forbidden, exception.StatusCode);
        }

        [Fact]
        public async Task Update_Password_Of_Not_Exist_User()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("admin", "Admin"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var login = "not_exist_user_login";
            var updateDto = new UpdatePasswordDto()
            {
                Password = "newuserlogin"
            };

            // Act
            Func<Task> act = async () => await usersController.UpdatePassword(login, updateDto);

            // Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(act);
            Assert.Equal("User Not Found", exception.Title);
            Assert.Equal($"User with login {login} not found", exception.Message);
            Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        }

        #endregion

        #region Recover Tests

        [Fact]
        public async Task Recover_Not_Exist_User()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("admin", "Admin"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var login = "not_exist_user_login";

            // Act
            Func<Task> act = async () => await usersController.RecoverUser(login);

            // Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(act);
            Assert.Equal("User Not Found", exception.Title);
            Assert.Equal($"User with login {login} not found", exception.Message);
            Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        }

        [Fact]
        public async Task Recover_User()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("admin", "Admin"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var login = "revoked_user";

            // Act
            var result = await usersController.RecoverUser(login);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var userDto = Assert.IsType<UserExtendedDto>(okResult.Value);

            var user = await _dbContext.Users.FirstAsync(u => u.Login == login);
            Assert.Null(user.RevokedOn);
            Assert.Null(user.RevokedBy);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_User()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("admin", "Admin"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);

            // Act
            var result = await usersController.Delete("user1", false);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.False(await _dbContext.Users.AnyAsync(u => u.Login == "user1"));
        }

        [Fact]
        public async Task Soft_Delete_User()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("admin", "Admin"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);

            // Act
            var result = await usersController.Delete("user1", true);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            var user = await _dbContext.Users.FirstAsync(u => u.Login == "user1");
            Assert.NotNull(user.RevokedOn);
            Assert.Equal("admin", user.RevokedBy);
        }

        [Fact]
        public async Task Delete_Not_Exist_User()
        {
            // Arrange
            var usersService = new UsersService(_passwordHasher, GetHttpContextAccessor("admin", "Admin"), _mapper, _dbContext);
            var usersController = new UsersController(usersService);
            var login = "not_exist_user_login";

            // Act
            Func<Task> act = async () => await usersController.Delete(login, false);

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
