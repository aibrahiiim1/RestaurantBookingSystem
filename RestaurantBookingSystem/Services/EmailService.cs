using System.Net;
using System.Net.Mail;

namespace RestaurantBookingSystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            return await SendEmailAsync(to, subject, body, null, null);
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, string? cc = null, string? bcc = null)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUsername = _configuration["Email:SmtpUsername"] ?? "";
                var smtpPassword = _configuration["Email:SmtpPassword"] ?? "";
                var fromEmail = _configuration["Email:FromEmail"] ?? smtpUsername;
                var fromName = _configuration["Email:FromName"] ?? "Restaurant Booking System";

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);

                if (!string.IsNullOrEmpty(cc))
                    mailMessage.CC.Add(cc);

                if (!string.IsNullOrEmpty(bcc))
                    mailMessage.Bcc.Add(bcc);

                await client.SendMailAsync(mailMessage);

                _logger.LogInformation("Email sent successfully to {To}", to);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", to);
                return false;
            }
        }

        public async Task<bool> SendTemplateEmailAsync(string to, string templateName, Dictionary<string, string> parameters)
        {
            try
            {
                var template = await LoadTemplateAsync(templateName);

                foreach (var param in parameters)
                {
                    template = template.Replace($"{{{{{param.Key}}}}}", param.Value);
                }

                var lines = template.Split('\n');
                var subject = lines[0].Replace("Subject:", "").Trim();
                var body = string.Join("\n", lines.Skip(1));

                return await SendEmailAsync(to, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send template email to {To}", to);
                return false;
            }
        }

        public async Task<bool> SendBookingConfirmationAsync(string to, string bookingReference, DateTime reservationDate, TimeSpan reservationTime, string restaurantName)
        {
            var subject = $"Booking Confirmation - {bookingReference}";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Booking Confirmation</h2>
                    <p>Thank you for your reservation!</p>
                    <hr/>
                    <p><strong>Booking Reference:</strong> {bookingReference}</p>
                    <p><strong>Restaurant:</strong> {restaurantName}</p>
                    <p><strong>Date:</strong> {reservationDate:dddd, MMMM dd, yyyy}</p>
                    <p><strong>Time:</strong> {reservationTime:hh\\:mm}</p>
                    <hr/>
                    <p>We look forward to serving you!</p>
                    <p>If you need to cancel or modify your reservation, please contact us.</p>
                </body>
                </html>
            ";

            return await SendEmailAsync(to, subject, body);
        }

        public async Task<bool> SendBookingCancellationAsync(string to, string bookingReference, string restaurantName, string reason)
        {
            var subject = $"Booking Cancelled - {bookingReference}";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Booking Cancellation</h2>
                    <p>Your reservation has been cancelled.</p>
                    <hr/>
                    <p><strong>Booking Reference:</strong> {bookingReference}</p>
                    <p><strong>Restaurant:</strong> {restaurantName}</p>
                    <p><strong>Reason:</strong> {reason}</p>
                    <hr/>
                    <p>We hope to see you again soon!</p>
                </body>
                </html>
            ";

            return await SendEmailAsync(to, subject, body);
        }

        public async Task<bool> SendReservationReminderAsync(string to, string bookingReference, DateTime reservationDate, TimeSpan reservationTime, string restaurantName)
        {
            var subject = $"Reservation Reminder - {restaurantName}";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Reservation Reminder</h2>
                    <p>This is a friendly reminder about your upcoming reservation.</p>
                    <hr/>
                    <p><strong>Booking Reference:</strong> {bookingReference}</p>
                    <p><strong>Restaurant:</strong> {restaurantName}</p>
                    <p><strong>Date:</strong> {reservationDate:dddd, MMMM dd, yyyy}</p>
                    <p><strong>Time:</strong> {reservationTime:hh\\:mm}</p>
                    <hr/>
                    <p>We look forward to seeing you!</p>
                    <p>If you need to cancel or modify, please let us know as soon as possible.</p>
                </body>
                </html>
            ";

            return await SendEmailAsync(to, subject, body);
        }

        // ⭐ NEW METHOD - Added for line 107
        public async Task<bool> SendVerificationCodeAsync(string to, string verificationCode)
        {
            var subject = "Email Verification Code";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Email Verification</h2>
                    <p>Thank you for booking with us!</p>
                    <p>Your verification code is:</p>
                    <h1 style='color: #007bff; letter-spacing: 5px; font-size: 36px;'>{verificationCode}</h1>
                    <p>This code will expire in 15 minutes.</p>
                    <p>If you didn't request this code, please ignore this email.</p>
                </body>
                </html>
            ";

            return await SendEmailAsync(to, subject, body);
        }

        // ⭐ NEW METHOD - Added for line 259
        public async Task<bool> SendCancellationConfirmationAsync(string to, string bookingReference, string restaurantName)
        {
            var subject = $"Cancellation Confirmed - {bookingReference}";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Cancellation Confirmed</h2>
                    <p>Your reservation has been successfully cancelled.</p>
                    <hr/>
                    <p><strong>Booking Reference:</strong> {bookingReference}</p>
                    <p><strong>Restaurant:</strong> {restaurantName}</p>
                    <hr/>
                    <p>We're sorry to see you won't be joining us.</p>
                    <p>We hope to welcome you in the future!</p>
                </body>
                </html>
            ";

            return await SendEmailAsync(to, subject, body);
        }

        private async Task<string> LoadTemplateAsync(string templateName)
        {
            await Task.CompletedTask;
            return $"Subject: Email Template\n\nThis is a template email for {templateName}";
        }
    }
}