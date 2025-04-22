using ATONTestTask.Enums;

namespace ATONTestTask.ViewModels.Request
{
    public class UpdateUserDto
    {
        public string Name { get; set; } = string.Empty;
        public Gender? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
