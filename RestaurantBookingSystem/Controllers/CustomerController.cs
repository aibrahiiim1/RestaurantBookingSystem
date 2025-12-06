using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantBookingSystem.Models;
using RestaurantBookingSystem.Services;
using RestaurantBookingSystem.ViewModels;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RestaurantBookingSystem.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IEmailService _emailService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(
            ICustomerService customerService,
            IEmailService emailService,
            ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _emailService = emailService;
            _logger = logger;
        }

        #region Login & Registration

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(CustomerLoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var customer = await _customerService.AuthenticateAsync(model.Email, model.Password);

                if (customer == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return View(model);
                }

                // Check if email is verified (optional based on your requirements)
                if (!customer.IsEmailVerified && customer.RequiresEmailVerification)
                {
                    TempData["Error"] = "Please verify your email address before logging in.";
                    return RedirectToAction(nameof(ResendVerificationEmail), new { email = model.Email });
                }

                // Create claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, customer.CustomerId.ToString()),
                    new Claim(ClaimTypes.Name, $"{customer.FirstName} {customer.LastName}"),
                    new Claim(ClaimTypes.Email, customer.Email),
                    new Claim("CustomerRole", "Customer")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(12)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                _logger.LogInformation("Customer {CustomerId} logged in", customer.CustomerId);

                // Update last login
                await _customerService.UpdateLastLoginAsync(customer.CustomerId);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during customer login");
                ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CustomerRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!model.AcceptTerms)
            {
                ModelState.AddModelError(nameof(model.AcceptTerms), "You must accept the terms and conditions.");
                return View(model);
            }

            try
            {
                // Check if email already exists
                var existingCustomer = await _customerService.GetByEmailAsync(model.Email);
                if (existingCustomer != null)
                {
                    ModelState.AddModelError(nameof(model.Email), "An account with this email already exists.");
                    return View(model);
                }

                // Create new customer
                var customer = new Customer
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    DateOfBirth = model.DateOfBirth,
                    PasswordHash = HashPassword(model.Password),
                    IsEmailVerified = false,
                    RequiresEmailVerification = true,
                    CreatedAt = DateTime.UtcNow
                };

                var customerId = await _customerService.CreateAsync(customer);

                // Send verification email
                var verificationToken = GenerateVerificationToken();
                await _customerService.SaveVerificationTokenAsync(customerId, verificationToken);
                await SendVerificationEmail(customer.Email, verificationToken);

                TempData["Success"] = "Registration successful! Please check your email to verify your account.";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during customer registration");
                ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("Customer logged out");
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Email Verification

        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Invalid verification link.";
                return RedirectToAction(nameof(Login));
            }

            try
            {
                var result = await _customerService.VerifyEmailAsync(token);

                if (result)
                {
                    TempData["Success"] = "Email verified successfully! You can now log in.";
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    TempData["Error"] = "Invalid or expired verification link.";
                    return RedirectToAction(nameof(Login));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying email");
                TempData["Error"] = "An error occurred during email verification.";
                return RedirectToAction(nameof(Login));
            }
        }

        [HttpGet]
        public IActionResult ResendVerificationEmail(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendVerificationEmail(string email, bool resend)
        {
            try
            {
                var customer = await _customerService.GetByEmailAsync(email);
                if (customer == null)
                {
                    TempData["Error"] = "Customer not found.";
                    return RedirectToAction(nameof(Login));
                }

                if (customer.IsEmailVerified)
                {
                    TempData["Info"] = "Your email is already verified.";
                    return RedirectToAction(nameof(Login));
                }

                var verificationToken = GenerateVerificationToken();
                await _customerService.SaveVerificationTokenAsync(customer.CustomerId, verificationToken);
                await SendVerificationEmail(customer.Email, verificationToken);

                TempData["Success"] = "Verification email sent! Please check your inbox.";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending verification email");
                TempData["Error"] = "An error occurred. Please try again.";
                return View();
            }
        }

        #endregion

        #region Password Management

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var customer = await _customerService.GetByEmailAsync(model.Email);
                
                // Don't reveal if email exists or not for security
                if (customer != null)
                {
                    var resetToken = GeneratePasswordResetToken();
                    await _customerService.SavePasswordResetTokenAsync(customer.CustomerId, resetToken);
                    await SendPasswordResetEmail(customer.Email, resetToken);
                }

                TempData["Success"] = "If an account with that email exists, we've sent password reset instructions.";
                return RedirectToAction(nameof(ForgotPassword));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in forgot password");
                ModelState.AddModelError(string.Empty, "An error occurred. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Invalid reset link.";
                return RedirectToAction(nameof(Login));
            }

            var model = new ResetPasswordViewModel { Token = token };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _customerService.ResetPasswordAsync(model.Token, HashPassword(model.Password));

                if (result)
                {
                    TempData["Success"] = "Password reset successfully! You can now log in.";
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid or expired reset link.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                ModelState.AddModelError(string.Empty, "An error occurred. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            try
            {
                var customerId = GetCurrentCustomerId();
                var customer = await _customerService.GetByIdAsync(customerId);

                if (customer == null)
                {
                    return NotFound();
                }

                // Verify current password
                if (!VerifyPassword(currentPassword, customer.PasswordHash))
                {
                    TempData["Error"] = "Current password is incorrect.";
                    return RedirectToAction(nameof(Profile));
                }

                // Validate new password
                if (newPassword != confirmPassword)
                {
                    TempData["Error"] = "New passwords do not match.";
                    return RedirectToAction(nameof(Profile));
                }

                // Update password
                customer.PasswordHash = HashPassword(newPassword);
                await _customerService.UpdateAsync(customer);

                TempData["Success"] = "Password changed successfully!";
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                TempData["Error"] = "An error occurred while changing password.";
                return RedirectToAction(nameof(Profile));
            }
        }

        #endregion

        #region Profile Management

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var customerId = GetCurrentCustomerId();
                var customer = await _customerService.GetByIdAsync(customerId);

                if (customer == null)
                {
                    return NotFound();
                }

                var model = new CustomerProfileViewModel
                {
                    CustomerId = customer.CustomerId,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    PhoneNumber = customer.PhoneNumber,
                    DateOfBirth = customer.DateOfBirth,
                    AvatarUrl = customer.AvatarUrl,
                    IsEmailVerified = customer.IsEmailVerified,
                    MemberSince = customer.CreatedAt,
                    TotalReservations = await _customerService.GetTotalReservationsAsync(customerId),
                    TotalReviews = await _customerService.GetTotalReviewsAsync(customerId),
                    FavoriteRestaurants = await _customerService.GetFavoriteRestaurantsCountAsync(customerId),
                    FavoriteCuisines = customer.FavoriteCuisines?.Split(',').ToList(),
                    DietaryRestrictions = customer.DietaryRestrictions?.Split(',').ToList(),
                    DefaultPartySize = customer.DefaultPartySize ?? 2,
                    TwoFactorEnabled = customer.TwoFactorEnabled
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading profile");
                TempData["Error"] = "An error occurred while loading your profile.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(CustomerProfileViewModel model)
        {
            try
            {
                var customerId = GetCurrentCustomerId();
                var customer = await _customerService.GetByIdAsync(customerId);

                if (customer == null)
                {
                    return NotFound();
                }

                customer.FirstName = model.FirstName;
                customer.LastName = model.LastName;
                customer.Email = model.Email;
                customer.PhoneNumber = model.PhoneNumber;
                customer.DateOfBirth = model.DateOfBirth;

                await _customerService.UpdateAsync(customer);

                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                TempData["Error"] = "An error occurred while updating your profile.";
                return RedirectToAction(nameof(Profile));
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePreferences(string[] cuisines, string[] dietary, int defaultPartySize)
        {
            try
            {
                var customerId = GetCurrentCustomerId();
                var customer = await _customerService.GetByIdAsync(customerId);

                if (customer == null)
                {
                    return NotFound();
                }

                customer.FavoriteCuisines = string.Join(",", cuisines);
                customer.DietaryRestrictions = string.Join(",", dietary);
                customer.DefaultPartySize = defaultPartySize;

                await _customerService.UpdateAsync(customer);

                TempData["Success"] = "Preferences updated successfully!";
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating preferences");
                TempData["Error"] = "An error occurred while updating preferences.";
                return RedirectToAction(nameof(Profile));
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateNotifications(
            bool emailReservationConfirm,
            bool smsReminders,
            bool emailCancellations,
            bool emailPromotions,
            bool emailNewRestaurants,
            bool emailNewsletter)
        {
            try
            {
                var customerId = GetCurrentCustomerId();
                var customer = await _customerService.GetByIdAsync(customerId);

                if (customer == null)
                {
                    return NotFound();
                }

                // Store notification preferences (you may want to create a separate table for this)
                customer.EmailNotifications = emailReservationConfirm || emailCancellations || emailPromotions || emailNewRestaurants || emailNewsletter;
                customer.SmsNotifications = smsReminders;

                await _customerService.UpdateAsync(customer);

                TempData["Success"] = "Notification settings updated successfully!";
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification settings");
                TempData["Error"] = "An error occurred while updating settings.";
                return RedirectToAction(nameof(Profile));
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            try
            {
                if (avatar == null || avatar.Length == 0)
                {
                    return Json(new { success = false, message = "No file uploaded" });
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(avatar.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(extension))
                {
                    return Json(new { success = false, message = "Invalid file type" });
                }

                // Validate file size (max 5MB)
                if (avatar.Length > 5 * 1024 * 1024)
                {
                    return Json(new { success = false, message = "File size exceeds 5MB" });
                }

                var customerId = GetCurrentCustomerId();
                
                // Save file
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{customerId}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await avatar.CopyToAsync(fileStream);
                }

                // Update customer avatar URL
                var customer = await _customerService.GetByIdAsync(customerId);
                customer.AvatarUrl = $"/uploads/avatars/{uniqueFileName}";
                await _customerService.UpdateAsync(customer);

                return Json(new { success = true, avatarUrl = customer.AvatarUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading avatar");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                var customerId = GetCurrentCustomerId();
                
                // Delete customer account (this should handle cascading deletes)
                await _customerService.DeleteAsync(customerId);

                // Sign out
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                TempData["Success"] = "Your account has been deleted.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting account");
                TempData["Error"] = "An error occurred while deleting your account.";
                return RedirectToAction(nameof(Profile));
            }
        }

        #endregion

        #region Helper Methods

        private int GetCurrentCustomerId()
        {
            var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (customerIdClaim != null && int.TryParse(customerIdClaim.Value, out int customerId))
            {
                return customerId;
            }
            throw new UnauthorizedAccessException("Customer not authenticated");
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == hash;
        }

        private string GenerateVerificationToken()
        {
            return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        }

        private string GeneratePasswordResetToken()
        {
            return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        }

        private async Task SendVerificationEmail(string email, string token)
        {
            var verificationUrl = Url.Action(
                nameof(VerifyEmail),
                "Customer",
                new { token },
                Request.Scheme);

            var subject = "Verify Your Email Address";
            var body = $@"
                <h2>Welcome to Restaurant Booking System!</h2>
                <p>Please verify your email address by clicking the link below:</p>
                <p><a href='{verificationUrl}'>Verify Email Address</a></p>
                <p>Or copy and paste this link into your browser:</p>
                <p>{verificationUrl}</p>
                <p>This link will expire in 24 hours.</p>
                <p>If you didn't create an account, please ignore this email.</p>
            ";

            await _emailService.SendEmailAsync(email, subject, body);
        }

        private async Task SendPasswordResetEmail(string email, string token)
        {
            var resetUrl = Url.Action(
                nameof(ResetPassword),
                "Customer",
                new { token },
                Request.Scheme);

            var subject = "Reset Your Password";
            var body = $@"
                <h2>Password Reset Request</h2>
                <p>We received a request to reset your password. Click the link below to reset it:</p>
                <p><a href='{resetUrl}'>Reset Password</a></p>
                <p>Or copy and paste this link into your browser:</p>
                <p>{resetUrl}</p>
                <p>This link will expire in 24 hours.</p>
                <p>If you didn't request a password reset, please ignore this email or contact support if you have concerns.</p>
            ";

            await _emailService.SendEmailAsync(email, subject, body);
        }

        #endregion
    }
}
