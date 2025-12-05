using RestaurantBookingSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace RestaurantBookingSystem.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalReservationsToday { get; set; }
        public int TotalReservationsThisMonth { get; set; }
        public int PendingReservations { get; set; }
        public int UnreadMessages { get; set; }
        public List<ReservationViewModel> TodayReservations { get; set; }
        public List<ReservationViewModel> UpcomingReservations { get; set; }
        public Dictionary<string, int> ReservationsByStatus { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public Restaurant Restaurant { get; set; }
    }

    public class RestaurantFormViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(120)]
        public string Name { get; set; }
        
        [MaxLength(100)]
        public string? BrandName { get; set; }
        
        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }
        
        [Required]
        public int CuisineId { get; set; }
        
        [Required]
        [MaxLength(60)]
        public string AddressLine1 { get; set; }
        
        [MaxLength(60)]
        public string? AddressLine2 { get; set; }
        
        [Required]
        [MaxLength(168)]
        public string City { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Area { get; set; }
        
        [MaxLength(15)]
        public string? PostalCode { get; set; }
        
        [Required]
        [MaxLength(60)]
        public string Country { get; set; }
        
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Url]
        public string? WebsiteUrl { get; set; }
        
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        
        [Range(1, 1000)]
        public int TotalSeatingCapacity { get; set; }
        
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
        
        [Range(30, 240)]
        public int DefaultBookingDurationMinutes { get; set; } = 120;
        
        [Range(15, 60)]
        public int TimeSlotIntervalMinutes { get; set; } = 30;
        
        [Range(1, 48)]
        public int CancellationPolicyHours { get; set; } = 5;
        
        public List<OpeningTimeFormViewModel> OpeningTimes { get; set; } = new();
        public List<Cuisine> AvailableCuisines { get; set; } = new();
    }

    public class OpeningTimeFormViewModel
    {
        public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }
        public bool IsClosed { get; set; }
    }

    public class TableManagementViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public List<TableViewModel> Tables { get; set; }
        public TableFormViewModel NewTable { get; set; }
    }

    public class TableViewModel
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public int SeatingCapacity { get; set; }
        public TableLocation Location { get; set; }
        public bool IsAvailable { get; set; }
        public int CurrentReservations { get; set; }
    }

    public class TableFormViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [Range(1, 999)]
        public int TableNumber { get; set; }
        
        [Required]
        [Range(1, 50)]
        public int SeatingCapacity { get; set; }
        
        [Required]
        public TableLocation Location { get; set; }
        
        public bool IsAvailable { get; set; } = true;
        
        public int RestaurantId { get; set; }
    }

    public class ReservationManagementViewModel
    {
        public List<ReservationAdminViewModel> Reservations { get; set; }
        public DateTime? FilterDate { get; set; }
        public ReservationStatus? FilterStatus { get; set; }
        public string? SearchTerm { get; set; }
    }

    public class ReservationAdminViewModel
    {
        public int Id { get; set; }
        public string BookingReference { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeOnly ReservationTime { get; set; }
        public int NumberOfGuests { get; set; }
        public int TableNumber { get; set; }
        public TableLocation TableLocation { get; set; }
        public ReservationStatus Status { get; set; }
        public string? OccasionName { get; set; }
        public string? SpecialRequests { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UnreadMessages { get; set; }
    }

    public class ReservationDetailViewModel
    {
        public Reservation Reservation { get; set; }
        public Customer Customer { get; set; }
        public Table Table { get; set; }
        public List<ReservationMessage> Messages { get; set; }
        public List<CustomerAllergy> CustomerAllergies { get; set; }
        public bool CanModify { get; set; }
        public bool CanCancel { get; set; }
    }

    public class PhotoManagementViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public List<Attachment> Photos { get; set; }
        public string? LogoUrl { get; set; }
    }

    public class MenuManagementViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public Dictionary<string, List<MenuItem>> MenuByCategory { get; set; }
        public List<string> Categories { get; set; }
    }

    public class MenuItemFormViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        [Required]
        [Range(0.01, 10000)]
        public decimal Price { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Category { get; set; }
        
        [Url]
        public string? ImageUrl { get; set; }
        
        public bool IsAvailable { get; set; } = true;
        
        public int RestaurantId { get; set; }
    }

    public class PromotionManagementViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public List<Promotion> ActivePromotions { get; set; }
        public List<Promotion> UpcomingPromotions { get; set; }
        public List<Promotion> ExpiredPromotions { get; set; }
    }

    public class PromotionFormViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        
        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }
        
        [Required]
        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        [MaxLength(50)]
        public string? PromoCode { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public int RestaurantId { get; set; }
    }

    public class SpecialEventManagementViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public List<SpecialEvent> UpcomingEvents { get; set; }
        public List<SpecialEvent> PastEvents { get; set; }
    }

    public class SpecialEventFormViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(120)]
        public string EventName { get; set; }
        
        [MaxLength(2000)]
        public string? Description { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        [Url]
        public string? ImageUrl { get; set; }
        
        public bool RequiresReservation { get; set; }
        
        public int RestaurantId { get; set; }
    }

    public class ReviewManagementViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public List<RestaurantReviewViewModel> Reviews { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; }
    }

    public class ClosureManagementViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public List<RestaurantClosure> FutureClosures { get; set; }
        public List<RestaurantClosure> PastClosures { get; set; }
    }

    public class ClosureFormViewModel
    {
        public int Id { get; set; }
        
        [Required]
        public DateTime ClosureDate { get; set; }
        
        [MaxLength(100)]
        public string? Reason { get; set; }
        
        public int RestaurantId { get; set; }
    }

    public class StaffManagementViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public List<User> Staff { get; set; }
        public List<Role> AvailableRoles { get; set; }
    }

    public class StaffFormViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
        
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        
        [Required]
        public int RoleId { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public int RestaurantId { get; set; }
    }

    public class MessagesViewModel
    {
        public int RestaurantId { get; set; }
        public List<ReservationWithMessages> ReservationsWithMessages { get; set; }
        public int UnreadCount { get; set; }
    }

    public class ReservationWithMessages
    {
        public Reservation Reservation { get; set; }
        public Customer Customer { get; set; }
        public List<ReservationMessage> Messages { get; set; }
        public int UnreadCount { get; set; }
    }

    public class AdminLoginViewModel
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        public bool RememberMe { get; set; }
    }

    public class AnalyticsViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        
        public int TotalReservationsThisMonth { get; set; }
        public int TotalReservationsLastMonth { get; set; }
        public int TotalCustomers { get; set; }
        public int NewCustomersThisMonth { get; set; }
        
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        
        public Dictionary<string, int> ReservationsByDay { get; set; }
        public Dictionary<string, int> ReservationsByStatus { get; set; }
        public Dictionary<int, int> ReservationsByPartySize { get; set; }
        public Dictionary<string, int> PopularTimeSlots { get; set; }
        
        public List<TopCustomer> TopCustomers { get; set; }
        public int NoShowRate { get; set; }
        public int CancellationRate { get; set; }
    }

    public class TopCustomer
    {
        public string Name { get; set; }
        public int TotalReservations { get; set; }
        public int TotalGuests { get; set; }
    }
}
