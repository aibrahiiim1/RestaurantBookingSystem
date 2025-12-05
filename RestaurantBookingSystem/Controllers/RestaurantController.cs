using Microsoft.AspNetCore.Mvc;
using RestaurantBookingSystem.Services;
using RestaurantBookingSystem.ViewModels;

namespace RestaurantBookingSystem.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IReservationService _reservationService;

        public RestaurantController(
            IRestaurantService restaurantService,
            IReservationService reservationService)
        {
            _restaurantService = restaurantService;
            _reservationService = reservationService;
        }

        public async Task<IActionResult> SearchResults(SearchViewModel search, string? cuisine, string? filter, string sortBy = "featured")
        {
            var viewModel = new RestaurantSearchResultsViewModel
            {
                Restaurants = await _restaurantService.SearchRestaurantsAsync(search, cuisine, filter, sortBy),
                Search = search,
                SelectedCuisine = cuisine,
                SelectedFilter = filter,
                SortBy = sortBy
            };

            viewModel.TotalResults = viewModel.Restaurants.Count;

            ViewBag.Cuisines = await _restaurantService.GetAllCuisinesAsync();

            return View(viewModel);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var viewModel = await _restaurantService.GetRestaurantDetailAsync(id);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GetAvailableTimeSlots(int restaurantId, DateTime date, int partySize)
        {
            var timeSlots = await _reservationService.GetAvailableTimeSlotsAsync(restaurantId, date, partySize);
            
            return Json(new { success = true, timeSlots });
        }

        public async Task<IActionResult> BrowseByLocation(string city)
        {
            var search = new SearchViewModel
            {
                Location = city,
                PartySize = 2
            };

            return RedirectToAction("SearchResults", search);
        }

        public async Task<IActionResult> BrowseByCuisine(string cuisine)
        {
            var search = new SearchViewModel { PartySize = 2 };
            return RedirectToAction("SearchResults", new { search, cuisine });
        }
    }
}
