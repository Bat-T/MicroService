using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mango.Services.AuthAPI.Service
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IOptions<JwtOptions> jwtOptions;

        public JwtTokenGenerator(IOptions<JwtOptions> options)
        {
            this.jwtOptions = options;
        }

        public string GenerateToken(ApplicationUser applicationUser,IEnumerable<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            //key
            var key = Encoding.ASCII.GetBytes(jwtOptions.Value.Secret);

            //audience
            var claimList = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Name,applicationUser.Name),
                new Claim(JwtRegisteredClaimNames.Email,applicationUser.Email),
                new Claim(JwtRegisteredClaimNames.Sub,applicationUser.Id)
            };

            claimList.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            //tokendescripptor
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Audience = jwtOptions.Value.Audience,
                Issuer = jwtOptions.Value.Issuer,
                Subject = new ClaimsIdentity(claimList),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
