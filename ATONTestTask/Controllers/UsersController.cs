using ATONTestTask.Abstractions;
using ATONTestTask.ViewModels.Request;
using ATONTestTask.ViewModels.Resposne;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ATONTestTask.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost]
        public async Task<ActionResult<UserExtendedDto>> Create(CreateUsereDto model)
        {
            var userDto = await _usersService.Create(model);
            return Ok(userDto);
        }

        [HttpGet("{login}")]
        public async Task<ActionResult<UserExtendedDto>> Get(string login)
        {
            var userDto = await _usersService.Get(login);
            return Ok(userDto);
        }

        [HttpGet]
        public async Task<ActionResult<UserExtendedDto>> Get(UsersFilter filter)
        {
            var userDto = await _usersService.Get(filter);
            return Ok(userDto);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPut("{login}")]
        public async Task<ActionResult<UserDto>> Update(string login, UpdateUserDto model)
        {
            var userDto = await _usersService.Update(login, model);
            return Ok(userDto);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPatch("{login}/password")]
        public async Task<ActionResult<UserDto>> UpdatePassword(string login, [FromBody]string password)
        {
            var userDto = await _usersService.UpdatePassword(login, password);
            return Ok(userDto);
        }


        [Authorize(Roles = "Admin, User")]
        [HttpPatch("{login}/login")]
        public async Task<ActionResult<UserDto>> UpdateLogin(string login, [FromBody] string newLogin)
        {
            var userDto = await _usersService.UpdateLogin(login, newLogin);
            return Ok(userDto);
        }

        [HttpPatch("{login}/recover")]
        public async Task<ActionResult<UserExtendedDto>> RecoverUser(string login)
        {
            var userDto = await _usersService.Recover(login);
            return Ok(userDto);
        }

        [HttpDelete("{login}")]
        public async Task<ActionResult> Delete(string login, bool isSoft)
        {
            await _usersService.Delete(login, isSoft);
            return Ok();
        }
    }
}
