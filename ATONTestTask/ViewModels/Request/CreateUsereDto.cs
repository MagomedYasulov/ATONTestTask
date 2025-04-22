using ATONTestTask.Enums;

namespace ATONTestTask.ViewModels.Request
{
    public class CreateUsereDto
    {
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Gender? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool? Admin { get; set; }
    }
}
