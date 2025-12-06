using RestaurantBookingSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace RestaurantBookingSystem.ViewModels
{
    // Admin Login
    public class AdminLoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;  // ⭐ ADDED
        //[Required(ErrorMessage = "Email is required")]
        //[EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    // Admin Dashboard - ⭐ FIXED: Changed List<Reservation> to List<ReservationViewModel>
    //public class AdminDashboardViewModel
    //{
    //    // Statistics
    //    public int TotalReservations { get; set; }
    //    public int TodaysReservations { get; set; }
    //    public int UpcomingReservations { get; set; }
    //    public int PendingReviews { get; set; }
    //    public int TotalCustomers { get; set; }
    //    public int NewCustomersThisMonth { get; set; }

    //    // Revenue
    //    public decimal TodaysRevenue { get; set; }
    //    public decimal MonthlyRevenue { get; set; }
    //    public decimal YearlyRevenue { get; set; }

    //    // Lists - ⭐ FIXED: Use ReservationViewModel instead of Reservation
    //    public List<ReservationViewModel> RecentReservations { get; set; } = new List<ReservationViewModel>();
    //    public List<ReservationViewModel> TodaysReservationsList { get; set; } = new List<ReservationViewModel>();
    //    public List<ReservationViewModel> UpcomingReservationsList { get; set; } = new List<ReservationViewModel>();

    //    // Charts Data
    //    public Dictionary<string, int> ReservationsByStatus { get; set; } = new Dictionary<string, int>();
    //    public Dictionary<string, decimal> RevenueByMonth { get; set; } = new Dictionary<string, decimal>();
    //    public List<int> Last7DaysReservations { get; set; } = new List<int>();
    //    public List<string> Last7DaysLabels { get; set; } = new List<string>();

    //    // Top Items
    //    public List<TopRestaurantViewModel> TopRestaurants { get; set; } = new List<TopRestaurantViewModel>();
    //    public List<PopularTimeSlot> PopularTimeSlots { get; set; } = new List<PopularTimeSlot>();
    //}
    // Admin Dashboard - ⭐ FIXED: Added all missing properties
    public class AdminDashboardViewModel
    {
        // Restaurant Info - ⭐ ADDED
        public Restaurant? Restaurant { get; set; }  // Line 111

        // Statistics
        public int TotalReservations { get; set; }
        public int TotalReservationsToday { get; set; }  // ⭐ ADDED - Line 112
        public int TotalReservationsThisMonth { get; set; }  // ⭐ ADDED - Line 113
        public int TodaysReservations { get; set; }
        //public int UpcomingReservations { get; set; }
        public List<Reservation> UpcomingReservations { get; set; }
        public int PendingReservations { get; set; }  // ⭐ ADDED - Line 118
        public int PendingReviews { get; set; }
        public int TotalCustomers { get; set; }
        public int NewCustomersThisMonth { get; set; }
        public int TotalReviews { get; set; }  // ⭐ ADDED - Line 148
        public int UnreadMessages { get; set; }  // ⭐ ADDED - Line 121

        // Ratings - ⭐ ADDED
        public decimal AverageRating { get; set; }  // Line 145

        // Revenue
        public decimal TodaysRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal YearlyRevenue { get; set; }

        // Lists - Using ReservationViewModel (already fixed)
        public List<ReservationViewModel> RecentReservations { get; set; } = new List<ReservationViewModel>();
        public List<ReservationViewModel> TodayReservations { get; set; } = new List<ReservationViewModel>();  // ⭐ ADDED - Line 126
        public List<ReservationViewModel> TodaysReservationsList { get; set; } = new List<ReservationViewModel>();
        public List<ReservationViewModel> UpcomingReservationsList { get; set; } = new List<ReservationViewModel>();

        // Charts Data
        public Dictionary<string, int> ReservationsByStatus { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> RevenueByMonth { get; set; } = new Dictionary<string, decimal>();
        public List<int> Last7DaysReservations { get; set; } = new List<int>();
        public List<string> Last7DaysLabels { get; set; } = new List<string>();

        // Top Items
        public List<TopRestaurantViewModel> TopRestaurants { get; set; } = new List<TopRestaurantViewModel>();
        public List<PopularTimeSlot> PopularTimeSlots { get; set; } = new List<PopularTimeSlot>();
    }


    public class TopRestaurantViewModel
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ReservationCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal AverageRating { get; set; }
    }

    public class PopularTimeSlot
    {
        public TimeSpan Time { get; set; }
        public int BookingCount { get; set; }
        public string TimeDisplay { get; set; } = string.Empty;
    }

    // Reservations Management
    public class ReservationsListViewModel
    {
        public List<ReservationListItemViewModel> Reservations { get; set; } = new List<ReservationListItemViewModel>();
        public List<ReservationListItemViewModel> TodayReservations { get; set; } = new List<ReservationListItemViewModel>();
        public List<ReservationListItemViewModel> ThisWeekReservations { get; set; } = new List<ReservationListItemViewModel>();
        public List<ReservationListItemViewModel> PendingReservations { get; set; } = new List<ReservationListItemViewModel>();
        public List<ReservationListItemViewModel> ConfirmedReservations { get; set; } = new List<ReservationListItemViewModel>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public string? SearchTerm { get; set; }
        public ReservationStatus? StatusFilter { get; set; }
        public DateTime? DateFilter { get; set; }

    }

    public class ReservationListItemViewModel
    {
        public int ReservationId { get; set; }
        public string BookingReference { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string RestaurantName { get; set; } = string.Empty;
        public DateTime ReservationDate { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public int NumberOfGuests { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? SpecialRequests { get; set; }
        public string CustomerPhone { get; set; } = string.Empty;
        public int? TableNumber { get; set; }
        public string? Occasion { get; set; }
    }

    // Reviews Management
    public class ReviewsManagementViewModel
    {
        public List<ReviewItemViewModel> Reviews { get; set; } = new List<ReviewItemViewModel>();
        public int TotalCount { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int PendingCount { get; set; }
        public int FlaggedCount { get; set; }
        public ReviewStatus? StatusFilter { get; set; }
        public string? SearchTerm { get; set; }
        // ⭐ NEW - Rating distribution for the bar chart
        public Dictionary<int, int> RatingDistribution { get; set; } = new Dictionary<int, int>();
    }

    public class ReviewItemViewModel
    {
        public int ReviewId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string RestaurantName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Title { get; set; }
        public string Comment { get; set; } = string.Empty;
        public ReviewStatus Status { get; set; }
        public bool IsFlagged { get; set; }
        public string? FlagReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ReviewPhotoViewModel> Photos { get; set; } = new List<ReviewPhotoViewModel>();
        public bool IsVerifiedGuest { get; set; }
        public int HelpfulCount { get; set; }
        // ⭐ NEW - Aspect ratings (nullable so .HasValue works)
        public int? FoodRating { get; set; }
        public int? ServiceRating { get; set; }
        public int? AmbianceRating { get; set; }

        // ⭐ NEW - Restaurant response
        public string? RestaurantResponse { get; set; }
        public DateTime? ResponseDate { get; set; }
    }

    public class ReviewPhotoViewModel
    {
        public int PhotoId { get; set; }
        public string Url { get; set; } = string.Empty;
    }

    // Menu Management
    public class MenuManagementViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public List<MenuCategoryViewModel> Categories { get; set; } = new List<MenuCategoryViewModel>();
        public List<MenuItemViewModel> MenuItems { get; set; } = new List<MenuItemViewModel>();
    }

    public class MenuCategoryViewModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }

    public class MenuItemViewModel
    {
        public int ItemId { get; set; }
        public int MenuItemId { get; set; }
        public string CategoryName { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public bool IsPopular { get; set; }
        public bool IsSpicy { get; set; }
        public string? ImageUrl { get; set; }
        public List<string> Allergens { get; set; } = new List<string>();
        public bool IsVegetarian { get; set; }
        public bool IsVegan { get; set; }
        public bool IsGlutenFree { get; set; }
        public TimeSpan? PreparationTime { get; set; }

    }

    // Photo Gallery Management
    public class PhotoGalleryViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public List<PhotoItemViewModel> Photos { get; set; } = new List<PhotoItemViewModel>();
    }

    public class PhotoItemViewModel
    {
        public int PhotoId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public string? Category { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    // Restaurant Settings
    public class RestaurantSettingsViewModel
    {
        public int RestaurantId { get; set; }

        // Basic Information
        [Required(ErrorMessage = "Restaurant name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Cuisine type is required")]
        [StringLength(50)]
        public string? CuisineType { get; set; }

        [StringLength(20)]
        public string? PriceRange { get; set; }

        // Contact Information
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Url(ErrorMessage = "Invalid URL")]
        [StringLength(200)]
        public string? Website { get; set; }

        // Address
        [Required(ErrorMessage = "Address is required")]
        [StringLength(200)]
        public string AddressLine1 { get; set; } = string.Empty;

        [StringLength(200)]
        public string? AddressLine2 { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State/Province is required")]
        [StringLength(100)]
        public string Area { get; set; } = string.Empty;

        [Required(ErrorMessage = "Postal code is required")]
        [StringLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country is required")]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        // Booking Settings
        [Required(ErrorMessage = "Seating capacity is required")]
        [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000")]
        public int TotalSeatingCapacity { get; set; }

        [Required(ErrorMessage = "Booking duration is required")]
        [Range(30, 300, ErrorMessage = "Duration must be between 30 and 300 minutes")]
        public int DefaultBookingDurationMinutes { get; set; } = 120;

        [Required(ErrorMessage = "Max guests is required")]
        [Range(1, 100, ErrorMessage = "Max guests must be between 1 and 100")]
        public int MaxGuestsPerReservation { get; set; } = 12;

        [Required(ErrorMessage = "Min guests is required")]
        [Range(1, 50, ErrorMessage = "Min guests must be between 1 and 50")]
        public int MinGuestsPerReservation { get; set; } = 1;

        [Required(ErrorMessage = "Advance booking days is required")]
        [Range(1, 365, ErrorMessage = "Must be between 1 and 365 days")]
        public int MaxAdvanceBookingDays { get; set; } = 90;

        [Required(ErrorMessage = "Cancellation policy is required")]
        [Range(0, 168, ErrorMessage = "Must be between 0 and 168 hours")]
        public int CancellationPolicyHours { get; set; } = 24;

        public bool AcceptsOnlineBookings { get; set; } = true;
        public bool RequiresEmailVerification { get; set; } = true;

        // Features & Amenities
        public List<string> Features { get; set; } = new List<string>();

        // Opening Hours
        public List<OpeningHoursViewModel> OpeningHours { get; set; } = new List<OpeningHoursViewModel>();

        // For backward compatibility (map to TotalSeatingCapacity)
        public int Capacity
        {
            get => TotalSeatingCapacity;
            set => TotalSeatingCapacity = value;
        }

        // Address for backward compatibility (map to AddressLine1)
        public string Address
        {
            get => AddressLine1;
            set => AddressLine1 = value;
        }

        // State for backward compatibility (map to Area)
        public string State
        {
            get => Area;
            set => Area = value;
        }
    }

    public class OpeningHoursViewModel
    {
        public int OpeningHoursId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public bool IsOpen { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
    }

    // Tables Management
    public class TablesManagementViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public List<TableViewModel> Tables { get; set; } = new List<TableViewModel>();
        public TableFormViewModel NewTable { get; set; } = new TableFormViewModel();
    }

    public class TableViewModel
    {
        public int TableId { get; set; }
        public int NewTable { get; set; }
        public int Id { get; set; } // Alias for TableId
        public int TableNumber { get; set; }
        public int MaxCapacity { get; set; }
        public int SeatingCapacity { get; set; } // Alias for MaxCapacity
        public int MinCapacity { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? Shape { get; set; }
        public bool IsActive { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsWindowSeat { get; set; }
        public bool IsAccessible { get; set; }
        public bool IsOutdoor { get; set; }
        public bool IsPrivate { get; set; }
        public List<ReservationListItemViewModel> CurrentReservations { get; set; } = new List<ReservationListItemViewModel>();
    }

    // Table Form (in AdminFormViewModels.cs but including here for completeness)
    public class TableFormViewModel
    {
        public int TableId { get; set; }


        [Required]
        [Range(1, 999)]
        public int TableNumber { get; set; }

        [Required]
        [Range(1, 20)]
        public int MaxCapacity { get; set; }

        [Required]
        [Range(1, 20)]
        public int MinCapacity { get; set; }

      

        [StringLength(50)]
        public string? Shape { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsWindowSeat { get; set; }
        public bool IsAccessible { get; set; }
        public bool IsOutdoor { get; set; }
        public bool IsPrivate { get; set; }
        public int SeatingCapacity { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int RestaurantId { get; set; }
        [Required]
        [StringLength(50)]
        public TableLocation Location { get; set; }
    }
}