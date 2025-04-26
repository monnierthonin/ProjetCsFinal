using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    /// <summary>
    /// Représente un commentaire associé à une tâche
    /// </summary>
    /// <remarks>
    /// Un commentaire permet de :
    /// - Suivre l'historique des discussions sur une tâche
    /// - Ajouter des notes et des précisions
    /// - Identifier l'auteur et la date de création
    /// </remarks>
    public class Comment
    {
        /// <summary>
        /// Identifiant unique du commentaire
        /// </summary>
        /// <example>1</example>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Contenu du commentaire
        /// </summary>
        /// <example>Ceci est un exemple de commentaire</example>
        [Required(ErrorMessage = "Le contenu du commentaire est requis")]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Date et heure de création du commentaire
        /// </summary>
        /// <example>2022-01-01T12:00:00</example>
        [Required(ErrorMessage = "La date de création est requise")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Identifiant de la tâche associée au commentaire
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "L'identifiant de la tâche est requis")]
        public int TaskId { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur qui a créé le commentaire
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "L'identifiant de l'utilisateur est requis")]
        public int UserId { get; set; }

        [ForeignKey("TaskId")]
        public virtual ProjectTask Task { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
