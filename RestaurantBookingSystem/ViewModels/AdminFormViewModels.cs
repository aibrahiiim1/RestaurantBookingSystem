using RestaurantBookingSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantBookingSystem.ViewModels.Admin
{
    // Table Form ViewModel (for Add/Edit Table)
    //public class TableFormViewModel
    //{
    //    public int? TableId { get; set; }
        
    //    [Required(ErrorMessage = "Table number is required")]
    //    public int TableNumber { get; set; }

    //    [Required(ErrorMessage = "Maximum capacity is required")]
    //    [Range(1, 20, ErrorMessage = "Capacity must be between 1 and 20")]
    //    public int MaxCapacity { get; set; }

    //    [Required(ErrorMessage = "Minimum capacity is required")]
    //    [Range(1, 20, ErrorMessage = "Capacity must be between 1 and 20")]
    //    public int MinCapacity { get; set; }

    //    public string? Shape { get; set; }
    //    public bool IsActive { get; set; } = true;
    //    public bool IsWindowSeat { get; set; }
    //    public bool IsAccessible { get; set; }
    //    public bool IsOutdoor { get; set; }
    //    public bool IsPrivate { get; set; }
    //    public int SeatingCapacity { get; set; }
    //    [Required(ErrorMessage = "Location is required")]

    //    public TableLocation Location { get; set; }

    //    public bool IsAvailable { get; set; } = true;

    //    public int RestaurantId { get; set; }
    //}

    // Restaurant Form ViewModel (for Add/Edit Restaurant)
    public class RestaurantFormViewModel
    {
        public int? RestaurantId { get; set; }

        [Required(ErrorMessage = "Restaurant name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cuisine type is required")]
        public string CuisineType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price range is required")]
        public string PriceRange { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Website { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string AddressLine1 { get; set; } = string.Empty;

        public string? AddressLine2 { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "Postal code is required")]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; } = string.Empty;

        public int Capacity { get; set; }
        public bool AcceptsOnlineBookings { get; set; } = true;

        public List<string> AvailableCuisines { get; set; } = new List<string>
    {
        "Italian", "Chinese", "Japanese", "Mexican", "Indian",
        "Thai", "French", "Mediterranean", "American", "Other"
    };
        public int Id { get; set; }


        [MaxLength(100)]
        public string? BrandName { get; set; } // For restaurant chains


        public string? LogoUrl { get; set; }



        public string Area { get; set; }


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
        public string? Features { get; set; }
        // Booking configuration
        public int DefaultBookingDurationMinutes { get; set; } = 120;
        public int TimeSlotIntervalMinutes { get; set; } = 30;
        public int CancellationPolicyHours { get; set; } = 5;

        public ICollection<OpeningTime> OpeningTimes { get; set; }
    }

    // Opening Time Form ViewModel
    public class OpeningTimeFormViewModel
    {
        [Required]
        public string DayOfWeek { get; set; } = string.Empty;

        public bool IsOpen { get; set; }

        [Required(ErrorMessage = "Opening time is required")]
        public TimeSpan? OpenTime { get; set; }

        [Required(ErrorMessage = "Closing time is required")]
        public TimeSpan? CloseTime { get; set; }

    }

    // Table Management ViewModel (alias for compatibility)
    public class TableManagementViewModel : TablesManagementViewModel
    {
        // This is just an alias to fix the naming inconsistency
    }
}
