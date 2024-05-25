using Mango.Services.AuthAPI.Data.DTO;

namespace Mango.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDTO registrationRequest);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest);
    }
}
