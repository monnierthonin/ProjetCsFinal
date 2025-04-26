using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    /// <summary>
    /// Représente une tâche dans le système de gestion de projets
    /// </summary>
    /// <remarks>
    /// Une tâche est une unité de travail qui :
    /// - Appartient à un projet spécifique
    /// - Peut avoir différents statuts (A faire, En cours, Terminé)
    /// - Peut avoir une date d'échéance
    /// - Peut contenir des commentaires
    /// </remarks>
    public class ProjectTask
    {
        /// <summary>
        /// Identifiant unique de la tâche
        /// </summary>
        /// <example>1</example>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Titre de la tâche
        /// </summary>
        /// <example>Implémenter l'authentification JWT</example>
        [Required(ErrorMessage = "Le titre de la tâche est requis")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Le titre doit contenir entre 3 et 200 caractères")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Description détaillée de la tâche
        /// </summary>
        /// <example>Mettre en place l'authentification JWT avec validation des tokens</example>
        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Statut actuel de la tâche
        /// </summary>
        /// <example>EnCours</example>
        [Required(ErrorMessage = "Le statut de la tâche est requis")]
        [EnumDataType(typeof(ProjectTaskStatus), ErrorMessage = "Statut invalide")]
        public ProjectTaskStatus Status { get; set; }

        /// <summary>
        /// Identifiant du projet auquel la tâche appartient
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "L'identifiant du projet est requis")]
        public int ProjectId { get; set; }

        /// <summary>
        /// Projet auquel la tâche appartient
        /// </summary>
        /// <remarks>
        /// Navigation property pour Entity Framework
        /// Permet d'accéder aux informations du projet parent
        /// </remarks>
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; } = null!;

        /// <summary>
        /// Date d'échéance de la tâche (UTC)
        /// </summary>
        /// <example>2025-05-26T19:45:51Z</example>
        [Required]
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Collection des commentaires associés à la tâche
        /// </summary>
        /// <remarks>
        /// Navigation property pour Entity Framework
        /// Permet d'accéder à l'historique des commentaires
        /// </remarks>
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }

    /// <summary>
    /// Énumération des statuts possibles pour une tâche
    /// </summary>
    /// <remarks>
    /// - AFaire : Tâche non commencée
    /// - EnCours : Tâche en cours de réalisation
    /// - Termine : Tâche terminée
    /// </remarks>
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
