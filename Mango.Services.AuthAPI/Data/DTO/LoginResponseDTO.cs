﻿namespace Mango.Services.AuthAPI.Data.DTO
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }

    }
}