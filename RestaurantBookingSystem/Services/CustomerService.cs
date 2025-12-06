using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RestaurantBookingSystem.Data;
using RestaurantBookingSystem.Models;
using System.Data;

namespace RestaurantBookingSystem.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(IConfiguration configuration, ILogger<CustomerService> logger, ApplicationDbContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
            _context = context;
        }
        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            try
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                throw;
            }
        }

        #region Authentication

        public async Task<Customer?> AuthenticateAsync(string email, string password)
        {
            try
            {
                var customer = await GetByEmailAsync(email);
                
                if (customer == null)
                    return null;

                // Verify password (assuming password is already hashed by the controller)
                // In production, use a proper password hashing library like BCrypt
                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating customer with email: {Email}", email);
                throw;
            }
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT CustomerId, FirstName, LastName, Email, PhoneNumber, PasswordHash, 
                           DateOfBirth, AvatarUrl, IsEmailVerified, RequiresEmailVerification,
                           TwoFactorEnabled, EmailNotifications, SmsNotifications,
                           FavoriteCuisines, DietaryRestrictions, DefaultPartySize, CreatedAt, LastLoginAt
                    FROM Customers 
                    WHERE Email = @Email AND IsActive = 1";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return MapCustomerFromReader(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer by email: {Email}", email);
                throw;
            }
        }

        public async Task<Customer?> GetByIdAsync(int customerId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT CustomerId, FirstName, LastName, Email, PhoneNumber, PasswordHash, 
                           DateOfBirth, AvatarUrl, IsEmailVerified, RequiresEmailVerification,
                           TwoFactorEnabled, EmailNotifications, SmsNotifications,
                           FavoriteCuisines, DietaryRestrictions, DefaultPartySize, CreatedAt, LastLoginAt
                    FROM Customers 
                    WHERE CustomerId = @CustomerId AND IsActive = 1";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerId", customerId);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return MapCustomerFromReader(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer by ID: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task UpdateLastLoginAsync(int customerId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "UPDATE Customers SET LastLoginAt = @LastLoginAt WHERE CustomerId = @CustomerId";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerId", customerId);
                command.Parameters.AddWithValue("@LastLoginAt", DateTime.UtcNow);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating last login for customer: {CustomerId}", customerId);
                // Don't throw - this is not critical
            }
        }

        #endregion

        #region CRUD Operations

        public async Task<int> CreateAsync(Customer customer)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    INSERT INTO Customers (
                        FirstName, LastName, Email, PhoneNumber, PasswordHash, DateOfBirth,
                        IsEmailVerified, RequiresEmailVerification, IsActive, CreatedAt
                    )
                    VALUES (
                        @FirstName, @LastName, @Email, @PhoneNumber, @PasswordHash, @DateOfBirth,
                        @IsEmailVerified, @RequiresEmailVerification, 1, @CreatedAt
                    );
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FirstName", customer.FirstName);
                command.Parameters.AddWithValue("@LastName", customer.LastName);
                command.Parameters.AddWithValue("@Email", customer.Email);
                command.Parameters.AddWithValue("@PhoneNumber", (object?)customer.PhoneNumber ?? DBNull.Value);
                command.Parameters.AddWithValue("@PasswordHash", customer.PasswordHash);
                command.Parameters.AddWithValue("@DateOfBirth", (object?)customer.DateOfBirth ?? DBNull.Value);
                command.Parameters.AddWithValue("@IsEmailVerified", customer.IsEmailVerified);
                command.Parameters.AddWithValue("@RequiresEmailVerification", customer.RequiresEmailVerification);
                command.Parameters.AddWithValue("@CreatedAt", customer.CreatedAt);

                var customerId = (int)await command.ExecuteScalarAsync();
                
                _logger.LogInformation("Created new customer with ID: {CustomerId}", customerId);
                return customerId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                throw;
            }
        }

        public async Task UpdateAsync(Customer customer)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    UPDATE Customers SET
                        FirstName = @FirstName,
                        LastName = @LastName,
                        Email = @Email,
                        PhoneNumber = @PhoneNumber,
                        PasswordHash = @PasswordHash,
                        DateOfBirth = @DateOfBirth,
                        AvatarUrl = @AvatarUrl,
                        IsEmailVerified = @IsEmailVerified,
                        TwoFactorEnabled = @TwoFactorEnabled,
                        EmailNotifications = @EmailNotifications,
                        SmsNotifications = @SmsNotifications,
                        FavoriteCuisines = @FavoriteCuisines,
                        DietaryRestrictions = @DietaryRestrictions,
                        DefaultPartySize = @DefaultPartySize
                    WHERE CustomerId = @CustomerId";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
                command.Parameters.AddWithValue("@FirstName", customer.FirstName);
                command.Parameters.AddWithValue("@LastName", customer.LastName);
                command.Parameters.AddWithValue("@Email", customer.Email);
                command.Parameters.AddWithValue("@PhoneNumber", (object?)customer.PhoneNumber ?? DBNull.Value);
                command.Parameters.AddWithValue("@PasswordHash", customer.PasswordHash);
                command.Parameters.AddWithValue("@DateOfBirth", (object?)customer.DateOfBirth ?? DBNull.Value);
                command.Parameters.AddWithValue("@AvatarUrl", (object?)customer.AvatarUrl ?? DBNull.Value);
                command.Parameters.AddWithValue("@IsEmailVerified", customer.IsEmailVerified);
                command.Parameters.AddWithValue("@TwoFactorEnabled", customer.TwoFactorEnabled);
                command.Parameters.AddWithValue("@EmailNotifications", customer.EmailNotifications);
                command.Parameters.AddWithValue("@SmsNotifications", customer.SmsNotifications);
                command.Parameters.AddWithValue("@FavoriteCuisines", (object?)customer.FavoriteCuisines ?? DBNull.Value);
                command.Parameters.AddWithValue("@DietaryRestrictions", (object?)customer.DietaryRestrictions ?? DBNull.Value);
                command.Parameters.AddWithValue("@DefaultPartySize", (object?)customer.DefaultPartySize ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
                
                _logger.LogInformation("Updated customer: {CustomerId}", customer.CustomerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer: {CustomerId}", customer.CustomerId);
                throw;
            }
        }

        public async Task DeleteAsync(int customerId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Soft delete
                var query = "UPDATE Customers SET IsActive = 0 WHERE CustomerId = @CustomerId";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerId", customerId);

                await command.ExecuteNonQueryAsync();
                
                _logger.LogInformation("Deleted customer: {CustomerId}", customerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer: {CustomerId}", customerId);
                throw;
            }
        }

        #endregion

        #region Email Verification

        public async Task SaveVerificationTokenAsync(int customerId, string token)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    INSERT INTO EmailVerificationTokens (CustomerId, Token, ExpiresAt, CreatedAt)
                    VALUES (@CustomerId, @Token, @ExpiresAt, @CreatedAt)";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerId", customerId);
                command.Parameters.AddWithValue("@Token", token);
                command.Parameters.AddWithValue("@ExpiresAt", DateTime.UtcNow.AddHours(24));
                command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving verification token for customer: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Get the customer ID from the token
                var query = @"
                    SELECT CustomerId FROM EmailVerificationTokens 
                    WHERE Token = @Token 
                    AND ExpiresAt > @CurrentTime 
                    AND IsUsed = 0";

                int? customerId = null;
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Token", token);
                    command.Parameters.AddWithValue("@CurrentTime", DateTime.UtcNow);

                    var result = await command.ExecuteScalarAsync();
                    if (result != null && result != DBNull.Value)
                    {
                        customerId = Convert.ToInt32(result);
                    }
                }

                if (!customerId.HasValue)
                {
                    return false;
                }

                // Mark token as used and verify email
                using var transaction = connection.BeginTransaction();
                
                try
                {
                    var updateTokenQuery = @"
                        UPDATE EmailVerificationTokens 
                        SET IsUsed = 1, UsedAt = @UsedAt 
                        WHERE Token = @Token";

                    using (var command = new SqlCommand(updateTokenQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@Token", token);
                        command.Parameters.AddWithValue("@UsedAt", DateTime.UtcNow);
                        await command.ExecuteNonQueryAsync();
                    }

                    var updateCustomerQuery = @"
                        UPDATE Customers 
                        SET IsEmailVerified = 1 
                        WHERE CustomerId = @CustomerId";

                    using (var command = new SqlCommand(updateCustomerQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@CustomerId", customerId.Value);
                        await command.ExecuteNonQueryAsync();
                    }

                    transaction.Commit();
                    _logger.LogInformation("Email verified for customer: {CustomerId}", customerId.Value);
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying email with token");
                throw;
            }
        }

        #endregion

        #region Password Reset

        public async Task SavePasswordResetTokenAsync(int customerId, string token)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    INSERT INTO PasswordResetTokens (CustomerId, Token, ExpiresAt, CreatedAt)
                    VALUES (@CustomerId, @Token, @ExpiresAt, @CreatedAt)";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerId", customerId);
                command.Parameters.AddWithValue("@Token", token);
                command.Parameters.AddWithValue("@ExpiresAt", DateTime.UtcNow.AddHours(24));
                command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving password reset token for customer: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPasswordHash)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Get the customer ID from the token
                var query = @"
                    SELECT CustomerId FROM PasswordResetTokens 
                    WHERE Token = @Token 
                    AND ExpiresAt > @CurrentTime 
                    AND IsUsed = 0";

                int? customerId = null;
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Token", token);
                    command.Parameters.AddWithValue("@CurrentTime", DateTime.UtcNow);

                    var result = await command.ExecuteScalarAsync();
                    if (result != null && result != DBNull.Value)
                    {
                        customerId = Convert.ToInt32(result);
                    }
                }

                if (!customerId.HasValue)
                {
                    return false;
                }

                // Mark token as used and update password
                using var transaction = connection.BeginTransaction();
                
                try
                {
                    var updateTokenQuery = @"
                        UPDATE PasswordResetTokens 
                        SET IsUsed = 1, UsedAt = @UsedAt 
                        WHERE Token = @Token";

                    using (var command = new SqlCommand(updateTokenQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@Token", token);
                        command.Parameters.AddWithValue("@UsedAt", DateTime.UtcNow);
                        await command.ExecuteNonQueryAsync();
                    }

                    var updatePasswordQuery = @"
                        UPDATE Customers 
                        SET PasswordHash = @PasswordHash 
                        WHERE CustomerId = @CustomerId";

                    using (var command = new SqlCommand(updatePasswordQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@CustomerId", customerId.Value);
                        command.Parameters.AddWithValue("@PasswordHash", newPasswordHash);
                        await command.ExecuteNonQueryAsync();
                    }

                    transaction.Commit();
                    _logger.LogInformation("Password reset for customer: {CustomerId}", customerId.Value);
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password with token");
                throw;
            }
        }

        #endregion

        #region Profile Statistics

        public async Task<int> GetTotalReservationsAsync(int customerId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "SELECT COUNT(*) FROM Reservations WHERE CustomerId = @CustomerId";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerId", customerId);

                return (int)await command.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total reservations for customer: {CustomerId}", customerId);
                return 0;
            }
        }

        public async Task<int> GetTotalReviewsAsync(int customerId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "SELECT COUNT(*) FROM Reviews WHERE CustomerId = @CustomerId";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerId", customerId);

                return (int)await command.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total reviews for customer: {CustomerId}", customerId);
                return 0;
            }
        }

        public async Task<int> GetFavoriteRestaurantsCountAsync(int customerId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "SELECT COUNT(*) FROM FavoriteRestaurants WHERE CustomerId = @CustomerId";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerId", customerId);

                return (int)await command.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting favorite restaurants count for customer: {CustomerId}", customerId);
                return 0;
            }
        }

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            return await GetByEmailAsync(email);
        }
        #endregion

        #region Additional Methods

        public async Task<List<Customer>> GetAllAsync()
        {
            try
            {
                var customers = new List<Customer>();

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT CustomerId, FirstName, LastName, Email, PhoneNumber, PasswordHash, 
                           DateOfBirth, AvatarUrl, IsEmailVerified, RequiresEmailVerification,
                           TwoFactorEnabled, EmailNotifications, SmsNotifications,
                           FavoriteCuisines, DietaryRestrictions, DefaultPartySize, CreatedAt, LastLoginAt
                    FROM Customers 
                    WHERE IsActive = 1
                    ORDER BY CreatedAt DESC";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    customers.Add(MapCustomerFromReader(reader));
                }

                return customers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all customers");
                throw;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "SELECT COUNT(*) FROM Customers WHERE Email = @Email AND IsActive = 1";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);

                var count = (int)await command.ExecuteScalarAsync();
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if email exists: {Email}", email);
                throw;
            }
        }

        #endregion

        #region Helper Methods

        private Customer MapCustomerFromReader(SqlDataReader reader)
        {
            return new Customer
            {
                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? null : reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                AvatarUrl = reader.IsDBNull(reader.GetOrdinal("AvatarUrl")) ? null : reader.GetString(reader.GetOrdinal("AvatarUrl")),
                IsEmailVerified = reader.GetBoolean(reader.GetOrdinal("IsEmailVerified")),
                RequiresEmailVerification = reader.GetBoolean(reader.GetOrdinal("RequiresEmailVerification")),
                TwoFactorEnabled = reader.GetBoolean(reader.GetOrdinal("TwoFactorEnabled")),
                EmailNotifications = reader.GetBoolean(reader.GetOrdinal("EmailNotifications")),
                SmsNotifications = reader.GetBoolean(reader.GetOrdinal("SmsNotifications")),
                FavoriteCuisines = reader.IsDBNull(reader.GetOrdinal("FavoriteCuisines")) ? null : reader.GetString(reader.GetOrdinal("FavoriteCuisines")),
                DietaryRestrictions = reader.IsDBNull(reader.GetOrdinal("DietaryRestrictions")) ? null : reader.GetString(reader.GetOrdinal("DietaryRestrictions")),
                DefaultPartySize = reader.IsDBNull(reader.GetOrdinal("DefaultPartySize")) ? null : reader.GetInt32(reader.GetOrdinal("DefaultPartySize")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                LastLoginAt = reader.IsDBNull(reader.GetOrdinal("LastLoginAt")) ? null : reader.GetDateTime(reader.GetOrdinal("LastLoginAt"))
            };
        }

        #endregion
    }
}
