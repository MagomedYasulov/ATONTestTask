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
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> Update(Guid id, UpdateUserDto model)
        {
            var userDto = await _usersService.Update(id ,model);
            return Ok(userDto);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPatch("{id}/password")]
        public async Task<ActionResult<UserDto>> UpdatePassword(Guid id, [FromBody]string password)
        {
            var userDto = await _usersService.UpdatePassword(id, password);
            return Ok(userDto);
        }


        [Authorize(Roles = "Admin, User")]
        [HttpPatch("{id}/login")]
        public async Task<ActionResult<UserDto>> UpdateLogin(Guid id, [FromBody] string login)
        {
            var userDto = await _usersService.UpdatePassword(id, login);
            return Ok(userDto);
        }

        [HttpPatch("{id}/recover")]
        public async Task<ActionResult<UserDto>> UpdatePassword(Guid id)
        {
            var userDto = await _usersService.Recover(id);
            return Ok(userDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, bool isSoft)
        {
            await _usersService.Delete(id, isSoft);
            return Ok();
        }
    }
}
