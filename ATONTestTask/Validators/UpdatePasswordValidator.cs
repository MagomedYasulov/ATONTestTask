using ATONTestTask.Extentions;
using ATONTestTask.ViewModels.Request;
using FluentValidation;

namespace ATONTestTask.Validators
{
    public class UpdatePasswordValidator : AbstractValidator<UpdatePasswordDto>
    {
        public UpdatePasswordValidator()
        {
            RuleFor(u => u.Password).NotEmpty().Password();
        }
    }
}
