using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DAL.Models;
using DAL;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ProjetCsFinal.Extensions;
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
        [HttpGet]
        /// <summary>
        /// Récupère la liste des utilisateurs
        /// </summary>
        /// <returns>Liste des utilisateurs</returns>
        /// <response code="200">Retourne la liste des utilisateurs</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        /// <summary>
        /// Récupère un utilisateur spécifique
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur</param>
        /// <returns>L'utilisateur demandé</returns>
        /// <response code="200">Retourne l'utilisateur demandé</response>
        /// <response code="404">Utilisateur non trouvé</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPut("{id}")]
        /// <summary>
        /// Met à jour un utilisateur
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur</param>
        /// <param name="updatedUser">Nouvelles informations de l'utilisateur</param>
        /// <returns>Aucun contenu en cas de succès</returns>
        /// <response code="204">Mise à jour réussie</response>
        /// <response code="404">Utilisateur non trouvé</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            if (!string.IsNullOrEmpty(updatedUser.PasswordHash))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updatedUser.PasswordHash);
            }
            user.Role = updatedUser.Role;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        /// <summary>
        /// Supprime un utilisateur
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur à supprimer</param>
        /// <returns>Aucun contenu en cas de succès</returns>
        /// <response code="204">Suppression réussie</response>
        /// <response code="404">Utilisateur non trouvé</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

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
        /// <remarks>
        /// Exemple de requête :
        ///
        ///     POST /api/users/register
        ///     {
        ///         "name": "John Doe",
        ///         "email": "john.doe@example.com",
        ///         "passwordHash": "MonMotDePasse123!"
        ///     }
        ///
        /// Note : Ne pas remplir les champs id, role, projects, comments, etc. Ils seront gérés automatiquement.
        /// </remarks>
        /// <response code="200">Utilisateur créé avec succès</response>
        /// <response code="400">Le nom d'utilisateur existe déjà</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<object>> Register(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Name == user.Name))
            {
                return BadRequest(new { Success = false, Message = "Name already exists" });
            }

            // Hash du mot de passe et définition du rôle
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.Role = Role.User;
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
        public async Task<ActionResult<object>> Login([FromQuery] string username, [FromQuery] string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == username);

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
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
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
