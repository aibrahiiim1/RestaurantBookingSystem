using Microsoft.EntityFrameworkCore;
using RestaurantBookingSystem.Data;
using RestaurantBookingSystem.Models;
using RestaurantBookingSystem.ViewModels;

namespace RestaurantBookingSystem.Services
{
    public interface IReservationService
    {
        Task<List<TimeSlotViewModel>> GetAvailableTimeSlotsAsync(int restaurantId, DateTime date, int partySize);
        Task<Reservation> CreateReservationAsync(BookingConfirmationViewModel model, int customerId);
        Task<bool> CancelReservationAsync(int reservationId, string reason);
        Task<bool> UpdateReservationStatusAsync(int reservationId, ReservationStatus status);
        Task<Reservation> GetReservationByIdAsync(int id);
        Task<Reservation> GetReservationByReferenceAsync(string reference);
        Task<List<Reservation>> GetCustomerReservationsAsync(int customerId);
        Task<List<Reservation>> GetRestaurantReservationsAsync(int restaurantId, DateTime? date, ReservationStatus? status);
        Task<bool> CanCancelReservationAsync(int reservationId);
        Task<bool> CanModifyReservationAsync(int reservationId);
        Task<string> GenerateBookingReferenceAsync();
    }

    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;

        public ReservationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TimeSlotViewModel>> GetAvailableTimeSlotsAsync(int restaurantId, DateTime date, int partySize)
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.OpeningTimes)
                .Include(r => r.Tables)
                .Include(r => r.Closures)
                .FirstOrDefaultAsync(r => r.Id == restaurantId);

            if (restaurant == null)
                return new List<TimeSlotViewModel>();

            // Check if restaurant is closed on this date
            var isClosed = await _context.RestaurantClosures
                .AnyAsync(c => c.RestaurantId == restaurantId && c.ClosureDate.Date == date.Date);

            if (isClosed)
                return new List<TimeSlotViewModel>();

            // Get opening hours for the day
            var dayOfWeek = date.DayOfWeek;
            var openingTime = restaurant.OpeningTimes.FirstOrDefault(o => o.DayOfWeek == dayOfWeek);

            if (openingTime == null || openingTime.IsClosed)
                return new List<TimeSlotViewModel>();

            // Get all reservations for this date
            var existingReservations = await _context.Reservations
                .Include(r => r.Table)
                .Where(r => r.RestaurantId == restaurantId 
                    && r.ReservationDate.Date == date.Date 
                    && r.Status != ReservationStatus.Cancelled)
                .ToListAsync();

            // Generate time slots
            var timeSlots = new List<TimeSlotViewModel>();
            var currentTime = openingTime.OpenTime;
            var lastBookingTime = openingTime.CloseTime.AddMinutes(-restaurant.DefaultBookingDurationMinutes);

            while (currentTime <= lastBookingTime)
            {
                // Find tables that can accommodate party size and are available
                var availableTables = restaurant.Tables
                    .Where(t => t.SeatingCapacity >= partySize && t.IsAvailable)
                    .ToList();

                // Check each table's availability for this time slot
                var slotStart = date.Date.Add(currentTime.ToTimeSpan());
                var slotEnd = slotStart.AddMinutes(restaurant.DefaultBookingDurationMinutes);

                var tablesBooked = existingReservations
                    .Where(r => r.StartTime < slotEnd && r.EndTime > slotStart)
                    .Select(r => r.TableId)
                    .ToHashSet();

                var availableTablesCount = availableTables.Count(t => !tablesBooked.Contains(t.Id));

                timeSlots.Add(new TimeSlotViewModel
                {
                    Time = currentTime,
                    IsAvailable = availableTablesCount > 0,
                    AvailableTables = availableTablesCount,
                    DisplayTime = currentTime.ToString("h:mm tt")
                });

                currentTime = currentTime.AddMinutes(restaurant.TimeSlotIntervalMinutes);
            }

            return timeSlots;
        }

        public async Task<Reservation> CreateReservationAsync(BookingConfirmationViewModel model, int customerId)
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.Tables)
                .FirstOrDefaultAsync(r => r.Id == model.RestaurantId);

            if (restaurant == null)
                throw new Exception("Restaurant not found");

            // Find available table
            var slotStart = model.Date.Date.Add(model.Time.ToTimeSpan());
            var slotEnd = slotStart.AddMinutes(restaurant.DefaultBookingDurationMinutes);

            var bookedTableIds = await _context.Reservations
                .Where(r => r.RestaurantId == model.RestaurantId
                    && r.ReservationDate.Date == model.Date.Date
                    && r.StartTime < slotEnd
                    && r.EndTime > slotStart
                    && r.Status != ReservationStatus.Cancelled)
                .Select(r => r.TableId)
                .ToListAsync();

            var availableTable = restaurant.Tables
                .Where(t => t.SeatingCapacity >= model.PartySize 
                    && t.IsAvailable 
                    && !bookedTableIds.Contains(t.Id)
                    && t.Location == model.PreferredLocation)
                .OrderBy(t => t.SeatingCapacity)
                .FirstOrDefault();

            // If no table with preferred location, try any location
            if (availableTable == null)
            {
                availableTable = restaurant.Tables
                    .Where(t => t.SeatingCapacity >= model.PartySize 
                        && t.IsAvailable 
                        && !bookedTableIds.Contains(t.Id))
                    .OrderBy(t => t.SeatingCapacity)
                    .FirstOrDefault();
            }

            if (availableTable == null)
                throw new Exception("No tables available for the selected time");

            var reservation = new Reservation
            {
                BookingReference = await GenerateBookingReferenceAsync(),
                CustomerId = customerId,
                RestaurantId = model.RestaurantId,
                TableId = availableTable.Id,
                ReservationDate = model.Date.Date,
                ReservationTime = model.Time,
                StartTime = slotStart,
                EndTime = slotEnd,
                NumberOfGuests = model.PartySize,
                PreferredTableLocation = model.PreferredLocation,
                OccasionId = model.OccasionId,
                SpecialRequests = model.SpecialRequests,
                Status = ReservationStatus.Confirmed,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return reservation;
        }

        public async Task<bool> CancelReservationAsync(int reservationId, string reason)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null)
                return false;

            var restaurant = await _context.Restaurants.FindAsync(reservation.RestaurantId);
            if (restaurant == null)
                return false;

            // Check cancellation policy
            var hoursBefore = (reservation.StartTime - DateTime.UtcNow).TotalHours;
            if (hoursBefore < restaurant.CancellationPolicyHours)
                return false;

            reservation.Status = ReservationStatus.Cancelled;
            reservation.CancellationReason = reason;
            reservation.CancelledAt = DateTime.UtcNow;
            reservation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateReservationStatusAsync(int reservationId, ReservationStatus status)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null)
                return false;

            reservation.Status = status;
            reservation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Reservation> GetReservationByIdAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Restaurant)
                .Include(r => r.Table)
                .Include(r => r.Occasion)
                .Include(r => r.Messages)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Reservation> GetReservationByReferenceAsync(string reference)
        {
            return await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Restaurant)
                .Include(r => r.Table)
                .Include(r => r.Occasion)
                .Include(r => r.Messages)
                .FirstOrDefaultAsync(r => r.BookingReference == reference);
        }

        public async Task<List<Reservation>> GetCustomerReservationsAsync(int customerId)
        {
            return await _context.Reservations
                .Include(r => r.Restaurant)
                .ThenInclude(r => r.Photos)
                .Include(r => r.Occasion)
                .Include(r => r.Table)
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.ReservationDate)
                .ThenByDescending(r => r.ReservationTime)
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetRestaurantReservationsAsync(int restaurantId, DateTime? date, ReservationStatus? status)
        {
            var query = _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Table)
                .Include(r => r.Occasion)
                .Include(r => r.Messages)
                .Where(r => r.RestaurantId == restaurantId);

            if (date.HasValue)
            {
                query = query.Where(r => r.ReservationDate.Date == date.Value.Date);
            }

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            return await query
                .OrderBy(r => r.ReservationDate)
                .ThenBy(r => r.ReservationTime)
                .ToListAsync();
        }

        public async Task<bool> CanCancelReservationAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Restaurant)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null || reservation.Status == ReservationStatus.Cancelled)
                return false;

            var hoursBefore = (reservation.StartTime - DateTime.UtcNow).TotalHours;
            return hoursBefore >= reservation.Restaurant.CancellationPolicyHours;
        }

        public async Task<bool> CanModifyReservationAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Restaurant)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null || reservation.Status == ReservationStatus.Cancelled)
                return false;

            var hoursBefore = (reservation.StartTime - DateTime.UtcNow).TotalHours;
            return hoursBefore >= reservation.Restaurant.CancellationPolicyHours;
        }

        public async Task<string> GenerateBookingReferenceAsync()
        {
            string reference;
            bool exists;

            do
            {
                var datePart = DateTime.Now.ToString("yyyyMMdd");
                var randomPart = new Random().Next(1000, 9999);
                reference = $"RES-{datePart}-{randomPart}";

                exists = await _context.Reservations.AnyAsync(r => r.BookingReference == reference);
            } while (exists);

            return reference;
        }
    }
}
