using ATONTestTask.Extentions;
using ATONTestTask.ViewModels.Request;
using FluentValidation;

namespace ATONTestTask.Validators
{
    public class UpdateLoginValidator : AbstractValidator<UpdateLoginDto>
    {
        public UpdateLoginValidator()
        {
            RuleFor(u => u.Login).NotEmpty().Login();
        }
    }
}
