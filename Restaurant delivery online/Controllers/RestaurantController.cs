using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Restaurant_delivery_online.Models;
using System.Net.Http;

namespace Restaurant_delivery_online.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RestaurantController> _logger;

        public RestaurantController(HttpClient httpClient, ILogger<RestaurantController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchResults(int cityId)
        {
            var response = await _httpClient.GetStringAsync($"https://localhost:7278/api/Restaurants/bycity?cityId={cityId}");
            var restaurants = JsonConvert.DeserializeObject<List<Restaurant>>(response);
            return View(restaurants);
        }
    }
}
