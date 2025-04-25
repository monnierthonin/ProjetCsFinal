using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    /// <summary>
    /// Représente un utilisateur dans le système
    /// </summary>
    public class User
    {
        /// <summary>
        /// Identifiant unique de l'utilisateur
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nom d'utilisateur unique
        /// </summary>
        [Required]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Prénom de l'utilisateur
        /// </summary>
        [Required]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Nom de famille de l'utilisateur
        /// </summary>
        [Required]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Email de l'utilisateur (unique)
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Hash du mot de passe de l'utilisateur
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Rôle de l'utilisateur
        /// </summary>
        [Required]
        public UserRole Role { get; set; }

        /// <summary>
        /// Collection des projets appartenant à l'utilisateur
        /// </summary>
        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();

        /// <summary>
        /// Collection des commentaires de l'utilisateur
        /// </summary>
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }

    /// <summary>
    /// Énumération des rôles possibles pour un utilisateur
    /// </summary>
    public enum UserRole
    {
        User,
        Admin
    }
}
