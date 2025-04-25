using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL;
using System.Security.Claims;
using ProjetCsFinal.DTOs;

namespace ProjetCsFinal.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var tasks = await _context.Tasks
                .Include(t => t.Comments)
                .Where(t => t.Project.UserId == userId)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    ProjectId = t.ProjectId,
                    DueDate = t.DueDate,
                    Comments = t.Comments.Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        TaskId = c.TaskId,
                        UserId = c.UserId
                    }).ToList()
                })
                .ToListAsync();
            return tasks;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTask(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var task = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Comments)
                .Where(t => t.Id == id && t.Project.UserId == userId)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    ProjectId = t.ProjectId,
                    DueDate = t.DueDate,
                    Comments = t.Comments.Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        TaskId = c.TaskId,
                        UserId = c.UserId
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (task == null)
            {
                return NotFound();
            }

            return task;
        }

        [HttpPost]
        public async Task<ActionResult<TaskDto>> CreateTask(TaskDto taskDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == taskDto.ProjectId && p.UserId == userId);

            if (project == null)
            {
                return NotFound("Project not found or access denied");
            }

            var task = new ProjectTask
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                Status = taskDto.Status,
                ProjectId = taskDto.ProjectId,
                DueDate = taskDto.DueDate != default ? taskDto.DueDate : DateTime.UtcNow.AddDays(7) // Date d'échéance par défaut à 7 jours
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskDto taskDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project.UserId == userId);

            if (task == null)
            {
                return NotFound();
            }

            task.Title = taskDto.Title;
            task.Status = taskDto.Status;
            task.DueDate = taskDto.DueDate;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project.UserId == userId);

            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/comments")]
        public async Task<ActionResult<CommentDto>> AddComment(int id, CommentDto commentDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project.UserId == userId);

            if (task == null)
            {
                return NotFound();
            }

            var comment = new Comment
            {
                Content = commentDto.Content,
                CreatedAt = DateTime.UtcNow,
                TaskId = id,
                UserId = userId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            commentDto.Id = comment.Id;
            commentDto.CreatedAt = comment.CreatedAt;
            commentDto.TaskId = comment.TaskId;
            commentDto.UserId = comment.UserId;

            return CreatedAtAction(nameof(CommentsController.GetComment), 
                                 new { controller = "Comments", id = comment.Id }, 
                                 commentDto);
        }
    }
}
