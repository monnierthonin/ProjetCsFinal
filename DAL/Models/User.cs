using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DAL.Models
{
    /// <summary>
    /// Représente un utilisateur dans le système avec ses informations d'authentification et ses relations
    /// </summary>
    /// <remarks>
    /// Cette entité est utilisée pour :
    /// - L'authentification et l'autorisation
    /// - La gestion des projets personnels
    /// - Le suivi des commentaires
    /// </remarks>
    public class User
    {
        /// <summary>
        /// Identifiant unique de l'utilisateur
        /// </summary>
        /// <example>1</example>
        [Key]
        [JsonIgnore]
        public int Id { get; set; }

        /// <summary>
        /// Nom de l'utilisateur
        /// </summary>
        /// <example>John Doe</example>
        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Email de l'utilisateur (unique)
        /// </summary>
        /// <example>john.doe@example.com</example>
        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        [StringLength(100, ErrorMessage = "L'email ne peut pas dépasser 100 caractères")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Hash du mot de passe de l'utilisateur (BCrypt)
        /// </summary>
        /// <remarks>Le mot de passe est haché avec BCrypt avant stockage</remarks>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Rôle de l'utilisateur (User ou Admin)
        /// </summary>
        /// <example>User</example>
        [Required]
        public Role Role { get; set; }

        /// <summary>
        /// Collection des projets appartenant à l'utilisateur
        /// </summary>
        /// <remarks>
        /// Navigation property pour Entity Framework
        /// Permet d'accéder aux projets de l'utilisateur
        /// </remarks>
        [JsonIgnore]
        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();


    }
    /// <summary>
    /// Énumération des rôles possibles pour un utilisateur
    /// </summary>
    public enum Role
    {
        User,
        Admin
    }
}
