using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using RestaurantBookingSystem.Data;
using RestaurantBookingSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Add Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie("CustomerAuth", options =>
    {
        options.LoginPath = "/Customer/Login";
        options.LogoutPath = "/Customer/Logout";
        options.AccessDeniedPath = "/Customer/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    })
    .AddCookie("AdminAuth", options =>
    {
        options.LoginPath = "/Admin/Login";
        options.LogoutPath = "/Admin/Logout";
        options.AccessDeniedPath = "/Admin/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Configure routes
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{action=Dashboard}/{id?}",
    defaults: new { controller = "Admin" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        
        // Seed initial admin user if none exists
        if (!context.Users.Any())
        {
            var authService = services.GetRequiredService<IAuthenticationService>();
            var adminUser = new RestaurantBookingSystem.Models.User
            {
                Username = "admin",
                PasswordHash = authService.HashPassword("Admin@123"),
                Email = "admin@restaurant.com",
                FullName = "System Administrator",
                RoleId = 1, // SuperAdmin
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
            
            // Create a default restaurant for the admin
            var defaultRestaurant = new RestaurantBookingSystem.Models.Restaurant
            {
                Name = "Default Restaurant",
                Description = "This is a default restaurant. Please update with your details.",
                AddressLine1 = "123 Main Street",
                City = "New York",
                Area = "Manhattan",
                PostalCode = "10001",
                Country = "USA",
                PhoneNumber = "+1234567890",
                Email = "contact@defaultrestaurant.com",
                CuisineId = 1,
                TotalSeatingCapacity = 50,
                IsActive = true,
                IsFeatured = false,
                CreatedDate = DateTime.UtcNow,
                AverageRating = 0,
                TotalReviews = 0,
                IsChildFriendly = true,
                HasWheelchairAccess = true,
                DefaultBookingDurationMinutes = 120,
                TimeSlotIntervalMinutes = 30,
                CancellationPolicyHours = 5
            };
            
            context.Restaurants.Add(defaultRestaurant);
            context.SaveChanges();
            
            adminUser.RestaurantId = defaultRestaurant.Id;
            context.Users.Add(adminUser);
            context.SaveChanges();
            
            Console.WriteLine("Default admin user created:");
            Console.WriteLine("Username: admin");
            Console.WriteLine("Password: Admin@123");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.Run();
