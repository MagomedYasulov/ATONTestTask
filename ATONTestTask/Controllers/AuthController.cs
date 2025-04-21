using ATONTestTask.Abstractions;
using ATONTestTask.ViewModels.Request;
using ATONTestTask.ViewModels.Resposne;
using Microsoft.AspNetCore.Mvc;

namespace ATONTestTask.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Default user login: admin password: admin
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Возвращет обьект с пользователем, access и refresh токенами</returns>
        [HttpPost("/api/v1/login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginDto model)
        {
            var authResposne = await _authService.Login(model);
            return Ok(authResposne);
        }
    }
}
