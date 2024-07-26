using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Restaurant_delivery_online.Models;
using System.Diagnostics;

namespace Restaurant_delivery_online.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
           
            var response = await _httpClient.GetStringAsync("https://localhost:7278/api/restaurants/cities");

            var cities = JsonConvert.DeserializeObject<List<City>>(response);

       
            if (cities == null)
            {
                
                return View("ServerErrorPage");
            }

           

            return View(cities);
        }

        public IActionResult ServerErrorPage()
        {
            return View();
        }

       
    }
}
