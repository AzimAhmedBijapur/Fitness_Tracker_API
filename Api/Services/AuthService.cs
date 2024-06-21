using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Dto;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Api.Services{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService (UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> Register(RegisterDto request)
        {
            if(request.UserName == null || request.Password == null){
                throw new Exception("Username and Password is required!");
            }
            var userByUsername = await _userManager.FindByNameAsync(request.UserName);
            if (userByUsername is not null)
            {
                throw new ArgumentException($"User with username {request.UserName} already exists.");
            }

            AppUser user = new()
            {
                UserName = request.UserName,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if(!result.Succeeded)
            {
                throw new ArgumentException($"Unable to register user {request.UserName} errors: {GetErrorsText(result.Errors)}");
            }

            return await Login(new LoginDto { UserName = request.UserName, Password = request.Password });
        }

        public async Task<string> RegisterAdmin (RegisterDto request)
        {
            if(request.UserName == null || request.Password == null){
                throw new Exception("Username and Password is required!");
            }

            if(request.Password != _configuration["AdminKey"]){
                throw new ArgumentException($"Admin can't be registered!");   
            }

            var userByUsername = await _userManager.FindByNameAsync(request.UserName);
            if (userByUsername is not null)
            {
                throw new ArgumentException($"User with username {request.UserName} already exists.");
            }

    
            AppUser user = new()
            {
                UserName = request.UserName,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if(result.Succeeded)
            {
                var role = await _userManager.AddToRoleAsync(user, "Admin");

                if(!role.Succeeded){
                    
                    throw new ArgumentException($"Unable to register user {request.UserName} errors: {GetErrorsText(result.Errors)}");
                }
            }
            else{
                throw new ArgumentException($"Unable to register user {request.UserName} errors: {GetErrorsText(result.Errors)}");
            }

            return await Login(new LoginDto { UserName = request.UserName, Password = request.Password });
        }

        public async Task<string> Login(LoginDto request)
        {
            if(request.UserName == null || request.Password == null){
                throw new Exception("Username and Password is required!");
            }

            var user = await _userManager.FindByNameAsync(request.UserName);

            if(user == null){
                throw new Exception("User not found!");
            }

            var role = await _userManager.GetRolesAsync(user);

            if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                Console.WriteLine("User does not exist");
                throw new ArgumentException($"Unable to authenticate user {request.UserName}");
            }

            if(user.UserName== null){
                throw new Exception("Username is required!");
            }

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
            };

            if (role != null && role.Any())
            {
                foreach (var r in role)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, r));
                }
            }

            var token = GetToken(authClaims);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
        {
            var signInKey =_configuration["JWT:SignInKey"] ;
            if(signInKey == null){
                throw new Exception("Could not generate token");
            }
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signInKey));

            try{

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));
                return token;
            }
            catch{
                throw new Exception("Issue in generating token!");
            }

        }
        
        private string GetErrorsText(IEnumerable<IdentityError> errors)
        {
            return string.Join(", ", errors.Select(error => error.Description).ToArray());
        }
    }


}