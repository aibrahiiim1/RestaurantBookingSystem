using Microsoft.EntityFrameworkCore;
using RestaurantBookingSystem.Data;
using RestaurantBookingSystem.Models;
using System.Security.Cryptography;
using System.Text;

namespace RestaurantBookingSystem.Services
{
    //public interface ICustomerService
    //{
    //    Task<Customer> GetCustomerByIdAsync(int id);
    //    Task<Customer> GetCustomerByEmailAsync(string email);
    //    Task<Customer> GetCustomerByPhoneAsync(string phone);
    //    Task<Customer> CreateCustomerAsync(Customer customer);
    //    Task<bool> UpdateCustomerAsync(Customer customer);
    //    Task<List<RestaurantReview>> GetCustomerReviewsAsync(int customerId);
    //    Task<bool> AddFavoriteRestaurantAsync(int customerId, int restaurantId);
    //    Task<bool> RemoveFavoriteRestaurantAsync(int customerId, int restaurantId);
    //    Task<List<Restaurant>> GetFavoriteRestaurantsAsync(int customerId);
    //}

    //public class CustomerService : ICustomerService
    //{
    //    private readonly ApplicationDbContext _context;

    //    public CustomerService(ApplicationDbContext context)
    //    {
    //        _context = context;
    //    }

    //    public async Task<Customer> GetCustomerByIdAsync(int id)
    //    {
    //        return await _context.Customers
    //            .Include(c => c.Allergies)
    //            .FirstOrDefaultAsync(c => c.Id == id);
    //    }

    //    public async Task<Customer> GetCustomerByEmailAsync(string email)
    //    {
    //        return await _context.Customers
    //            .Include(c => c.Allergies)
    //            .FirstOrDefaultAsync(c => c.Email == email);
    //    }

    //    public async Task<Customer> GetCustomerByPhoneAsync(string phone)
    //    {
    //        return await _context.Customers
    //            .Include(c => c.Allergies)
    //            .FirstOrDefaultAsync(c => c.PhoneNumber == phone);
    //    }

    //    public async Task<Customer> CreateCustomerAsync(Customer customer)
    //    {
    //        customer.CreatedDate = DateTime.UtcNow;
    //        customer.IsActive = true;

    //        _context.Customers.Add(customer);
    //        await _context.SaveChangesAsync();

    //        return customer;
    //    }

    //    public async Task<bool> UpdateCustomerAsync(Customer customer)
    //    {
    //        try
    //        {
    //            _context.Customers.Update(customer);
    //            await _context.SaveChangesAsync();
    //            return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }

    //    public async Task<List<RestaurantReview>> GetCustomerReviewsAsync(int customerId)
    //    {
    //        return await _context.RestaurantReviews
    //            .Include(r => r.Restaurant)
    //            .Where(r => r.CustomerId == customerId)
    //            .OrderByDescending(r => r.ReviewDate)
    //            .ToListAsync();
    //    }

    //    public async Task<bool> AddFavoriteRestaurantAsync(int customerId, int restaurantId)
    //    {
    //        var exists = await _context.FavoriteRestaurants
    //            .AnyAsync(f => f.CustomerId == customerId && f.RestaurantId == restaurantId);

    //        if (exists)
    //            return false;

    //        var favorite = new FavoriteRestaurant
    //        {
    //            CustomerId = customerId,
    //            RestaurantId = restaurantId,
    //            AddedDate = DateTime.UtcNow
    //        };

    //        _context.FavoriteRestaurants.Add(favorite);
    //        await _context.SaveChangesAsync();
    //        return true;
    //    }

    //    public async Task<bool> RemoveFavoriteRestaurantAsync(int customerId, int restaurantId)
    //    {
    //        var favorite = await _context.FavoriteRestaurants
    //            .FirstOrDefaultAsync(f => f.CustomerId == customerId && f.RestaurantId == restaurantId);

    //        if (favorite == null)
    //            return false;

    //        _context.FavoriteRestaurants.Remove(favorite);
    //        await _context.SaveChangesAsync();
    //        return true;
    //    }

    //    public async Task<List<Restaurant>> GetFavoriteRestaurantsAsync(int customerId)
    //    {
    //        return await _context.FavoriteRestaurants
    //            .Include(f => f.Restaurant)
    //            .ThenInclude(r => r.Cuisine)
    //            .Include(f => f.Restaurant)
    //            .ThenInclude(r => r.Photos)
    //            .Where(f => f.CustomerId == customerId)
    //            .Select(f => f.Restaurant)
    //            .ToListAsync();
    //    }
    //}

    public interface IAuthenticationService
    {
        Task<Customer> AuthenticateCustomerAsync(string email, string password);
        Task<User> AuthenticateUserAsync(string username, string password);
        Task<Customer> RegisterCustomerAsync(string firstName, string lastName, string email, string phone, string password);
        Task<string> GenerateVerificationCodeAsync();
        Task<bool> VerifyCodeAsync(string email, string code);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<string, (string Code, DateTime Expiry)> _verificationCodes = new();

        public AuthenticationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> AuthenticateCustomerAsync(string email, string password)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email && !c.IsGuest && c.IsActive);

            if (customer == null || string.IsNullOrEmpty(customer.PasswordHash))
                return null;

            if (!VerifyPassword(password, customer.PasswordHash))
                return null;

            customer.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return customer;
        }

        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            var user = await _context.Users
                .Include(u => u.Restaurant)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            if (user == null)
                return null;

            if (!VerifyPassword(password, user.PasswordHash))
                return null;

            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<Customer> RegisterCustomerAsync(string firstName, string lastName, string email, string phone, string password)
        {
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);

            if (existingCustomer != null)
                throw new Exception("Customer with this email already exists");

            var customer = new Customer
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phone,
                PasswordHash = HashPassword(password),
                IsGuest = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ReceivePromotions = true,
                ReceiveReminders = true
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        public async Task<string> GenerateVerificationCodeAsync()
        {
            var random = new Random();
            var code = random.Next(100000, 999999).ToString();
            
            return await Task.FromResult(code);
        }

        public async Task<bool> VerifyCodeAsync(string email, string code)
        {
            if (!_verificationCodes.ContainsKey(email))
                return false;

            var (storedCode, expiry) = _verificationCodes[email];

            if (DateTime.UtcNow > expiry)
            {
                _verificationCodes.Remove(email);
                return false;
            }

            if (storedCode != code)
                return false;

            _verificationCodes.Remove(email);
            return await Task.FromResult(true);
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        public bool VerifyPassword(string password, string hash)
        {
            var passwordHash = HashPassword(password);
            return passwordHash == hash;
        }
    }

    //public interface IEmailService
    //{
    //    Task SendVerificationCodeAsync(string email, string code);
    //    Task SendBookingConfirmationAsync(Reservation reservation);
    //    Task SendCancellationConfirmationAsync(Reservation reservation);
    //    Task SendReminderAsync(Reservation reservation);
    //}

    //public class EmailService : IEmailService
    //{
    //    public async Task SendVerificationCodeAsync(string email, string code)
    //    {
    //        // In production, integrate with SendGrid, AWS SES, or similar
    //        Console.WriteLine($"Sending verification code {code} to {email}");
    //        await Task.CompletedTask;
    //    }

    //    public async Task SendBookingConfirmationAsync(Reservation reservation)
    //    {
    //        Console.WriteLine($"Sending booking confirmation for {reservation.BookingReference}");
    //        await Task.CompletedTask;
    //    }

    //    public async Task SendCancellationConfirmationAsync(Reservation reservation)
    //    {
    //        Console.WriteLine($"Sending cancellation confirmation for {reservation.BookingReference}");
    //        await Task.CompletedTask;
    //    }

    //    public async Task SendReminderAsync(Reservation reservation)
    //    {
    //        Console.WriteLine($"Sending reminder for {reservation.BookingReference}");
    //        await Task.CompletedTask;
    //    }
    //}
}
