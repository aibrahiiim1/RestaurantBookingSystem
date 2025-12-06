using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantBookingSystem.Data;
using RestaurantBookingSystem.Models;
using RestaurantBookingSystem.Services;
using RestaurantBookingSystem.ViewModels;
using System.Security.Claims;

namespace RestaurantBookingSystem.Controllers
{
    public class BookingController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly IRestaurantService _restaurantService;
        private readonly ICustomerService _customerService;
        private readonly IAuthenticationService _authService;
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _context;

        public BookingController(
            IReservationService reservationService,
            IRestaurantService restaurantService,
            ICustomerService customerService,
            IAuthenticationService authService,
            IEmailService emailService,
            ApplicationDbContext context)
        {
            _reservationService = reservationService;
            _restaurantService = restaurantService;
            _customerService = customerService;
            _authService = authService;
            _emailService = emailService;
            _context = context;
        }

        [HttpPost]
        public IActionResult InitiateBooking(BookingFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all required fields";
                return RedirectToAction("Detail", "Restaurant", new { id = model.RestaurantId });
            }

            // Store booking details in session
            HttpContext.Session.SetString("BookingData", System.Text.Json.JsonSerializer.Serialize(model));

            return RedirectToAction("Confirmation");
        }

        public async Task<IActionResult> Confirmation()
        {
            var bookingData = HttpContext.Session.GetString("BookingData");
            if (string.IsNullOrEmpty(bookingData))
            {
                return RedirectToAction("Index", "Home");
            }

            var bookingForm = System.Text.Json.JsonSerializer.Deserialize<BookingFormViewModel>(bookingData);
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(bookingForm.RestaurantId);

            var viewModel = new BookingFormViewModel
            {
                RestaurantId = bookingForm.RestaurantId,
                Date = bookingForm.Date.Value,
                Time = bookingForm.Time.Value,
                PartySize = bookingForm.PartySize,
                PreferredLocation = bookingForm.PreferredLocation
            };

            ViewBag.Restaurant = restaurant;
            ViewBag.Occasions = await _context.Occasions.ToListAsync();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Confirmation(BookingConfirmationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Restaurant = await _restaurantService.GetRestaurantByIdAsync(model.RestaurantId);
                ViewBag.Occasions = await _context.Occasions.ToListAsync();
                return View(model);
            }

            // Check if customer exists or create new one
            var customer = await _customerService.GetCustomerByEmailAsync(model.Email);

            if (customer == null)
            {
                customer = new Customer
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    IsGuest = true,
                    ReceivePromotions = model.ReceivePromotions
                };
                customer = await _customerService.CreateCustomerAsync(customer);
            }

            // Send verification code
            var verificationCode = await _authService.GenerateVerificationCodeAsync();
            await _emailService.SendVerificationCodeAsync(model.Email, verificationCode);

            // Store verification code and booking data in session
            HttpContext.Session.SetString("VerificationCode", verificationCode);
            HttpContext.Session.SetString("VerificationEmail", model.Email);
            HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);
            HttpContext.Session.SetString("FinalBookingData", System.Text.Json.JsonSerializer.Serialize(model));

            return RedirectToAction("Verify");
        }

        public IActionResult Verify()
        {
            var email = HttpContext.Session.GetString("VerificationEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new VerificationViewModel { Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> Verify(VerificationViewModel model)
        {
            var storedCode = HttpContext.Session.GetString("VerificationCode");
            var email = HttpContext.Session.GetString("VerificationEmail");

            if (storedCode != model.VerificationCode || email != model.VerificationCode)
            {
                ModelState.AddModelError("", "Invalid verification code");
                return View(model);
            }

            // Retrieve booking data
            var bookingData = HttpContext.Session.GetString("FinalBookingData");
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (string.IsNullOrEmpty(bookingData) || !customerId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }

            var bookingModel = System.Text.Json.JsonSerializer.Deserialize<BookingConfirmationViewModel>(bookingData);

            try
            {
                // Create reservation
                var reservation = await _reservationService.CreateReservationAsync(bookingModel, customerId.Value);

                // Send confirmation email
                //await _emailService.SendBookingConfirmationAsync(reservation);
                await _emailService.SendBookingConfirmationAsync(
    reservation.Customer.Email,
    reservation.BookingReference,
    reservation.ReservationDate,
    reservation.ReservationTime,
    reservation.Restaurant.Name
);
                // Clear session
                HttpContext.Session.Remove("BookingData");
                HttpContext.Session.Remove("FinalBookingData");
                HttpContext.Session.Remove("VerificationCode");
                HttpContext.Session.Remove("VerificationEmail");
                HttpContext.Session.Remove("CustomerId");

                return RedirectToAction("Success", new { reference = reservation.BookingReference });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> Success(string reference)
        {
            var reservation = await _reservationService.GetReservationByReferenceAsync(reference);

            if (reservation == null)
            {
                return NotFound();
            }

            var viewModel = new BookingSuccessViewModel
            {
                Reservation = reservation,
                Restaurant = reservation.Restaurant,
                Customer = reservation.Customer,
                MapUrl = $"https://www.google.com/maps/search/?api=1&query={reservation.Restaurant.Latitude},{reservation.Restaurant.Longitude}"
            };

            return View(viewModel);
        }

        public async Task<IActionResult> MyReservations()
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Login", "Customer");
            }

            var reservations = await _reservationService.GetCustomerReservationsAsync(int.Parse(customerId));

            var viewModel = new MyReservationsViewModel
            {
                UpcomingReservations = reservations
                    .Where(r => r.StartTime > DateTime.UtcNow && r.Status != ReservationStatus.Cancelled)
                    .Select(MapToReservationViewModel)
                    .ToList(),
                PastReservations = reservations
                    .Where(r => r.StartTime <= DateTime.UtcNow || r.Status == ReservationStatus.Cancelled)
                    .Select(MapToReservationViewModel)
                    .ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Login", "Customer");
            }

            var reservation = await _reservationService.GetReservationByIdAsync(id);

            if (reservation == null || reservation.CustomerId != int.Parse(customerId))
            {
                return NotFound();
            }

            var viewModel = MapToReservationViewModel(reservation);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id, string reason)
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerId))
            {
                return Unauthorized();
            }

            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null || reservation.CustomerId != int.Parse(customerId))
            {
                return NotFound();
            }

            var success = await _reservationService.CancelReservationAsync(id, reason);

            if (success)
            {
                //await _emailService.SendCancellationConfirmationAsync(reservation);
                await _emailService.SendCancellationConfirmationAsync(
    reservation.Customer.Email,
    reservation.BookingReference,
    reservation.Restaurant.Name
);
                TempData["Success"] = "Reservation cancelled successfully";
            }
            else
            {
                TempData["Error"] = "Unable to cancel reservation. Cancellation must be made at least " +
                    $"{reservation.Restaurant.CancellationPolicyHours} hours before the reservation time.";
            }

            return RedirectToAction("MyReservations");
        }

        private ReservationViewModel MapToReservationViewModel(Reservation reservation)
        {
            return new ReservationViewModel
            {
                Id = reservation.Id,
                BookingReference = reservation.BookingReference,
                RestaurantName = reservation.Restaurant.Name,
                RestaurantPhoto = reservation.Restaurant.Photos?.FirstOrDefault()?.Url ?? "/images/default-restaurant.jpg",
                ReservationDate = reservation.ReservationDate,
                ReservationTime = reservation.ReservationTime,
                NumberOfGuests = reservation.NumberOfGuests,
                Status = reservation.Status,
                OccasionName = reservation.Occasion?.Name,
                SpecialRequests = reservation.SpecialRequests,
                CanCancel = (reservation.StartTime - DateTime.UtcNow).TotalHours >= reservation.Restaurant.CancellationPolicyHours,
                CanModify = (reservation.StartTime - DateTime.UtcNow).TotalHours >= reservation.Restaurant.CancellationPolicyHours,
                Restaurant = reservation.Restaurant
            };
        }
    }
}
