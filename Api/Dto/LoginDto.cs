using System.ComponentModel.DataAnnotations;
using Api.Models;

namespace Api.Dto{
    public class LoginDto{
        [Required(ErrorMessage = "Username is required!")]
        public string? UserName { get; set;}
        [Required(ErrorMessage = "Password is required!")]
        public string? Password { get; set;}


    }
}