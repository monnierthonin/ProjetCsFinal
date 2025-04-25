namespace ProjetCsFinal.DTOs
{
    public class GradeDto
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
    }
}
