using ATONTestTask.Enums;

namespace ATONTestTask.ViewModels.Resposne
{
    public class UserExtendedDto
    {
        public Guid Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Gender Gender { get; set; } = Gender.Unknown;
        public DateTime? BirthDate { get; set; }
        public bool Admin { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? RevokedOn { get; set; }
        public string? RevokedBy { get; set; }
        public bool IsActive => RevokedOn == null;
    }
}
