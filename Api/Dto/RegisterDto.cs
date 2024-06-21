using System.ComponentModel.DataAnnotations;

namespace Api.Dto{
    public class RegisterDto{
        [Required(ErrorMessage ="Username is required!")]
        public string? UserName { get; set; }
        [Required(ErrorMessage ="Password is required!")]
        public string? Password { get; set; }
    }
}