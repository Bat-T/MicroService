﻿using Mango.Services.AuthAPI.Models.DTO;

namespace Mango.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDTO registrationRequest);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest);
        Task<bool> AssignRole(string user, string role);
    }
}
