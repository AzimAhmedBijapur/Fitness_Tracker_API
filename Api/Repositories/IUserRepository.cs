using Api.Dto;
using Api.Models;

namespace Api.Repositories{
    public interface IUserRepository{
        public Task<string?> Login(LoginDto request);
        public Task<string?> Register(RegisterDto request);
        public Task<string?> RegisterAdmin(RegisterDto request);
        public Task<List<UserResponseDto>> GetUsers();
        public Task<UserResponseDto?> GetUserById(string userid);
        public Task<bool> DeleteUser(string userid);
        public bool AppUserExists(string id);

    }
}