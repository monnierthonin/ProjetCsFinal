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
    public class GradesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GradesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Grade>>> GetGrades()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var grades = await _context.Grades
                .Where(g => g.UserId == userId)
                .ToListAsync();

            return grades;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Grade>> GetGrade(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var grade = await _context.Grades
                .Where(g => g.Id == id && g.UserId == userId)

                .FirstOrDefaultAsync();

            if (grade == null)
            {
                return NotFound();
            }

            return grade;
        }

        [HttpPost]
        public async Task<ActionResult<Grade>> CreateGrade(Grade grade)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            grade.UserId = userId;
            grade.CreatedAt = DateTime.UtcNow;

            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGrade), new { id = grade.Id }, grade);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGrade(int id, Grade updatedGrade)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var grade = await _context.Grades.FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (grade == null)
            {
                return NotFound();
            }

            grade.Value = updatedGrade.Value;
            grade.Comment = updatedGrade.Comment;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var grade = await _context.Grades.FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (grade == null)
            {
                return NotFound();
            }

            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
