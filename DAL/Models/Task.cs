using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
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
        [JsonIgnore]
        public int Id { get; set; }

        /// <summary>
        /// Titre de la tâche
        /// </summary>
        /// <example>Implémenter l'authentification JWT</example>
        [Required(ErrorMessage = "Le titre de la tâche est requis")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Le titre doit contenir entre 3 et 200 caractères")]
        public string Title { get; set; } = string.Empty;



        /// <summary>
        /// Statut actuel de la tâche
        /// </summary>
        /// <example>EnCours</example>
        [Required(ErrorMessage = "Le statut de la tâche est requis")]
        public Status Status { get; set; }

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
        [JsonIgnore]
        public virtual Project Project { get; set; } = null!;

        /// <summary>
        /// Date d'échéance de la tâche (UTC, optionnel)
        /// </summary>
        /// <example>2025-05-26T19:45:51Z</example>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Collection des commentaires (notes) associés à la tâche
        /// </summary>
        /// <remarks>
        /// Stocke l'historique des notes sous forme de strings
        /// </remarks>
        [JsonIgnore]
        public List<string> Commentaire { get; set; } = new List<string>();
    }

    /// <summary>
    /// Énumération des statuts possibles pour une tâche
    /// </summary>
    public enum Status
    {
        [Display(Name = "À faire")]
        ÀFaire,
        [Display(Name = "En cours")]
        EnCours,
        [Display(Name = "Terminé")]
        Terminé
    }
}
