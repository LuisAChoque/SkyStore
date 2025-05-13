using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SkyStore.Data;
using SkyStore.Interfaces;
using SkyStore.Models;
using SkyStore.Settings;

namespace SkyStore.Services
{
    public class UserAuthService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtSettings _jwtSettings;
        public UserAuthService(ApplicationDbContext context,IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }

        public bool RegisterUser(UserRegister request)
        {

            if (this.userExists(request.Username))
            {
                throw new InvalidOperationException("Este usuario ya existe"); // Usuario ya existe
            }

            // Crear y almacenar el nuevo usuario
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = request.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "User"
            };
            _context.Users.Add(user);
           _context.SaveChanges();

            return true;
        }

        public bool userExists(string username)
        {
            return (_context.Users.FirstOrDefault(u=>u.Username == username)!=null);
        }

        public string LoginUser(UserLogin request)
        {
            // Verificar que el usuario existe y que la contraseña es correcta

            if (!this.userExists(request.Username))
            {
                throw new InvalidOperationException("este usuario no existe"); // Usuario no existe
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                throw new InvalidOperationException("La Contraseña es incorrecta"); // contraseña incorrecta
            }

            // Generar y retornar el token JWT
           return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", user.Id.ToString()),   // Incluir ID del usuario
                new Claim("username", user.Username),  // Incluir nombre de usuario
                new Claim("role", user.Role)           // Incluir rol del usuario
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_jwtSettings.ExpirationInMinutes)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
