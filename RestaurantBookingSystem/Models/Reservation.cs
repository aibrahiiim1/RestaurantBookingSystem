using System.ComponentModel.DataAnnotations;

namespace RestaurantBookingSystem.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        
        [Required]
        public string BookingReference { get; set; } // e.g., "RES-20240101-1234"
        
        public int CustomerId { get; set; }
        
        public int RestaurantId { get; set; }
        public int ReservationId { get; set; }
        
        public int TableId { get; set; }
        
        public DateTime ReservationDate { get; set; }
        
        public TimeSpan ReservationTime { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }
        
        [Range(1, 100)]
        public int NumberOfGuests { get; set; }
        
        public ReservationStatus Status { get; set; }
        
        public int? MealTypeId { get; set; }
        
        public int? OccasionId { get; set; }
        
        [MaxLength(1000)]
        public string? SpecialRequests { get; set; }
        //public string? Occasion { get; set; }

        [MaxLength(500)]
        public string? CancellationReason { get; set; }
        
        public TableLocation PreferredTableLocation { get; set; }
        
        public int? PromotionId { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public int CreatedByPrincipalId { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public DateTime? CancelledAt { get; set; }

        // Navigation properties
        public Customer Customer { get; set; }
        public Restaurant Restaurant { get; set; }
        public Table Table { get; set; }
        public MealType? MealType { get; set; }
        public Occasion? Occasion { get; set; }
        public Promotion? Promotion { get; set; }
        public Principal CreatedByPrincipal { get; set; }
        public ICollection<ReservationMessage> Messages { get; set; }
    }

    //public enum ReservationStatus
    //{
    //    Pending = 1,
    //    Confirmed = 2,
    //    Cancelled = 3,
    //    NoShow = 4,
    //    Completed = 5,
    //    Seated = 6
    //}
}
