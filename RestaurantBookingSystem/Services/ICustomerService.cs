using RestaurantBookingSystem.Models;

namespace RestaurantBookingSystem.Services
{
    public interface ICustomerService
    {
        // Authentication
        Task<Customer?> AuthenticateAsync(string email, string password);
        Task<Customer?> GetByEmailAsync(string email);
        Task<Customer?> GetCustomerByEmailAsync(string email); // Alias for GetByEmailAsync
        Task<Customer?> GetByIdAsync(int customerId);
        Task UpdateLastLoginAsync(int customerId);

        // CRUD Operations
        Task<int> CreateAsync(Customer customer);
        Task<Customer> CreateCustomerAsync(Customer customer); // ⭐ NEW - Alias for CreateAsync (line 102)
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(int customerId);

        // Email Verification
        Task SaveVerificationTokenAsync(int customerId, string token);
        Task<bool> VerifyEmailAsync(string token);

        // Password Reset
        Task SavePasswordResetTokenAsync(int customerId, string token);
        Task<bool> ResetPasswordAsync(string token, string newPasswordHash);

        // Statistics
        Task<int> GetTotalReservationsAsync(int customerId);
        Task<int> GetTotalReviewsAsync(int customerId);
        Task<int> GetFavoriteRestaurantsCountAsync(int customerId);

        // Utility
        Task<List<Customer>> GetAllAsync();
        Task<bool> EmailExistsAsync(string email);
    }
}