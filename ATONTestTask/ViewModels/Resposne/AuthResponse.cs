namespace ATONTestTask.ViewModels.Resposne
{
    public class AuthResponse
    {
        public UserDto? User { get; set; }
        public string AccessToken { get; set; } = string.Empty;
    }
}
