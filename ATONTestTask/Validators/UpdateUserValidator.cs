using ATONTestTask.Extentions;
using ATONTestTask.ViewModels.Request;
using FluentValidation;

namespace ATONTestTask.Validators
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserValidator()
        {
            RuleFor(u => u.Name).NotEmpty().Name();
            RuleFor(u => u.Gender).NotNull().IsInEnum();
        }
    }
}
