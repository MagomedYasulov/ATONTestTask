using ATONTestTask.ViewModels.Request;
using FluentValidation;

namespace ATONTestTask.Validators
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(l => l.Login).NotEmpty();
            RuleFor(l => l.Password).NotEmpty();
        }
    }
}
