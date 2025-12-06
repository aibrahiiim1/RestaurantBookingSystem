using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantBookingSystem.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public int RestaurantId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public int? ReservationId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(100)]
        public string? Title { get; set; }

        [Required]
        [StringLength(2000)]
        public string Comment { get; set; } = string.Empty;

        // Specific ratings
        [Range(1, 5)]
        public int? FoodRating { get; set; }

        [Range(1, 5)]
        public int? ServiceRating { get; set; }

        [Range(1, 5)]
        public int? AmbianceRating { get; set; }

        [Range(1, 5)]
        public int? ValueRating { get; set; }

        // Restaurant response
        [StringLength(1000)]
        public string? RestaurantResponse { get; set; }

        public DateTime? ResponseDate { get; set; }

        // Metadata
        public bool IsVerifiedGuest { get; set; }
        public bool IsFlagged { get; set; }
        public string? FlagReason { get; set; }
        public int HelpfulCount { get; set; }
        public int NotHelpfulCount { get; set; }
        
        public ReviewStatus Status { get; set; } = ReviewStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(RestaurantId))]
        public virtual Restaurant? Restaurant { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer? Customer { get; set; }

        [ForeignKey(nameof(ReservationId))]
        public virtual Reservation? Reservation { get; set; }

        public virtual ICollection<ReviewPhoto> Photos { get; set; } = new List<ReviewPhoto>();
    }

    public class ReviewPhoto
    {
        [Key]
        public int PhotoId { get; set; }

        [Required]
        public int ReviewId { get; set; }

        [Required]
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ReviewId))]
        public virtual Review? Review { get; set; }
    }

    public class FavoriteRestaurant
    {
        [Key]
        public int FavoriteId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int RestaurantId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer? Customer { get; set; }

        [ForeignKey(nameof(RestaurantId))]
        public virtual Restaurant? Restaurant { get; set; }
    }
}
