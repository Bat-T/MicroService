using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IAuthService
    {
        public Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequest);
        public Task<ResponseDTO?> RegisterAsync(RegistrationRequestDTO loginRequest);
        public Task<ResponseDTO?> AssignRoleAsync(RegistrationRequestDTO loginRequest);
    }
}
