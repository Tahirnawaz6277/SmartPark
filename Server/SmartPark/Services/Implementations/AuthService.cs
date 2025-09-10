using Microsoft.IdentityModel.Tokens;
using SmartPark.Dtos.UserDtos;
using SmartPark.Models;
using SmartPark.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartPark.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ICryptoService _cryptoService;
        private readonly IConfiguration _configuration;
        private readonly IHelper _helper;

        public AuthService(ICryptoService cryptoService, IConfiguration configuration, IHelper helper)
        {
            _cryptoService = cryptoService;
            _configuration = configuration;
            _helper = helper;
        }

        public async Task<LoginResponse> AuthenticateAsync(string email, string password)
        {
            var user = await _helper.GetActiveUserAsync(email);
            //handling password
            var encryptedPasswrod = _cryptoService.Encrypt(password);
            if (user == null || user.Password != encryptedPasswrod)
            {
                throw new Exception("Invalid Credentails");
            }
            var token = GenerateUserTokenAsync(user);
            var loginResponse = new LoginResponse
            {
                Name = user.Name,
                Email = user.Email,
                AccessToken = await token
            };
            return loginResponse;
        }

        private async Task<string> GenerateUserTokenAsync(User user)
        {

            var jwtSettings = _configuration.GetSection("Jwt");
            var key = jwtSettings.GetValue<string>("Key");
            var tokenExpiry = jwtSettings.GetValue<int>("TokenExpiry");
            //var issuer = jwtSettings.GetValue<string>("Issuer");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var Credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user?.Role?.RoleName),

            };
            // Add role claims - one for each role
            //foreach (var userRole in user.TblUserRoles)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, userRole.RoleId.ToString() ?? string.Empty));
            //}


            var token = new JwtSecurityToken
                (
                    //issuer: issuer,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(tokenExpiry),
                    signingCredentials: Credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
