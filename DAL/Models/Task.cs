using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    /// <summary>
    /// Représente une tâche dans le système
    /// </summary>
    public class ProjectTask
    {
        /// <summary>
        /// Identifiant unique de la tâche
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Titre de la tâche
        /// </summary>
        [Required]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Description de la tâche
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Statut de la tâche
        /// </summary>
        [Required]
        public ProjectTaskStatus Status { get; set; }

        /// <summary>
        /// ID du projet associé
        /// </summary>
        [Required]
        public int ProjectId { get; set; }

        /// <summary>
        /// Projet associé à la tâche
        /// </summary>
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; } = null!;

        /// <summary>
        /// Date d'échéance de la tâche
        /// </summary>
        [Required]
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Collection de commentaires pour la tâche
        /// </summary>
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }

    /// <summary>
    /// Énumération des statuts possibles pour une tâche
    /// </summary>
    public enum ProjectTaskStatus
    {
        [Display(Name = "À faire")]
        ToDo,
        [Display(Name = "En cours")]
        InProgress,
        [Display(Name = "Terminé")]
        Done
    }
}
