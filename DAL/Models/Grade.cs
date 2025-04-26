using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    /// <summary>
    /// Représente une note attribuée à un projet
    /// </summary>
    /// <remarks>
    /// Une note permet de :
    /// - Évaluer un projet
    /// - Ajouter un commentaire justificatif
    /// - Identifier l'évaluateur et la date d'évaluation
    /// </remarks>
    public class Grade
    {
        /// <summary>
        /// Identifiant unique de la note
        /// </summary>
        /// <example>1</example>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Valeur numérique de la note (de 0 à 20)
        /// </summary>
        /// <example>15</example>
        [Required(ErrorMessage = "La note est requise")]
        [Range(0, 20, ErrorMessage = "La note doit être comprise entre 0 et 20")]
        public int Value { get; set; }
        /// <summary>
        /// Commentaire justifiant la note
        /// </summary>
        /// <example>Excellent projet, bonne implémentation des fonctionnalités demandées</example>
        [StringLength(500, ErrorMessage = "Le commentaire ne peut pas dépasser 500 caractères")]
        public string Comment { get; set; } = string.Empty;
        /// <summary>
        /// Date et heure de création de la note (UTC)
        /// </summary>
        /// <example>2025-04-26T19:50:20Z</example>
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Identifiant du projet évalué
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "L'identifiant du projet est requis")]
        public int ProjectId { get; set; }
        /// <summary>
        /// Identifiant de l'utilisateur ayant donné la note
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "L'identifiant de l'évaluateur est requis")]
        public int UserId { get; set; }

        /// <summary>
        /// Projet évalué
        /// </summary>
        /// <remarks>
        /// Navigation property pour Entity Framework
        /// Permet d'accéder aux informations du projet noté
        /// </remarks>
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; } = null!;
        /// <summary>
        /// Utilisateur ayant donné la note (évaluateur)
        /// </summary>
        /// <remarks>
        /// Navigation property pour Entity Framework
        /// Permet d'accéder aux informations de l'évaluateur
        /// </remarks>
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
