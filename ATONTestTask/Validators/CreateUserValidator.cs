using ATONTestTask.Extentions;
using ATONTestTask.ViewModels.Request;
using FluentValidation;

namespace ATONTestTask.Validators
{
    public class CreateUserValidator : AbstractValidator<CreateUsereDto>
    {
        public CreateUserValidator()
        {
            RuleFor(c => c.Login).NotEmpty().Login().WithMessage("Login must contain only latin letters and digits");
            RuleFor(c => c.Password).NotEmpty().Password().WithMessage("Password must contain only latin letters and digits");
            RuleFor(c => c.Name).NotEmpty().Name().WithMessage("Name must contain only letters");
            RuleFor(c => c.Admin).NotNull();
            RuleFor(c => c.Gender).NotNull().IsInEnum();
        }
    }
}
