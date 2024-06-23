using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService baseService;

        public AuthService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<ResponseDTO?> AssignRoleAsync(RegistrationRequestDTO assignroleRequest)
        {
            return await baseService.SendAsync(new RequestDTO()
            {
                ApiType = Utility.SD.ApiType.POST,
                Data = assignroleRequest,
                Url = SD.AuthAPIBase + "/api/auth/AssignRole"
            },true);
        }

        public async Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequest)
        {
            return await baseService.SendAsync(new RequestDTO()
            {
                ApiType = Utility.SD.ApiType.POST,
                Data = loginRequest,
                Url = SD.AuthAPIBase + "/api/auth/login"
            },false);
        }

        public async Task<ResponseDTO?> RegisterAsync(RegistrationRequestDTO registrationRequest)
        {
            return await baseService.SendAsync(new RequestDTO()
            {
                ApiType = Utility.SD.ApiType.POST,
                Data = registrationRequest,
                Url = SD.AuthAPIBase + "/api/auth/register"
            },false);
        }
    }
}
