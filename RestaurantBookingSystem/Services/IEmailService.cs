namespace RestaurantBookingSystem.Services
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email asynchronously
        /// </summary>
        Task<bool> SendEmailAsync(string to, string subject, string body);

        /// <summary>
        /// Sends an email with CC and BCC asynchronously
        /// </summary>
        Task<bool> SendEmailAsync(string to, string subject, string body, string? cc = null, string? bcc = null);

        /// <summary>
        /// Sends an email using a template asynchronously
        /// </summary>
        Task<bool> SendTemplateEmailAsync(string to, string templateName, Dictionary<string, string> parameters);

        /// <summary>
        /// Sends a booking confirmation email
        /// </summary>
        Task<bool> SendBookingConfirmationAsync(string to, string bookingReference, DateTime reservationDate, TimeSpan reservationTime, string restaurantName);

        /// <summary>
        /// Sends a booking cancellation email
        /// </summary>
        Task<bool> SendBookingCancellationAsync(string to, string bookingReference, string restaurantName, string reason);

        /// <summary>
        /// Sends a reminder email before the reservation
        /// </summary>
        Task<bool> SendReservationReminderAsync(string to, string bookingReference, DateTime reservationDate, TimeSpan reservationTime, string restaurantName);

        /// <summary>
        /// Sends a verification code email
        /// ⭐ NEW - Added for line 107
        /// </summary>
        Task<bool> SendVerificationCodeAsync(string to, string verificationCode);

        /// <summary>
        /// Sends a cancellation confirmation email
        /// ⭐ NEW - Added for line 259
        /// </summary>
        Task<bool> SendCancellationConfirmationAsync(string to, string bookingReference, string restaurantName);
    }
}