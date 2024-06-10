using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.DTO;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext db;
        private readonly UserManager<ApplicationUser> usermanager;
        private readonly RoleManager<IdentityRole> rolemanager;
        private readonly IJwtTokenGenerator jwtTokenGenerator;

        public AuthService(AppDbContext db, UserManager<ApplicationUser> user, RoleManager<IdentityRole> role,IJwtTokenGenerator jwtTokenGenerator)
        {
            this.db = db;
            this.usermanager = user;
            this.jwtTokenGenerator = jwtTokenGenerator;
            this.rolemanager = role;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest)
        {
            try
            {
                var user = await usermanager.Users.FirstOrDefaultAsync(x => x.Email == loginRequest.UserName);
                if (user != null)
                {
                    var userPasswordValid = await usermanager.CheckPasswordAsync(user, loginRequest.Password);
                    if (userPasswordValid)
                    {
                        var roles = await usermanager.GetRolesAsync(user);
                        return new LoginResponseDTO
                        {
                            User = new()
                            {
                                Email = user.Email,
                                ID = user.Id,
                                Name = user.Name,
                                PhoneNumber = user.PhoneNumber
                            },
                            Token = jwtTokenGenerator.GenerateToken(user,roles)
                        };
                    }
                }
                return new LoginResponseDTO()
                {
                    User = null,
                    Token = ""
                };
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<string> Register(RegistrationRequestDTO registrationRequest)
        {
            try
            {
                ApplicationUser user = new()
                {
                    Email = registrationRequest.Email,
                    Name = registrationRequest.Name,
                    NormalizedEmail = registrationRequest.Email.ToUpper(),
                    PhoneNumber = registrationRequest.PhoneNumber,
                    UserName = registrationRequest.Email
                };

                var createdUser = await usermanager.CreateAsync(user, registrationRequest.PassWord);
                if (createdUser.Succeeded)
                {
                    //var newUser = await db.Users.FirstAsync(x => x.Email == registrationRequest.Email);

                    //return new UserDTO()
                    //{
                    //    Email = newUser.Email,
                    //    ID = newUser.Id,
                    //    Name = newUser.Name,
                    //    PhoneNumber = newUser.PhoneNumber
                    //};
                    return "";
                }
                return createdUser.Errors.FirstOrDefault().Description;
            }
            catch (Exception ex)
            {
            }
            return "Error Encountered";
        }

        public async Task<bool> AssignRole(string user,string role)
        {
            var userDb = await usermanager.Users.FirstOrDefaultAsync(x => x.Email == user);
            if (userDb == null)
                return false;
            if (!await rolemanager.RoleExistsAsync(role))
                await rolemanager.CreateAsync(new IdentityRole(role));

            await usermanager.AddToRoleAsync(userDb, role);

            return true;
        }
    }
}
