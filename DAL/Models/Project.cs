using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    /// <summary>
    /// Représente un projet dans le système de gestion de tâches
    /// </summary>
    /// <remarks>
    /// Un projet est une entité qui :
    /// - Appartient à un utilisateur spécifique
    /// - Contient une collection de tâches associées
    /// - Permet d'organiser et de regrouper les tâches
    /// </remarks>
    public class Project
    {
        /// <summary>
        /// Identifiant unique du projet
        /// </summary>
        /// <example>1</example>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nom du projet
        /// </summary>
        /// <example>Projet TaskFlow API</example>
        [Required(ErrorMessage = "Le nom du projet est requis")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Le nom du projet doit contenir entre 3 et 100 caractères")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description détaillée du projet
        /// </summary>
        /// <example>Développement d'une API REST pour la gestion de tâches</example>
        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        public string? Description { get; set; }

        /// <summary>
        /// Date de création du projet (auto-générée)
        /// </summary>
        [Required]
        [JsonIgnore]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Identifiant de l'utilisateur propriétaire
        /// </summary>
        /// <example>1</example>
        [JsonIgnore]
        [Required(ErrorMessage = "L'identifiant de l'utilisateur est requis")]
        public int UserId { get; set; }

        /// <summary>
        /// Utilisateur propriétaire du projet
        /// </summary>
        /// <remarks>
        /// Navigation property pour Entity Framework
        /// Permet d'accéder aux informations de l'utilisateur propriétaire
        /// </remarks>
        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual User? User { get; set; }

        /// <summary>
        /// Collection des tâches associées au projet
        /// </summary>
        /// <remarks>
        /// Navigation property pour Entity Framework
        /// Permet d'accéder à toutes les tâches du projet
        /// </remarks>
        [JsonIgnore]
        public virtual ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    }
}
