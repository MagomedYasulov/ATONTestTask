using ATONTestTask.Enums;

namespace ATONTestTask.ViewModels.Resposne
{
    public class UserDto
    {
        public string Login { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Gender Gender { get; set; } = Gender.Unknown;
        public DateTime? BirthDate { get; set; }
        public bool Admin { get; set; }
    }
}
