using Microsoft.EntityFrameworkCore;
using RestaurantBookingSystem.Data;
using RestaurantBookingSystem.Models;
using RestaurantBookingSystem.ViewModels;

namespace RestaurantBookingSystem.Services
{
    public interface IRestaurantService
    {
        Task<List<RestaurantCardViewModel>> GetFeaturedRestaurantsAsync(int count = 6);
        Task<List<RestaurantCardViewModel>> SearchRestaurantsAsync(SearchViewModel search, string? cuisine, string? filter, string sortBy);
        Task<RestaurantDetailViewModel> GetRestaurantDetailAsync(int restaurantId);
        Task<Restaurant> GetRestaurantByIdAsync(int id);
        Task<List<Restaurant>> GetAllRestaurantsAsync();
        Task<List<Restaurant>> GetRestaurantsByBrandAsync(string brandName);
        Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant);
        Task<bool> UpdateRestaurantAsync(Restaurant restaurant);
        Task<bool> DeleteRestaurantAsync(int id);
        Task<List<Cuisine>> GetAllCuisinesAsync();
        Task UpdateRestaurantRatingAsync(int restaurantId);
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly ApplicationDbContext _context;

        public RestaurantService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RestaurantCardViewModel>> GetFeaturedRestaurantsAsync(int count = 6)
        {
            var restaurants = await _context.Restaurants
                .Include(r => r.Cuisine)
                .Include(r => r.Photos)
                .Where(r => r.IsActive && r.IsFeatured)
                .OrderByDescending(r => r.AverageRating)
                .Take(count)
                .Select(r => new RestaurantCardViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    CuisineName = r.Cuisine.Name,
                    City = r.City,
                    Area = r.Area,
                    PrimaryPhotoUrl = r.Photos.FirstOrDefault(p => p.IsPrimary) != null 
                        ? r.Photos.FirstOrDefault(p => p.IsPrimary).Url 
                        : r.Photos.FirstOrDefault() != null ? r.Photos.FirstOrDefault().Url : "/images/default-restaurant.jpg",
                    AverageRating = r.AverageRating,
                    TotalReviews = r.TotalReviews,
                    PriceRange = GetPriceRange(r.MinimumCharge),
                    IsChildFriendly = r.IsChildFriendly,
                    HasOutdoorSeating = r.HasOutdoorSeating,
                    IsFeatured = r.IsFeatured,
                    CreatedDate = r.CreatedDate
                })
                .ToListAsync();

            return restaurants;
        }

        public async Task<List<RestaurantCardViewModel>> SearchRestaurantsAsync(
            SearchViewModel search, 
            string? cuisine, 
            string? filter, 
            string sortBy)
        {
            var query = _context.Restaurants
                .Include(r => r.Cuisine)
                .Include(r => r.Photos)
                .Where(r => r.IsActive);

            // Location filter
            if (!string.IsNullOrEmpty(search.Location))
            {
                query = query.Where(r => 
                    r.City.Contains(search.Location) || 
                    r.Area.Contains(search.Location));
            }

            // Cuisine filter
            if (!string.IsNullOrEmpty(cuisine))
            {
                query = query.Where(r => r.Cuisine.Name == cuisine);
            }

            // Apply filters
            if (!string.IsNullOrEmpty(filter))
            {
                query = filter.ToLower() switch
                {
                    "child-friendly" => query.Where(r => r.IsChildFriendly),
                    "romantic" => query.Where(r => r.SpecialEvents.Any(e => e.EventName.Contains("Romantic"))),
                    "birthday" => query.Where(r => r.SpecialEvents.Any(e => e.EventName.Contains("Birthday"))),
                    "bar" => query.Where(r => r.HasBarArea),
                    "outdoor" => query.Where(r => r.HasOutdoorSeating),
                    "wheelchair" => query.Where(r => r.HasWheelchairAccess),
                    _ => query
                };
            }

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "featured" => query.OrderByDescending(r => r.IsFeatured).ThenByDescending(r => r.AverageRating),
                "newest" => query.OrderByDescending(r => r.CreatedDate),
                "rating" => query.OrderByDescending(r => r.AverageRating),
                "reviews" => query.OrderByDescending(r => r.TotalReviews),
                "name" => query.OrderBy(r => r.Name),
                _ => query.OrderByDescending(r => r.IsFeatured).ThenByDescending(r => r.AverageRating)
            };

            var restaurants = await query
                .Select(r => new RestaurantCardViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    CuisineName = r.Cuisine.Name,
                    City = r.City,
                    Area = r.Area,
                    PrimaryPhotoUrl = r.Photos.FirstOrDefault(p => p.IsPrimary) != null 
                        ? r.Photos.FirstOrDefault(p => p.IsPrimary).Url 
                        : r.Photos.FirstOrDefault() != null ? r.Photos.FirstOrDefault().Url : "/images/default-restaurant.jpg",
                    AverageRating = r.AverageRating,
                    TotalReviews = r.TotalReviews,
                    PriceRange = GetPriceRange(r.MinimumCharge),
                    IsChildFriendly = r.IsChildFriendly,
                    HasOutdoorSeating = r.HasOutdoorSeating,
                    IsFeatured = r.IsFeatured,
                    CreatedDate = r.CreatedDate
                })
                .ToListAsync();

            return restaurants;
        }

        public async Task<RestaurantDetailViewModel> GetRestaurantDetailAsync(int restaurantId)
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.Cuisine)
                .Include(r => r.Photos)
                .Include(r => r.OpeningTimes)
                .Include(r => r.SpecialEvents)
                .Include(r => r.Promotions)
                .Include(r => r.Tables)
                .FirstOrDefaultAsync(r => r.Id == restaurantId);

            if (restaurant == null)
                return null;

            var reviews = await _context.RestaurantReviews
                .Include(r => r.Customer)
                .Where(r => r.RestaurantId == restaurantId)
                .OrderByDescending(r => r.ReviewDate)
                .Take(10)
                .Select(r => new RestaurantReviewViewModel
                {
                    Id = r.Id,
                    Title = r.Title,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    FoodRating = r.FoodRating,
                    ServiceRating = r.ServiceRating,
                    AmbianceRating = r.AmbianceRating,
                    ReviewDate = r.ReviewDate,
                    CustomerName = r.Customer.FirstName + " " + r.Customer.LastName.Substring(0, 1) + ".",
                    RestaurantName = restaurant.Name,
                    IsVerified = r.IsVerified
                })
                .ToListAsync();

            var availableLocations = restaurant.Tables
                .Select(t => t.Location)
                .Distinct()
                .ToList();

            return new RestaurantDetailViewModel
            {
                Restaurant = restaurant,
                Photos = restaurant.Photos.OrderBy(p => p.DisplayOrder).ToList(),
                Reviews = reviews,
                OpeningTimes = restaurant.OpeningTimes.OrderBy(o => o.DayOfWeek).ToList(),
                UpcomingEvents = restaurant.SpecialEvents.Where(e => e.StartDate >= DateTime.Today).ToList(),
                ActivePromotions = restaurant.Promotions.Where(p => p.IsActive && p.StartDate <= DateTime.Today && p.EndDate >= DateTime.Today).ToList(),
                BookingForm = new BookingFormViewModel
                {
                    RestaurantId = restaurantId,
                    Date = DateTime.Today.AddDays(1),
                    PartySize = 2
                },
                AvailableTableLocations = availableLocations
            };
        }

        public async Task<Restaurant> GetRestaurantByIdAsync(int id)
        {
            return await _context.Restaurants
                .Include(r => r.Cuisine)
                .Include(r => r.Photos)
                .Include(r => r.OpeningTimes)
                .Include(r => r.Tables)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Restaurant>> GetAllRestaurantsAsync()
        {
            return await _context.Restaurants
                .Include(r => r.Cuisine)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<List<Restaurant>> GetRestaurantsByBrandAsync(string brandName)
        {
            return await _context.Restaurants
                .Include(r => r.Cuisine)
                .Where(r => r.BrandName == brandName)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant)
        {
            restaurant.CreatedDate = DateTime.UtcNow;
            restaurant.IsActive = true;
            
            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();
            
            return restaurant;
        }

        public async Task<bool> UpdateRestaurantAsync(Restaurant restaurant)
        {
            try
            {
                _context.Restaurants.Update(restaurant);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteRestaurantAsync(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
                return false;

            restaurant.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Cuisine>> GetAllCuisinesAsync()
        {
            return await _context.Cuisines.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task UpdateRestaurantRatingAsync(int restaurantId)
        {
            var reviews = await _context.RestaurantReviews
                .Where(r => r.RestaurantId == restaurantId)
                .ToListAsync();

            if (reviews.Any())
            {
                var restaurant = await _context.Restaurants.FindAsync(restaurantId);
                if (restaurant != null)
                {
                    restaurant.AverageRating = Math.Round((decimal)reviews.Average(r => r.Rating), 1);
                    restaurant.TotalReviews = reviews.Count;
                    await _context.SaveChangesAsync();
                }
            }
        }

        private static string GetPriceRange(decimal? minimumCharge)
        {
            if (!minimumCharge.HasValue || minimumCharge == 0)
                return "$";
            
            return minimumCharge switch
            {
                <= 20 => "$",
                <= 50 => "$$",
                <= 100 => "$$$",
                _ => "$$$$"
            };
        }
    }
}
