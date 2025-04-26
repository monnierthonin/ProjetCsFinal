using System.ComponentModel.DataAnnotations;
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
        public int Id { get; set; }

        /// <summary>
        /// Nom d'utilisateur unique pour l'authentification
        /// </summary>
        /// <example>john.doe</example>
        [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Le nom d'utilisateur doit contenir entre 3 et 50 caractères")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Prénom de l'utilisateur
        /// </summary>
        /// <example>John</example>
        [Required(ErrorMessage = "Le prénom est requis")]
        [StringLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Nom de famille de l'utilisateur
        /// </summary>
        /// <example>Doe</example>
        [Required(ErrorMessage = "Le nom de famille est requis")]
        [StringLength(50, ErrorMessage = "Le nom de famille ne peut pas dépasser 50 caractères")]
        public string LastName { get; set; } = string.Empty;

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
        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Rôle de l'utilisateur (User ou Admin)
        /// </summary>
        /// <example>User</example>
        [Required]
        [EnumDataType(typeof(UserRole), ErrorMessage = "Rôle invalide")]
        public UserRole Role { get; set; }

        /// <summary>
        /// Collection des projets appartenant à l'utilisateur
        /// </summary>
        /// <remarks>
        /// Navigation property pour Entity Framework
        /// Permet d'accéder aux projets de l'utilisateur
        /// </remarks>
        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();

        /// <summary>
        /// Collection des commentaires de l'utilisateur
        /// </summary>
        /// <remarks>
        /// Navigation property pour Entity Framework
        /// Permet d'accéder aux commentaires de l'utilisateur
        /// </remarks>
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }

    /// <summary>
    /// Énumération des rôles possibles pour un utilisateur
    /// </summary>
    /// <remarks>
    /// - User : Utilisateur standard avec accès limité
    /// - Admin : Administrateur avec accès complet
    /// </remarks>
    public enum UserRole
    {
        User,
        Admin
    }
}
