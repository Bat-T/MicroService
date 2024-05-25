using Mango.Services.AuthAPI.Data.DTO;
using Mango.Services.AuthAPI.Models.DTO;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        protected ResponseDTO _response;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
            _response = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            var registraionResponse =await authService.Register(registrationRequestDTO);
            if(!string.IsNullOrEmpty(registraionResponse))
            {
                _response.IsSuccess = false;
                _response.Message = registraionResponse;
                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
        {
            var loginResponse = await authService.Login(loginRequestDTO);
            if(loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Imvalid UserName or Password";
                return BadRequest(_response);
            }
            _response.IsSuccess = true;
            _response.Result = loginResponse;
            return Ok(_response);
        }
    }
}
