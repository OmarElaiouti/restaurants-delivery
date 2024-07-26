using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Restaurant_delivery_online.Models;

namespace Restaurant_delivery_online.Controllers
{
    public class MenuController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MenuController> _logger;

        public MenuController(HttpClient httpClient, ILogger<MenuController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int restaurantId)
        {
            // Call the API to get menu items
            var response = await _httpClient.GetAsync($"https://localhost:7278/api/menu?restaurantId={restaurantId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var menuItems = JsonConvert.DeserializeObject<IEnumerable<MenuItem>>(json);

                ViewBag.RestaurantId = restaurantId;
                ViewBag.City = menuItems?.FirstOrDefault()?.Restaurantcity ?? "city";

                return View(menuItems);
            }

            return RedirectToAction("NotFoundPage", "Home");
        }

    }
}
