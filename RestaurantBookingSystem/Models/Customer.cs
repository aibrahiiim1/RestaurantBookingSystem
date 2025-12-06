using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantBookingSystem.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; }

        // Basic Information
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

        // Authentication
        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsEmailVerified { get; set; } = false;
        [NotMapped]
        [StringLength(500)]
        public string? AvatarUrl { get; set; }

        // Security & Preferences
        public bool TwoFactorEnabled { get; set; } = false;
        public bool RequiresEmailVerification { get; set; } = true;
        
        // Notification Settings
        public bool EmailNotifications { get; set; } = true;
        public bool SmsNotifications { get; set; } = true;

        // User Preferences
        [StringLength(500)]
        public string? FavoriteCuisines { get; set; }

        [StringLength(500)]
        public string? DietaryRestrictions { get; set; }

        public int? DefaultPartySize { get; set; }

        // Activity Tracking
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Soft Delete
        public bool IsActive { get; set; } = true;
        public bool IsGuest { get; set; } = true;
        public bool ReceiveReminders { get; set; } = true;
        public bool ReceivePromotions { get; set; } = true;
        public DateTime? DeletedAt { get; set; }

        // Navigation Properties
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<FavoriteRestaurant> FavoriteRestaurants { get; set; } = new List<FavoriteRestaurant>();

        // Computed Properties
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [NotMapped]
        public int Age
        {
            get
            {
                if (!DateOfBirth.HasValue)
                    return 0;

                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Value.Year;
                if (DateOfBirth.Value.Date > today.AddYears(-age))
                    age--;
                return age;
            }
        }
    }
}
