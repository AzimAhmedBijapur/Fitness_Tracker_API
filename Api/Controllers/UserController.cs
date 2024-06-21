using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Api.Dto;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Api.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;

        public UserController(ApplicationDbContext context, IAuthService authService, IUserRepository repository)
        {
            _repository = repository;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            if(!ModelState.IsValid){
                    
                    var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { Message = "Validation failed!", Errors = errors });
            }

            var response = await _repository.Login(request);

            if(response == null){
                return BadRequest("Could not login!");
            }

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            if(!ModelState.IsValid){
                    
                    var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { Message = "Validation failed!", Errors = errors });
            }

            var response = await _repository.Register(request);

            if(response == null){
                return BadRequest("Could not register!");
            }

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register/admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto request)
        {
            if(!ModelState.IsValid){
                    
                    var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { Message = "Validation failed!", Errors = errors });
            }

            var response = await _repository.RegisterAdmin(request);

            if(response == null){
                return BadRequest("Could not register!");
            }

            return Ok(response);
        }

        [HttpGet]
        [Authorize(Roles = "Admin",AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<UserResponseDto>>> GetUsers (){
            
            var users = await _repository.GetUsers();

            if(users == null){
                return NotFound("Could not fetch users!");
            }

            return Ok(users);

        } 

        [HttpGet("{userid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AppUser>> GetUserById(string userid){

            var user = await _repository.GetUserById(userid);
            if(user == null){
                return NotFound("User does not exist!");
            }
            return Ok(user);

        } 

        [HttpDelete("{userid}")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteUser(string userid)
        {
            var delete = await _repository.DeleteUser(userid);

            if(delete){
                return Ok("User Deleted!");
            }
            else{
                return BadRequest("Could not delete user!");
            }
        }


        private bool AppUserExists(string id)
        {
            return _repository.AppUserExists(id);
        }
    }
}
