using Api.Data;
using Api.Dto;
using Api.Models;
using Api.Services;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public UserRepository(ApplicationDbContext context, IAuthService authService){
            _context = context;
            _authService = authService;
        }

        public bool AppUserExists(string id)
        {
            return _context.AppUser.Any(e => e.Id == id);
        }

        public async Task<bool> DeleteUser(string userid)
        {
            var user = await _context.AppUser.Include(x => x.Workout).FirstOrDefaultAsync(x => x.Id == userid);

            if (user == null){
                return false;
            }

            try{
                _context.AppUser.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }   
            catch{
                return false;
            }

        }

        public async Task<UserResponseDto?> GetUserById(string userid)
        {
            var user = await _context.AppUser.FindAsync(userid);

            if(user == null){
                return null;
            }

            var userDto = new UserResponseDto{
                Id = userid,
                UserName = user.UserName,
            };

            return userDto;
        }

        public async Task<List<UserResponseDto>> GetUsers()
        {
            var users = await _context.AppUser.ToListAsync();
            var userDto = users.Select(user => new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
            }).ToList();

            return userDto;
        }

        public async Task<string?> Login(LoginDto request)
        {
            if(request == null){
                return null;
            }
            var response = await _authService.Login(request);

            return response;
        }

        public async Task<string?> Register(RegisterDto request)
        {
            if(request == null){
                return null;
            }
            var response = await _authService.Register(request);

            return response;
        }

        public async Task<string?> RegisterAdmin(RegisterDto request)
        {
            if(request == null){
                return null;
            }
            var response = await _authService.Register(request);

            return response;
        }
    }
}