using Microsoft.AspNetCore.Mvc;
using RestaurantBookingSystem.Services;
using RestaurantBookingSystem.ViewModels;
using System.Diagnostics;

namespace RestaurantBookingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRestaurantService _restaurantService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IRestaurantService restaurantService,
            ILogger<HomeController> logger)
        {
            _restaurantService = restaurantService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                FeaturedRestaurants = await _restaurantService.GetFeaturedRestaurantsAsync(6),
                Cuisines = await _restaurantService.GetAllCuisinesAsync(),
                Search = new SearchViewModel
                {
                    Date = DateTime.Today.AddDays(1),
                    PartySize = 2
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Search(SearchViewModel search)
        {
            return RedirectToAction("SearchResults", "Restaurant", search);
        }

        public IActionResult HowItWorks()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
