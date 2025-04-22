using ATONTestTask.Abstractions;
using ATONTestTask.ViewModels.Request;
using ATONTestTask.ViewModels.Resposne;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        [Authorize("Admin")]
        [HttpPost]
        public async Task<ActionResult<UserExtendedDto>> Create(CreateUsereDto model)
        {
            var userDto = await _usersService.Create(model);
            return Ok(userDto);
        }

        [HttpPost]
        public async Task<ActionResult<UserExtendedDto>> Create(CreateUsereDto model)
        {
            var userDto = await _usersService.Create(model);
            return Ok(userDto);
        }

    }
}
