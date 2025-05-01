using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL;
using System.Security.Claims;
using ProjetCsFinal.Extensions;


namespace ProjetCsFinal.Controllers
{
    /// <summary>
    /// Gestion des projets
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        /// <summary>
        /// Récupère tous les projets de l'utilisateur connecté
        /// </summary>
        /// <returns>Liste des projets de l'utilisateur</returns>
        /// <response code="200">Retourne la liste des projets</response>
        /// <response code="401">L'utilisateur n'est pas authentifié</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects.Include(p => p.Tasks).ToListAsync();
        }

        [HttpGet("{id}")]
        /// <summary>
        /// Récupère un projet spécifique
        /// </summary>
        /// <param name="id">Identifiant du projet</param>
        /// <returns>Le projet demandé</returns>
        /// <response code="200">Retourne le projet demandé</response>
        /// <response code="404">Le projet n'existe pas ou n'appartient pas à l'utilisateur</response>
        /// <response code="401">L'utilisateur n'est pas authentifié</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        [HttpPost]
        [Authorize]
        /// <summary>
        /// Crée un nouveau projet pour l'utilisateur connecté
        /// </summary>
        /// <param name="project">Les informations du projet à créer</param>
        /// <returns>Le projet créé</returns>
        /// <remarks>
        /// Exemple de requête :
        /// 
        ///     POST /api/projects
        ///     {
        ///         "name": "Mon Projet",
        ///         "description": "Description du projet"
        ///     }
        /// 
        /// </remarks>
        /// <response code="201">Retourne le projet créé</response>
        /// <response code="400">Les données du projet sont invalides</response>
        /// <response code="401">L'utilisateur n'est pas authentifié</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Project>> CreateProject(Project project)
        {
            // Récupérer le userId du token JWT
            var userId = User.GetUserId();
            project.UserId = userId;
            project.CreationDate = DateTime.UtcNow;

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        [HttpPut("{id}")]
        /// <summary>
        /// Met à jour un projet existant
        /// </summary>
        /// <param name="id">Identifiant du projet</param>
        /// <param name="updatedProject">Les nouvelles informations du projet</param>
        /// <returns>Aucun contenu en cas de succès</returns>
        /// <response code="204">Le projet a été mis à jour avec succès</response>
        /// <response code="404">Le projet n'existe pas ou n'appartient pas à l'utilisateur</response>
        /// <response code="400">Les données du projet sont invalides</response>
        /// <response code="401">L'utilisateur n'est pas authentifié</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateProject(int id, Project updatedProject)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            project.Name = updatedProject.Name;
            project.Description = updatedProject.Description;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        /// <summary>
        /// Supprime un projet
        /// </summary>
        /// <param name="id">Identifiant du projet à supprimer</param>
        /// <returns>Aucun contenu en cas de succès</returns>
        /// <response code="204">Le projet a été supprimé avec succès</response>
        /// <response code="404">Le projet n'existe pas ou n'appartient pas à l'utilisateur</response>
        /// <response code="401">L'utilisateur n'est pas authentifié</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
