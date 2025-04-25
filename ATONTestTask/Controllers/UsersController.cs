using ATONTestTask.Abstractions;
using ATONTestTask.ViewModels.Request;
using ATONTestTask.ViewModels.Resposne;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ATONTestTask.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }
        
        /// <summary>
        /// Создание пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<UserExtendedDto>> Create(CreateUsereDto model)
        {
            var userDto = await _usersService.Create(model);
            return Ok(userDto);
        }

        /// <summary>
        /// Получение пользователя по логину
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("{login}")]
        public async Task<ActionResult<UserExtendedDto>> Get(string login)
        {
            var userDto = await _usersService.Get(login);
            return Ok(userDto);
        }

        /// <summary>
        /// Получение пользователй
        /// </summary>
        /// <param name="filter">
        /// Фильтрация по максимальному и минимальному возрасту 
        /// и активных или нет пользователей
        /// </param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<UserExtendedDto>> Get(UsersFilter filter)
        {
            var userDto = await _usersService.Get(filter);
            return Ok(userDto);
        }

        /// <summary>
        /// Обновление пользователя
        /// </summary>
        /// <param name="login">логин обновляемого пользователя</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin, User")]
        [HttpPut("{login}")]
        public async Task<ActionResult<UserDto>> Update(string login, UpdateUserDto model)
        {
            var userDto = await _usersService.Update(login, model);
            return Ok(userDto);
        }

        /// <summary>
        /// Обновление пароя пользователя
        /// </summary>
        /// <param name="login"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin, User")]
        [HttpPatch("{login}/password")]
        public async Task<ActionResult<UserDto>> UpdatePassword(string login, UpdatePasswordDto model)
        {
            var userDto = await _usersService.UpdatePassword(login, model.Password);
            return Ok(userDto);
        }

        /// <summary>
        /// Обновление логина пользователя
        /// </summary>
        /// <param name="login"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin, User")]
        [HttpPatch("{login}/login")]
        public async Task<ActionResult<UserDto>> UpdateLogin(string login, UpdateLoginDto model)
        {
            var userDto = await _usersService.UpdateLogin(login, model.Login);
            return Ok(userDto);
        }

        /// <summary>
        /// Восстановление пользователя
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPatch("{login}/recover")]
        public async Task<ActionResult<UserExtendedDto>> RecoverUser(string login)
        {
            var userDto = await _usersService.Recover(login);
            return Ok(userDto);
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="login"></param>
        /// <param name="isSoft"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{login}")]
        public async Task<ActionResult> Delete(string login, bool isSoft)
        {
            await _usersService.Delete(login, isSoft);
            return Ok();
        }
    }
}
