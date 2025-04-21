using ATONTestTask.ViewModels.Request;
using ATONTestTask.ViewModels.Resposne;

namespace ATONTestTask.Abstractions
{
    public interface IAuthService
    {
        public Task<AuthResponse> Login(LoginDto model);
    }
}
