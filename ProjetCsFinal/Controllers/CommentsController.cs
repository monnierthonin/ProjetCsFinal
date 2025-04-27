using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL;
using System.Security.Claims;


namespace ProjetCsFinal.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var comments = await _context.Comments
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return comments;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var comment = await _context.Comments
                .Where(c => c.Id == id && c.UserId == userId)
                .FirstOrDefaultAsync();

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        [HttpPost]
        public async Task<ActionResult<Comment>> CreateComment(Comment comment)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            comment.CreatedAt = DateTime.UtcNow;
            comment.UserId = userId;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, Comment updatedComment)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (comment == null)
            {
                return NotFound();
            }

            comment.Content = updatedComment.Content;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
