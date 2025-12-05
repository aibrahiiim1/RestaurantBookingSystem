using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RestaurantBookingSystem.Data;
using RestaurantBookingSystem.Models;
using RestaurantBookingSystem.Services;
using RestaurantBookingSystem.ViewModels.Admin;
using System.Security.Claims;
using RestaurantBookingSystem.ViewModels;
using IAuthenticationService = RestaurantBookingSystem.Services.IAuthenticationService;
using Microsoft.EntityFrameworkCore;

namespace RestaurantBookingSystem.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminAuth")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRestaurantService _restaurantService;
        private readonly IReservationService _reservationService;
        private readonly IAuthenticationService _authService;

        public AdminController(
            ApplicationDbContext context,
            IRestaurantService restaurantService,
            IReservationService reservationService,
            IAuthenticationService authService)
        {
            _context = context;
            _restaurantService = restaurantService;
            _reservationService = reservationService;
            _authService = authService;
        }

        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _authService.AuthenticateUserAsync(model.Username, model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("RestaurantId", user.RestaurantId.ToString()),
                new Claim(ClaimTypes.Role, user.Role.RoleName)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "AdminAuth");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync("AdminAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminAuth");
            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> Dashboard()
        {
            var restaurantId = GetRestaurantId();
            var today = DateTime.Today;

            var todayReservations = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Table)
                .Where(r => r.RestaurantId == restaurantId && r.ReservationDate.Date == today)
                .OrderBy(r => r.ReservationTime)
                .ToListAsync();

            var viewModel = new AdminDashboardViewModel
            {
                Restaurant = await _restaurantService.GetRestaurantByIdAsync(restaurantId),
                TotalReservationsToday = todayReservations.Count,
                TotalReservationsThisMonth = await _context.Reservations
                    .Where(r => r.RestaurantId == restaurantId 
                        && r.ReservationDate.Month == today.Month 
                        && r.ReservationDate.Year == today.Year)
                    .CountAsync(),
                PendingReservations = await _context.Reservations
                    .Where(r => r.RestaurantId == restaurantId && r.Status == ReservationStatus.Pending)
                    .CountAsync(),
                UnreadMessages = await _context.ReservationMessages
                    .Where(m => m.Reservation.RestaurantId == restaurantId 
                        && !m.IsRead 
                        && m.IsFromCustomer)
                    .CountAsync(),
                TodayReservations = MapReservationsToViewModel(todayReservations),
                UpcomingReservations = MapReservationsToViewModel(
                    await _context.Reservations
                        .Include(r => r.Customer)
                        .Include(r => r.Table)
                        .Where(r => r.RestaurantId == restaurantId 
                            && r.ReservationDate.Date > today 
                            && r.Status != ReservationStatus.Cancelled)
                        .OrderBy(r => r.ReservationDate)
                        .ThenBy(r => r.ReservationTime)
                        .Take(10)
                        .ToListAsync()
                ),
                ReservationsByStatus = await _context.Reservations
                    .Where(r => r.RestaurantId == restaurantId 
                        && r.ReservationDate.Month == today.Month)
                    .GroupBy(r => r.Status)
                    .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                    .ToDictionaryAsync(x => x.Status, x => x.Count),
                AverageRating = await _context.RestaurantReviews
                    .Where(r => r.RestaurantId == restaurantId)
                    .AverageAsync(r => (decimal?)r.Rating) ?? 0,
                TotalReviews = await _context.RestaurantReviews
                    .Where(r => r.RestaurantId == restaurantId)
                    .CountAsync()
            };

            return View(viewModel);
        }

        // Restaurant Management
        public async Task<IActionResult> RestaurantSettings()
        {
            var restaurantId = GetRestaurantId();
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(restaurantId);

            var viewModel = new RestaurantFormViewModel
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                BrandName = restaurant.BrandName,
                Description = restaurant.Description,
                CuisineId = restaurant.CuisineId,
                AddressLine1 = restaurant.AddressLine1,
                AddressLine2 = restaurant.AddressLine2,
                City = restaurant.City,
                Area = restaurant.Area,
                PostalCode = restaurant.PostalCode,
                Country = restaurant.Country,
                PhoneNumber = restaurant.PhoneNumber,
                Email = restaurant.Email,
                WebsiteUrl = restaurant.WebsiteUrl,
                Latitude = restaurant.Latitude,
                Longitude = restaurant.Longitude,
                TotalSeatingCapacity = restaurant.TotalSeatingCapacity,
                MinimumCharge = restaurant.MinimumCharge,
                ParkingDetails = restaurant.ParkingDetails,
                DressCode = restaurant.DressCode,
                PaymentOptions = restaurant.PaymentOptions,
                IsChildFriendly = restaurant.IsChildFriendly,
                HasWheelchairAccess = restaurant.HasWheelchairAccess,
                HasBarArea = restaurant.HasBarArea,
                HasOutdoorSeating = restaurant.HasOutdoorSeating,
                HasTerraceSeating = restaurant.HasTerraceSeating,
                HasBeachView = restaurant.HasBeachView,
                IsActive = restaurant.IsActive,
                IsFeatured = restaurant.IsFeatured,
                DefaultBookingDurationMinutes = restaurant.DefaultBookingDurationMinutes,
                TimeSlotIntervalMinutes = restaurant.TimeSlotIntervalMinutes,
                CancellationPolicyHours = restaurant.CancellationPolicyHours,
                OpeningTimes = restaurant.OpeningTimes.Select(ot => new OpeningTimeFormViewModel
                {
                    Id = ot.Id,
                    DayOfWeek = ot.DayOfWeek,
                    OpenTime = ot.OpenTime,
                    CloseTime = ot.CloseTime,
                    IsClosed = ot.IsClosed
                }).ToList(),
                AvailableCuisines = await _restaurantService.GetAllCuisinesAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestaurantSettings(RestaurantFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableCuisines = await _restaurantService.GetAllCuisinesAsync();
                return View(model);
            }

            var restaurant = await _restaurantService.GetRestaurantByIdAsync(model.Id);
            if (restaurant == null)
            {
                return NotFound();
            }

            // Update restaurant properties
            restaurant.Name = model.Name;
            restaurant.BrandName = model.BrandName;
            restaurant.Description = model.Description;
            restaurant.CuisineId = model.CuisineId;
            restaurant.AddressLine1 = model.AddressLine1;
            restaurant.AddressLine2 = model.AddressLine2;
            restaurant.City = model.City;
            restaurant.Area = model.Area;
            restaurant.PostalCode = model.PostalCode;
            restaurant.Country = model.Country;
            restaurant.PhoneNumber = model.PhoneNumber;
            restaurant.Email = model.Email;
            restaurant.WebsiteUrl = model.WebsiteUrl;
            restaurant.Latitude = model.Latitude;
            restaurant.Longitude = model.Longitude;
            restaurant.TotalSeatingCapacity = model.TotalSeatingCapacity;
            restaurant.MinimumCharge = model.MinimumCharge;
            restaurant.ParkingDetails = model.ParkingDetails;
            restaurant.DressCode = model.DressCode;
            restaurant.PaymentOptions = model.PaymentOptions;
            restaurant.IsChildFriendly = model.IsChildFriendly;
            restaurant.HasWheelchairAccess = model.HasWheelchairAccess;
            restaurant.HasBarArea = model.HasBarArea;
            restaurant.HasOutdoorSeating = model.HasOutdoorSeating;
            restaurant.HasTerraceSeating = model.HasTerraceSeating;
            restaurant.HasBeachView = model.HasBeachView;
            restaurant.IsActive = model.IsActive;
            restaurant.IsFeatured = model.IsFeatured;
            restaurant.DefaultBookingDurationMinutes = model.DefaultBookingDurationMinutes;
            restaurant.TimeSlotIntervalMinutes = model.TimeSlotIntervalMinutes;
            restaurant.CancellationPolicyHours = model.CancellationPolicyHours;

            await _restaurantService.UpdateRestaurantAsync(restaurant);

            TempData["Success"] = "Restaurant settings updated successfully";
            return RedirectToAction(nameof(RestaurantSettings));
        }

        // Table Management
        public async Task<IActionResult> Tables()
        {
            var restaurantId = GetRestaurantId();
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(restaurantId);

            var tables = await _context.Tables
                .Where(t => t.RestaurantId == restaurantId)
                .OrderBy(t => t.TableNumber)
                .ToListAsync();

            var viewModel = new TableManagementViewModel
            {
                RestaurantId = restaurantId,
                RestaurantName = restaurant.Name,
                Tables = tables.Select(t => new TableViewModel
                {
                    Id = t.Id,
                    TableNumber = t.TableNumber,
                    SeatingCapacity = t.SeatingCapacity,
                    Location = t.Location,
                    IsAvailable = t.IsAvailable,
                    CurrentReservations = _context.Reservations
                        .Count(r => r.TableId == t.Id 
                            && r.ReservationDate.Date >= DateTime.Today 
                            && r.Status != ReservationStatus.Cancelled)
                }).ToList(),
                NewTable = new TableFormViewModel { RestaurantId = restaurantId }
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTable(TableFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid table data";
                return RedirectToAction(nameof(Tables));
            }

            var table = new Table
            {
                TableNumber = model.TableNumber,
                SeatingCapacity = model.SeatingCapacity,
                Location = model.Location,
                IsAvailable = model.IsAvailable,
                RestaurantId = model.RestaurantId
            };

            _context.Tables.Add(table);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Table added successfully";
            return RedirectToAction(nameof(Tables));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTable(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table == null || table.RestaurantId != GetRestaurantId())
            {
                return NotFound();
            }

            // Check if table has any active reservations
            var hasActiveReservations = await _context.Reservations
                .AnyAsync(r => r.TableId == id 
                    && r.ReservationDate.Date >= DateTime.Today 
                    && r.Status != ReservationStatus.Cancelled);

            if (hasActiveReservations)
            {
                TempData["Error"] = "Cannot delete table with active reservations";
                return RedirectToAction(nameof(Tables));
            }

            _context.Tables.Remove(table);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Table deleted successfully";
            return RedirectToAction(nameof(Tables));
        }

        private int GetRestaurantId()
        {
            var restaurantIdClaim = User.FindFirst("RestaurantId")?.Value;
            return int.Parse(restaurantIdClaim ?? "0");
        }

        private List<ReservationViewModel> MapReservationsToViewModel(List<Reservation> reservations)
        {
            return reservations.Select(r => new ReservationViewModel
            {
                Id = r.Id,
                BookingReference = r.BookingReference,
                RestaurantName = r.Restaurant?.Name ?? "",
                ReservationDate = r.ReservationDate,
                ReservationTime = r.ReservationTime,
                NumberOfGuests = r.NumberOfGuests,
                Status = r.Status
            }).ToList();
        }
    }
}
