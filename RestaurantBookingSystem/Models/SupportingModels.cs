using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantBookingSystem.Models
{
    public class Cuisine
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [MaxLength(500)]
        public string? IconUrl { get; set; }
        
        public ICollection<Restaurant> Restaurants { get; set; }
    }

    public class MealType
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } // Breakfast, Lunch, Dinner, Brunch
        
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }

    public class Occasion
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // Birthday, Anniversary, Business, Date Night, etc.
        
        [MaxLength(500)]
        public string? IconUrl { get; set; }
    }

    public class OpeningTime
    {
        public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }
        public bool IsClosed { get; set; }
        public int RestaurantId { get; set; }
        
        public Restaurant Restaurant { get; set; }
    }

    public class RestaurantClosure
    {
        public int Id { get; set; }
        
        [Required]
        public int RestaurantId { get; set; }
        
        [Required]
        public DateTime ClosureDate { get; set; }
        
        [MaxLength(200)]
        public string? Reason { get; set; }
        
        public Restaurant Restaurant { get; set; }
    }

    public class Attachment
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(300)]
        public string FileName { get; set; }
        
        [Required]
        [Url]
        [MaxLength(500)]
        public string Url { get; set; }
        
        public AttachmentType Type { get; set; }
        
        public bool IsPrimary { get; set; }
        
        public int DisplayOrder { get; set; }
        
        public int RestaurantId { get; set; }
        
        public Restaurant Restaurant { get; set; }
    }

    public enum AttachmentType
    {
        Photo = 1,
        Logo = 2,
        Menu = 3,
        Banner = 4
    }

    public class MenuItem
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        public decimal Price { get; set; }
        
        [MaxLength(50)]
        public string Category { get; set; } // Appetizer, Main Course, Dessert, etc.
        
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        
        public bool IsAvailable { get; set; }
        
        public int RestaurantId { get; set; }
        
        public Restaurant Restaurant { get; set; }
    }

    public class SpecialEvent
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(120)]
        public string EventName { get; set; }
        
        [MaxLength(2000)]
        public string? Description { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        
        public bool RequiresReservation { get; set; }
        
        public int RestaurantId { get; set; }
        
        public Restaurant Restaurant { get; set; }
    }

    public class Promotion
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        
        [MaxLength(1000)]
        public string Description { get; set; }
        
        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        [MaxLength(50)]
        public string? PromoCode { get; set; }
        
        public bool IsActive { get; set; }
        
        public int? RestaurantId { get; set; }
        
        public Restaurant? Restaurant { get; set; }
    }

    public class RestaurantReview
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(150)]
        public string Title { get; set; }
        
        [MaxLength(2000)]
        public string? Comment { get; set; }
        
        [Range(1, 5)]
        public int Rating { get; set; }
        
        [Range(1, 5)]
        public int FoodRating { get; set; }
        
        [Range(1, 5)]
        public int ServiceRating { get; set; }
        
        [Range(1, 5)]
        public int AmbianceRating { get; set; }
        
        public DateTime ReviewDate { get; set; }
        
        public bool IsVerified { get; set; } // Verified if customer had actual reservation
        
        public int RestaurantId { get; set; }
        public int CustomerId { get; set; }
        public int? ReservationId { get; set; }
        
        public Restaurant Restaurant { get; set; }
        public Customer Customer { get; set; }
        public Reservation? Reservation { get; set; }
    }

    public class CustomerAllergy
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string AllergyName { get; set; }
        
        [MaxLength(500)]
        public string? Notes { get; set; }
        
        public Customer Customer { get; set; }
    }

    //public class FavoriteRestaurant
    //{
    //    public int Id { get; set; }
    //    public int CustomerId { get; set; }
    //    public int RestaurantId { get; set; }
    //    public DateTime AddedDate { get; set; }
        
    //    public Customer Customer { get; set; }
    //    public Restaurant Restaurant { get; set; }
    //}

    public class ReservationMessage
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        
        [Required]
        [MaxLength(2000)]
        public string Message { get; set; }
        
        public bool IsFromCustomer { get; set; }
        
        public DateTime SentAt { get; set; }
        
        public bool IsRead { get; set; }
        
        public Reservation Reservation { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string PasswordHash { get; set; }
        
        [Required]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
        
        public int RestaurantId { get; set; }
        public int RoleId { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        
        //public Restaurant Restaurant { get; set; }
        //public Role Role { get; set; }
        public virtual Restaurant? Restaurant { get; set; }
        public virtual Role? Role { get; set; }
        public string? Permissions { get; set; }

    }

    public class Role
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; }
        
        public ICollection<User> Users { get; set; }
    }

    public class Principal
    {
        public int Id { get; set; }
        public PrincipalType Type { get; set; }
        public int? CustomerId { get; set; }
        public int? UserId { get; set; }
        
        public Customer? Customer { get; set; }
        public User? User { get; set; }
    }

    public enum PrincipalType
    {
        Customer = 1,
        User = 2,
        System = 3
    }

    public class Log
    {
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }
        
        [MaxLength(120)]
        public string OperationName { get; set; }
        
        public int RestaurantId { get; set; }
        public int CreatedByUserId { get; set; }
        
        [MaxLength(2000)]
        public string? Message { get; set; }
        
        public Restaurant Restaurant { get; set; }
        public User CreatedByUser { get; set; }
    }
}
