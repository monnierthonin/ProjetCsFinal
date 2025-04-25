using DAL.Models;

namespace ProjetCsFinal.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ProjectTaskStatus Status { get; set; }
        public int ProjectId { get; set; }
        public DateTime DueDate { get; set; }
        public ICollection<CommentDto> Comments { get; set; } = new HashSet<CommentDto>();
    }
}
