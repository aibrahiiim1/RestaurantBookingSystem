using System.ComponentModel.DataAnnotations;

namespace RestaurantBookingSystem.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string FirstName { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        
        [Required]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; }
        
        [Phone]
        [MaxLength(30)]
        public string PhoneNumber { get; set; }
        
        [MaxLength(200)]
        public string? PasswordHash { get; set; } // For registered users
        
        public bool IsGuest { get; set; } // True for one-time bookings without registration
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime? LastLoginDate { get; set; }
        
        // Preferences
        public bool ReceivePromotions { get; set; }
        public bool ReceiveReminders { get; set; }
        
        // Loyalty
        public int LoyaltyPoints { get; set; }

        // Navigation properties
        public ICollection<Reservation> Reservations { get; set; }
        public ICollection<RestaurantReview> Reviews { get; set; }
        public ICollection<CustomerAllergy> Allergies { get; set; }
        public ICollection<FavoriteRestaurant> FavoriteRestaurants { get; set; }
    }
}
