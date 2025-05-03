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
    /// Gestion des tâches des projets
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        /// <summary>
        /// Récupère toutes les tâches des projets de l'utilisateur
        /// </summary>
        /// <returns>Liste des tâches avec leurs commentaires</returns>
        /// <response code="200">Retourne la liste des tâches</response>
        /// <response code="401">L'utilisateur n'est pas authentifié</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ProjectTask>>> GetTasks()
        {
            var userId = User.GetUserId();
            var currentUser = await _context.Users.FindAsync(userId);
            var query = _context.Tasks
                .Include(t => t.Project)
                .ThenInclude(p => p.User)
                .AsQueryable();

            // Les utilisateurs normaux ne voient que les tâches de leurs projets
            if (currentUser?.Role != Role.Admin)
            {
                query = query.Where(t => t.Project.UserId == userId);
            }

            var tasks = await query.ToListAsync();

            // Si l'utilisateur n'est pas admin, on vérifie qu'il a accès à au moins une tâche
            if (currentUser?.Role != Role.Admin && !tasks.Any())
            {
                return NotFound("No tasks found");
            }

            return tasks;
        }

        [HttpGet("{id}")]
        /// <summary>
        /// Récupère une tâche spécifique avec ses commentaires
        /// </summary>
        /// <param name="id">Identifiant de la tâche</param>
        /// <returns>La tâche demandée</returns>
        /// <response code="200">Retourne la tâche demandée</response>
        /// <response code="404">La tâche n'existe pas ou n'appartient pas à l'utilisateur</response>
        /// <response code="401">L'utilisateur n'est pas authentifié</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ProjectTask>> GetTask(int id)
        {
            var userId = User.GetUserId();
            var currentUser = await _context.Users.FindAsync(userId);
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound("Task not found");
            }

            // Les utilisateurs normaux ne peuvent voir que les tâches de leurs projets
            if (currentUser?.Role != Role.Admin)
            {
                if (task.Project.UserId != userId)
                {
                    return NotFound("Task not found or access denied");
                }
            }

            return task;
        }

        [HttpPost]
        /// <summary>
        /// Crée une nouvelle tâche dans un projet
        /// </summary>
        /// <param name="task">Les informations de la tâche à créer</param>
        /// <returns>La tâche créée</returns>
        /// <remarks>
        /// Exemple de requête :
        /// 
        ///     POST /api/tasks
        ///     {
        ///         "title": "Ma Tâche",
                ///         "status": "ToDo",
        ///         "projectId": 1,
        ///         "dueDate": "2024-12-31T23:59:59Z"
        ///     }
        /// 
        /// </remarks>
        /// <response code="201">Retourne la tâche créée</response>
        /// <response code="400">Les données de la tâche sont invalides</response>
        /// <response code="404">Le projet spécifié n'existe pas</response>
        /// <response code="401">L'utilisateur n'est pas authentifié</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ProjectTask>> CreateTask(ProjectTask task)
        {
            var userId = User.GetUserId();
            var currentUser = await _context.Users.FindAsync(userId);
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == task.ProjectId);

            if (project == null)
            {
                return NotFound("Project not found");
            }

            // Les utilisateurs normaux ne peuvent créer des tâches que dans leurs projets
            if (currentUser?.Role != Role.Admin)
            {
                if (project.UserId != userId)
                {
                    return NotFound("Project not found or access denied");
                }
            }

            task.DueDate = task.DueDate != default ? task.DueDate : DateTime.UtcNow.AddDays(7); // Date d'échéance par défaut à 7 jours

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        /// <summary>
        /// Met à jour une tâche existante
        /// </summary>
        /// <param name="id">Identifiant de la tâche</param>
        /// <param name="updatedTask">Les nouvelles informations de la tâche</param>
        /// <returns>Aucun contenu en cas de succès</returns>
        /// <response code="204">La tâche a été mise à jour avec succès</response>
        /// <response code="404">La tâche n'existe pas ou n'appartient pas à l'utilisateur</response>
        /// <response code="400">Les données de la tâche sont invalides</response>
        /// <response code="401">L'utilisateur n'est pas authentifié</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateTask(int id, ProjectTask updatedTask)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            task.Title = updatedTask.Title;
                    task.Status = updatedTask.Status;
            task.DueDate = updatedTask.DueDate;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        /// <summary>
        /// Supprime une tâche
        /// </summary>
        /// <param name="id">Identifiant de la tâche à supprimer</param>
        /// <returns>Aucun contenu en cas de succès</returns>
        /// <response code="204">La tâche a été supprimée avec succès</response>
        /// <response code="404">La tâche n'existe pas ou n'appartient pas à l'utilisateur</response>
        /// <response code="401">L'utilisateur n'est pas authentifié</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = User.GetUserId();
            var currentUser = await _context.Users.FindAsync(userId);
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound("Task not found");
            }

            // Les utilisateurs normaux ne peuvent supprimer que les tâches de leurs projets
            if (currentUser?.Role != Role.Admin)
            {
                if (task.Project.UserId != userId)
                {
                    return NotFound("Task not found or access denied");
                }
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/comments")]
        /// <summary>
        /// Ajoute un commentaire à une tâche
        /// </summary>
        /// <param name="id">Identifiant de la tâche</param>
        /// <param name="comment">Le commentaire à ajouter</param>
        /// <returns>Le commentaire créé</returns>
        /// <remarks>
        /// Exemple de requête :
        /// 
        ///     POST /api/tasks/{id}/comments
        ///     {
        ///         "content": "Mon commentaire sur la tâche"
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProjectTask>> AddComment(int id, [FromBody] string commentaire)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            task.Commentaire.Add(commentaire);
            await _context.SaveChangesAsync();

            return Ok(task);
        }
    }
}
