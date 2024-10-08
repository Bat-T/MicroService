﻿using Mango.MessageBus;
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
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        protected ResponseDTO _response;

        public AuthController(IAuthService authService,IMessageBus messageBus,IConfiguration configuration)
        {
            this.authService = authService;
            _configuration = configuration;
            _messageBus = messageBus;
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
            await _messageBus.PublishMessage(registrationRequestDTO.Email, _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
        {
            var loginResponse = await authService.Login(loginRequestDTO);
            if(loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Invalid UserName or Password";
                return BadRequest(_response);
            }
            _response.IsSuccess = true;
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole(RegistrationRequestDTO requestDTO)
        {
            var loginResponse = await authService.AssignRole(requestDTO.Email,requestDTO.Role);
            if (!loginResponse)
            {
                _response.IsSuccess = false;
                _response.Message = "Error Occured";
                return BadRequest(_response);
            }
            _response.IsSuccess = true;
            _response.Result = "";
            return Ok(_response);
        }
    }
}
