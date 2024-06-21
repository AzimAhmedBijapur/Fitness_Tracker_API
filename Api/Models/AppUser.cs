using Microsoft.AspNetCore.Identity;

namespace Api.Models{
    public class AppUser:IdentityUser{
        public string? Password { get; set; }
        public List<Workouts>? Workout { get; set; }
        public int Totalworkouts { get; set; }
    }
}