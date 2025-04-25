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
    public class GradesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GradesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GradeDto>>> GetGrades()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var grades = await _context.Grades
                .Where(g => g.UserId == userId)
                .Select(g => new GradeDto
                {
                    Id = g.Id,
                    Value = g.Value,
                    Comment = g.Comment,
                    CreatedAt = g.CreatedAt,
                    ProjectId = g.ProjectId,
                    UserId = g.UserId
                })
                .ToListAsync();

            return grades;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GradeDto>> GetGrade(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var grade = await _context.Grades
                .Where(g => g.Id == id && g.UserId == userId)
                .Select(g => new GradeDto
                {
                    Id = g.Id,
                    Value = g.Value,
                    Comment = g.Comment,
                    CreatedAt = g.CreatedAt,
                    ProjectId = g.ProjectId,
                    UserId = g.UserId
                })
                .FirstOrDefaultAsync();

            if (grade == null)
            {
                return NotFound();
            }

            return grade;
        }

        [HttpPost]
        public async Task<ActionResult<GradeDto>> CreateGrade(GradeDto gradeDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            var grade = new Grade
            {
                Value = gradeDto.Value,
                Comment = gradeDto.Comment,
                CreatedAt = DateTime.UtcNow,
                ProjectId = gradeDto.ProjectId,
                UserId = userId
            };

            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();

            gradeDto.Id = grade.Id;
            gradeDto.CreatedAt = grade.CreatedAt;
            gradeDto.UserId = userId;

            return CreatedAtAction(nameof(GetGrade), new { id = grade.Id }, gradeDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGrade(int id, GradeDto gradeDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var grade = await _context.Grades.FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (grade == null)
            {
                return NotFound();
            }

            grade.Value = gradeDto.Value;
            grade.Comment = gradeDto.Comment;

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
