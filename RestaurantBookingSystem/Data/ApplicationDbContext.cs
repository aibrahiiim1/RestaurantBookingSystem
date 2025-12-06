using Microsoft.EntityFrameworkCore;
using RestaurantBookingSystem.Models;

namespace RestaurantBookingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Cuisine> Cuisines { get; set; }
        public DbSet<MealType> MealTypes { get; set; }
        public DbSet<Occasion> Occasions { get; set; }
        public DbSet<OpeningTime> OpeningTimes { get; set; }
        public DbSet<RestaurantClosure> RestaurantClosures { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<SpecialEvent> SpecialEvents { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<RestaurantReview> RestaurantReviews { get; set; }
        public DbSet<CustomerAllergy> CustomerAllergies { get; set; }
        public DbSet<FavoriteRestaurant> FavoriteRestaurants { get; set; }
        public DbSet<ReservationMessage> ReservationMessages { get; set; }
        public DbSet<Principal> Principals { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Restaurant configuration
            modelBuilder.Entity<Restaurant>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.PhoneNumber);
                entity.HasIndex(e => new { e.City, e.Area });
                entity.HasIndex(e => e.CuisineId);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.IsFeatured);
                
                entity.HasOne(r => r.Cuisine)
                    .WithMany(c => c.Restaurants)
                    .HasForeignKey(r => r.CuisineId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Table configuration
            modelBuilder.Entity<Table>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.RestaurantId, e.TableNumber }).IsUnique();
                
                entity.HasOne(t => t.Restaurant)
                    .WithMany(r => r.Tables)
                    .HasForeignKey(t => t.RestaurantId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Reservation configuration
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.BookingReference).IsUnique();
                entity.HasIndex(e => new { e.RestaurantId, e.ReservationDate, e.ReservationTime });
                entity.HasIndex(e => e.CustomerId);
                entity.HasIndex(e => e.Status);
                
                entity.HasOne(r => r.Customer)
                    .WithMany(c => c.Reservations)
                    .HasForeignKey(r => r.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(r => r.Restaurant)
                    .WithMany()
                    .HasForeignKey(r => r.RestaurantId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(r => r.Table)
                    .WithMany(t => t.Reservations)
                    .HasForeignKey(r => r.TableId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Customer configuration
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.PhoneNumber);
            });

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                
                entity.HasOne(u => u.Restaurant)
                    .WithMany(r => r.Staff)
                    .HasForeignKey(u => u.RestaurantId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Opening Time configuration
            modelBuilder.Entity<OpeningTime>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.RestaurantId, e.DayOfWeek }).IsUnique();
            });

            // Restaurant Review configuration
            modelBuilder.Entity<RestaurantReview>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.RestaurantId, e.CustomerId });
                entity.HasIndex(e => e.ReviewDate);
            });

            // Favorite Restaurant configuration
            modelBuilder.Entity<FavoriteRestaurant>(entity =>
            {
                entity.HasKey(e => e.FavoriteId);
                entity.HasIndex(e => new { e.CustomerId, e.RestaurantId }).IsUnique();
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, RoleName = "SuperAdmin" },
                new Role { Id = 2, RoleName = "RestaurantAdmin" },
                new Role { Id = 3, RoleName = "Manager" },
                new Role { Id = 4, RoleName = "Staff" }
            );

            // Seed Cuisines
            modelBuilder.Entity<Cuisine>().HasData(
                new Cuisine { Id = 1, Name = "Italian" },
                new Cuisine { Id = 2, Name = "Mexican" },
                new Cuisine { Id = 3, Name = "Chinese" },
                new Cuisine { Id = 4, Name = "Japanese" },
                new Cuisine { Id = 5, Name = "Indian" },
                new Cuisine { Id = 6, Name = "Mediterranean" },
                new Cuisine { Id = 7, Name = "American" },
                new Cuisine { Id = 8, Name = "French" },
                new Cuisine { Id = 9, Name = "Thai" },
                new Cuisine { Id = 10, Name = "Middle Eastern" },
                new Cuisine { Id = 11, Name = "Seafood" },
                new Cuisine { Id = 12, Name = "Steakhouse" },
                new Cuisine { Id = 13, Name = "Vegetarian/Vegan" },
                new Cuisine { Id = 14, Name = "BBQ" },
                new Cuisine { Id = 15, Name = "Fusion" }
            );

            // Seed Meal Types
            modelBuilder.Entity<MealType>().HasData(
                new MealType { Id = 1, Name = "Breakfast", StartTime = new TimeOnly(6, 0), EndTime = new TimeOnly(11, 0) },
                new MealType { Id = 2, Name = "Brunch", StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(14, 0) },
                new MealType { Id = 3, Name = "Lunch", StartTime = new TimeOnly(11, 30), EndTime = new TimeOnly(15, 0) },
                new MealType { Id = 4, Name = "Dinner", StartTime = new TimeOnly(17, 0), EndTime = new TimeOnly(23, 0) },
                new MealType { Id = 5, Name = "Late Night", StartTime = new TimeOnly(22, 0), EndTime = new TimeOnly(2, 0) }
            );

            // Seed Occasions
            modelBuilder.Entity<Occasion>().HasData(
                new Occasion { Id = 1, Name = "Birthday" },
                new Occasion { Id = 2, Name = "Anniversary" },
                new Occasion { Id = 3, Name = "Business Meal" },
                new Occasion { Id = 4, Name = "Date Night" },
                new Occasion { Id = 5, Name = "Wedding" },
                new Occasion { Id = 6, Name = "Celebration" },
                new Occasion { Id = 7, Name = "Family Gathering" },
                new Occasion { Id = 8, Name = "Casual Dining" },
                new Occasion { Id = 9, Name = "Special Event" },
                new Occasion { Id = 10, Name = "Romantic" }
            );
        }
    }
}
