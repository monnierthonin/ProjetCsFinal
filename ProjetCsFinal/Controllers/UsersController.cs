using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DAL.Models;
using DAL;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using BCrypt.Net;

namespace ProjetCsFinal.Controllers
{
    /// <summary>
    /// Gestion des utilisateurs et de l'authentification
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        /// <summary>
        /// Enregistre un nouvel utilisateur
        /// </summary>
        /// <param name="user">Les informations de l'utilisateur à créer</param>
        /// <returns>Le token JWT et les informations de l'utilisateur créé</returns>
        /// <response code="200">Utilisateur créé avec succès</response>
        /// <response code="400">Le nom d'utilisateur existe déjà</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<object>> Register(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                return BadRequest(new { Success = false, Message = "Username already exists" });
            }

            // Hash du mot de passe et définition du rôle
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.Role = UserRole.User;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            return Ok(new
            {
                Success = true,
                Token = token,
                Message = "Registration successful",
                UserId = user.Id
            });
        }

        [HttpPost("login")]
        /// <summary>
        /// Authentifie un utilisateur
        /// </summary>
        /// <param name="username">Nom d'utilisateur</param>
        /// <param name="password">Mot de passe</param>
        /// <returns>Le token JWT et les informations de l'utilisateur</returns>
        /// <response code="200">Authentification réussie</response>
        /// <response code="401">Identifiants invalides</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<object>> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return Unauthorized(new { Success = false, Message = "Invalid username or password" });
            }

            var token = GenerateJwtToken(user);
            return Ok(new
            {
                Success = true,
                Token = token,
                Message = "Login successful"
            });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
