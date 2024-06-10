using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService authService;
        private readonly ITokenProvider tokenProvider;

        public AuthController(IAuthService authService,ITokenProvider tokenProvider)
        {
            this.authService = authService;
            this.tokenProvider = tokenProvider;
        }
        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDTO loginRequestDTO = new();
            return View(loginRequestDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequest)
        {
            var loginresponse = await authService.LoginAsync(loginRequest);
            ResponseDTO role;
            if (loginresponse != null && loginresponse.IsSuccess)
            {
                LoginResponseDTO response = JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(loginresponse?.Result));
                await SignInUser(response);
                TempData["success"] = "Login Successful";
                return RedirectToAction("Index","Home");
            }
            else
            {
                TempData["error"] = loginresponse?.Message;
                return View(loginRequest);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            RegistrationRequestDTO registrationRequestDTO = new();
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text = SD.RoleAdmin,Value=SD.RoleAdmin},
                new SelectListItem{Text = SD.RoleCustomer,Value=SD.RoleCustomer}
            };
            ViewBag.RoleList = roleList;
            return View(registrationRequestDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDTO registrationRequest)
        {
            var result = await authService.RegisterAsync(registrationRequest);
            ResponseDTO role;
            if (result != null && result.IsSuccess)
            {
                if (string.IsNullOrEmpty(registrationRequest.Role))
                {
                    registrationRequest.Role = SD.RoleCustomer;
                }
                role = await authService.AssignRoleAsync(registrationRequest);

                if (role != null && role.IsSuccess)
                {
                    TempData["success"] = "Registration Successful";
                    return RedirectToAction(nameof(Login));
                }
                TempData["error"] = role?.Message;
            }
            else
            {
                TempData["error"] = result?.Message;
            }
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text = SD.RoleAdmin,Value=SD.RoleAdmin},
                new SelectListItem{Text = SD.RoleCustomer,Value=SD.RoleCustomer}
            };
            ViewBag.RoleList = roleList;
            return View(registrationRequest);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        [NonAction]
        private async Task SignInUser(LoginResponseDTO responseDTO)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(responseDTO.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(x => x.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
