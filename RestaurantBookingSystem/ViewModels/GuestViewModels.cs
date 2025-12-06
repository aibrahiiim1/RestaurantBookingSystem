using RestaurantBookingSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace RestaurantBookingSystem.ViewModels
{
    public class HomeViewModel
    {
        public List<RestaurantCardViewModel> FeaturedRestaurants { get; set; }
        public List<RestaurantReviewViewModel> RecentReviews { get; set; }
        public List<Cuisine> Cuisines { get; set; }
        public SearchViewModel Search { get; set; }
    }

    public class SearchViewModel
    {
        public string? Location { get; set; }
        public DateTime? Date { get; set; }
        public TimeOnly? Time { get; set; }
        public int PartySize { get; set; } = 2;
    }

    public class RestaurantSearchResultsViewModel
    {
        public List<RestaurantCardViewModel> Restaurants { get; set; }
        public SearchViewModel Search { get; set; }
        public string? SelectedCuisine { get; set; }
        public string? SelectedFilter { get; set; }
        public string SortBy { get; set; } = "featured";
        public int TotalResults { get; set; }
    }

    public class RestaurantCardViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CuisineName { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string PrimaryPhotoUrl { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public string PriceRange { get; set; }
        public bool IsChildFriendly { get; set; }
        public bool HasOutdoorSeating { get; set; }
        public double? Distance { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class RestaurantDetailViewModel
    {
        public Restaurant Restaurant { get; set; }
        public List<Attachment> Photos { get; set; }
        public List<RestaurantReviewViewModel> Reviews { get; set; }
        public List<OpeningTime> OpeningTimes { get; set; }
        public List<SpecialEvent> UpcomingEvents { get; set; }
        public List<Promotion> ActivePromotions { get; set; }
        public BookingFormViewModel BookingForm { get; set; }
        public List<TableLocation> AvailableTableLocations { get; set; }
    }

    public class BookingFormViewModel
    {
        public int RestaurantId { get; set; }
        
        [Required]
        public DateTime? Date { get; set; }
        
        [Required]
        public TimeOnly? Time { get; set; }
        
        [Required]
        [Range(1, 50)]
        public int PartySize { get; set; }
        
        public TableLocation PreferredLocation { get; set; }
    }

    public class AvailableTimeSlotsViewModel
    {
        public int RestaurantId { get; set; }
        public DateTime Date { get; set; }
        public int PartySize { get; set; }
        public List<TimeSlotViewModel> TimeSlots { get; set; }
    }

    public class TimeSlotViewModel
    {
        public TimeOnly Time { get; set; }
        public bool IsAvailable { get; set; }
        public int AvailableTables { get; set; }
        public string DisplayTime { get; set; }
    }

    //public class BookingConfirmationViewModel
    //{
    //    [Required]
    //    [EmailAddress]
    //    public string Email { get; set; }

    //    [Phone]
    //    public string PhoneNumber { get; set; }

    //    [Required]
    //    public string FirstName { get; set; }

    //    [Required]
    //    public string LastName { get; set; }

    //    public int? OccasionId { get; set; }

    //    [MaxLength(1000)]
    //    public string? SpecialRequests { get; set; }

    //    public bool ReceivePromotions { get; set; }

    //    // Hidden fields
    //    public int RestaurantId { get; set; }
    //    public DateTime Date { get; set; }
    //    public TimeOnly Time { get; set; }
    //    public int PartySize { get; set; }
    //    public TableLocation PreferredLocation { get; set; }
    //}

    public class VerificationViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string VerificationCode { get; set; }

        public string BookingReference { get; set; }
    }

    public class BookingSuccessViewModel
    {
        public Reservation Reservation { get; set; }
        public Restaurant Restaurant { get; set; }
        public Customer Customer { get; set; }
        public string MapUrl { get; set; }
    }

    public class MyReservationsViewModel
    {
        public List<ReservationViewModel> UpcomingReservations { get; set; }
        public List<ReservationViewModel> PastReservations { get; set; }
        public List<ReservationViewModel> CancelledReservations { get; set; }
    }

    public class ReservationViewModel
    {
        public int Id { get; set; }
        public string BookingReference { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantPhoto { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public int NumberOfGuests { get; set; }
        public string TableLocation { get; set; }
        public int TableNumber { get; set; }
        public ReservationStatus Status { get; set; }
        public string? OccasionName { get; set; }
        public string? SpecialRequests { get; set; }
        public bool CanCancel { get; set; }
        public bool CanModify { get; set; }
        public bool HasReview { get; set; }
        public Restaurant Restaurant { get; set; }
    }

    public class RestaurantReviewViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public int FoodRating { get; set; }
        public int ServiceRating { get; set; }
        public int AmbianceRating { get; set; }
        public DateTime ReviewDate { get; set; }
        public string CustomerName { get; set; }
        public string RestaurantName { get; set; }
        public bool IsVerified { get; set; }
    }

    public class WriteReviewViewModel
    {
        public int ReservationId { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        
        [Required]
        [MaxLength(150)]
        public string Title { get; set; }
        
        [MaxLength(2000)]
        public string Comment { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int FoodRating { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int ServiceRating { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int AmbianceRating { get; set; }
    }

    //public class CustomerLoginViewModel
    //{
    //    [Required]
    //    [EmailAddress]
    //    public string Email { get; set; }
        
    //    [Required]
    //    [DataType(DataType.Password)]
    //    public string Password { get; set; }
        
    //    public bool RememberMe { get; set; }
        
    //    public string? ReturnUrl { get; set; }
    //}

    //public class CustomerRegisterViewModel
    //{
    //    [Required]
    //    [MaxLength(40)]
    //    public string FirstName { get; set; }
        
    //    [Required]
    //    [MaxLength(50)]
    //    public string LastName { get; set; }
        
    //    [Required]
    //    [EmailAddress]
    //    public string Email { get; set; }
        
    //    [Phone]
    //    public string PhoneNumber { get; set; }
        
    //    [Required]
    //    [DataType(DataType.Password)]
    //    [MinLength(6)]
    //    public string Password { get; set; }
        
    //    [Required]
    //    [DataType(DataType.Password)]
    //    [Compare("Password")]
    //    public string ConfirmPassword { get; set; }
        
    //    public bool ReceivePromotions { get; set; }
    //}
}
