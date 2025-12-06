using System.ComponentModel.DataAnnotations;

namespace RestaurantBookingSystem.ViewModels
{

        public class BookingConfirmationViewModel
        {
            public int RestaurantId { get; set; }
            public string RestaurantName { get; set; } = string.Empty;
            public string RestaurantImage { get; set; } = string.Empty;
            public string RestaurantAddress { get; set; } = string.Empty;

            // Date/Time properties - supporting both naming conventions
            public DateTime Date { get; set; }
            public TimeOnly Time { get; set; }
            public int PartySize { get; set; }
        public int? OccasionId { get; set; }

        public string? PreferredLocation { get; set; }

            public DateTime ReservationDate { get; set; }
            public TimeSpan ReservationTime { get; set; }
            public int NumberOfGuests { get; set; }

            [Required(ErrorMessage = "First name is required")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Last name is required")]
            public string LastName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Phone number is required")]
            [Phone(ErrorMessage = "Invalid phone number")]
            public string PhoneNumber { get; set; } = string.Empty;

            public string? SpecialRequests { get; set; }
            public string? Occasion { get; set; }

            // ⭐ NEW - Added for line 99
            public bool ReceivePromotions { get; set; } = false;

            public bool IsAuthenticated { get; set; }
            public int? CustomerId { get; set; }
        }
    }
