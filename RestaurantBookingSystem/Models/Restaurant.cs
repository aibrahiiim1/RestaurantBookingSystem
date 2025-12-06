using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantBookingSystem.Models
{
    public class Restaurant
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(120)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string? BrandName { get; set; } // For restaurant chains

        [MaxLength(2000)]
        public string Description { get; set; }

        [MaxLength(500)]
        public string? LogoUrl { get; set; }

        [MaxLength(60)]
        public string AddressLine1 { get; set; }
        
        [MaxLength(60)]
        public string? AddressLine2 { get; set; }

        [MaxLength(168)]
        public string City { get; set; }

        [MaxLength(100)]
        public string Area { get; set; }

        [MaxLength(15)]
        public string? PostalCode { get; set; }

        [MaxLength(60)]
        public string Country { get; set; }

        [Phone]
        [MaxLength(30)]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; }

        [MaxLength(500)]
        public string? WebsiteUrl { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public int CuisineId { get; set; }

        public int TotalSeatingCapacity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? MinimumCharge { get; set; }

        [MaxLength(500)]
        public string? ParkingDetails { get; set; }

        [MaxLength(100)]
        public string? DressCode { get; set; }

        [MaxLength(500)]
        public string? PaymentOptions { get; set; }

        public bool IsChildFriendly { get; set; }
        public bool HasWheelchairAccess { get; set; }
        public bool HasBarArea { get; set; }
        public bool HasOutdoorSeating { get; set; }
        public bool HasTerraceSeating { get; set; }
        public bool HasBeachView { get; set; }

        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }

        [Range(0, 5)]
        [Column(TypeName = "decimal(2,1)")]
        public decimal AverageRating { get; set; }

        public int TotalReviews { get; set; }

        public DateTime CreatedDate { get; set; }

        // Booking configuration
        public int DefaultBookingDurationMinutes { get; set; } = 120;
        public int TimeSlotIntervalMinutes { get; set; } = 30;
        public int CancellationPolicyHours { get; set; } = 5;

        // Navigation properties
        public Cuisine Cuisine { get; set; }
        public ICollection<Table> Tables { get; set; }
        public ICollection<OpeningTime> OpeningTimes { get; set; }
        public ICollection<RestaurantClosure> Closures { get; set; }
        public ICollection<Attachment> Photos { get; set; }
        public ICollection<RestaurantReview> Reviews { get; set; }
        public ICollection<User> Staff { get; set; }
        public ICollection<SpecialEvent> SpecialEvents { get; set; }
        public ICollection<MenuItem> MenuItems { get; set; }
        public ICollection<Promotion> Promotions { get; set; }
        //public ICollection<string> AvailableCuisines { get; set; }
        //public List<string> AvailableCuisines { get; set; } = new List<string>();

        //        public List<string> AvailableCuisines { get; set; } = new List<string>
        //{ "Italian", "Chinese", "Japanese", "Mexican", "Indian", "Thai", "French", "Mediterranean", "American", "Other" };
    }
}
