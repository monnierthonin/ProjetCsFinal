using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    /// <summary>
    /// Représente un projet dans le système
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Identifiant unique du projet
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nom du projet
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description optionnelle du projet
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Date de création du projet (auto-générée)
        /// </summary>
        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// ID de l'utilisateur propriétaire
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Utilisateur propriétaire du projet
        /// </summary>
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        /// <summary>
        /// Collection des tâches associées au projet
        /// </summary>
        public virtual ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    }
}
