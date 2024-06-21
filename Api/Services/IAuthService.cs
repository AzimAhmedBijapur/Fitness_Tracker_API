using Api.Dto;

namespace Api.Services{

    public interface IAuthService
    {
        Task<string> Register(RegisterDto request);
        Task<string> RegisterAdmin(RegisterDto request);
        Task<string> Login(LoginDto request);
    }

}
