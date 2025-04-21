using ATONTestTask.Enums;

namespace ATONTestTask.Data.Entites
{
    public class User : BaseEntity
    {
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Gender Gender { get; set; } = Gender.Unknown;
        public DateTime? BirthDate { get; set; }
        public bool Admin { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public DateTime? RevokedOn { get; set; }
        public string? RevokedBy { get; set; }
    }
}
