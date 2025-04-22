using Microsoft.AspNetCore.Mvc;

namespace ATONTestTask.ViewModels.Request
{
    public class UsersFilter
    {
        [FromQuery]
        public bool? RevokedOn { get; set; }
        [FromQuery]
        public int? MinAge { get; set; }
        [FromQuery]
        public int? MaxAge { get; set; }
    }
}
