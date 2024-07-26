using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Restaurant_delivery_online.Models;
using System.Text;

namespace Restaurant_delivery_online.Controllers
{
    public class OrderController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IDistributedCache _cache;

        public OrderController(IHttpClientFactory clientFactory, IDistributedCache cache)
        {
            _clientFactory = clientFactory;
            _cache = cache;
        }


        [HttpPost]
        public IActionResult Confirm(List<int> selectedItems, int restaurantId)
        {
            if (selectedItems == null || !selectedItems.Any())
            {
                return BadRequest("Please select at least one item to continue.");
            }

            TempData["SelectedItems"] = JsonConvert.SerializeObject(selectedItems);
            TempData["restaurantId"] = restaurantId;
            _cache.SetString("SelectedItems", JsonConvert.SerializeObject(selectedItems));
            _cache.SetString("RestaurantId", restaurantId.ToString());

            return View("CustomerDetails");
        }

        [HttpGet]
        public IActionResult Confirm()
        {
            // Retrieve cached data
            var selectedItemsJson = _cache.GetString("SelectedItems");
            var restaurantIdString = _cache.GetString("RestaurantId");

            if (string.IsNullOrEmpty(selectedItemsJson) || string.IsNullOrEmpty(restaurantIdString))
            {
                // Handle the case where there is no data in the cache
                return RedirectToAction("Index", "Home"); // Or any other appropriate action
            }

            var selectedItems = JsonConvert.DeserializeObject<List<int>>(selectedItemsJson);
            var restaurantId = int.Parse(restaurantIdString);

            // Store the data in TempData for the CustomerDetails view
            TempData["SelectedItems"] = selectedItemsJson;
            TempData["restaurantId"] = restaurantId;

            return RedirectToAction("CustomerDetails");
        }

        [HttpGet]
        public IActionResult CustomerDetails()
        {
            var selectedItemsJson = _cache.GetString("SelectedItems");
            var restaurantId = _cache.GetString("RestaurantId");

            TempData["SelectedItems"] = selectedItemsJson;
            TempData["restaurantId"] = int.Parse(restaurantId);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CustomerDetails(Order order)
        {
            if (!ModelState.IsValid)
            {
                return View(order);
            }

            // Get selected items from TempData or cache
            var selectedItemsJson = TempData["SelectedItems"] as string
                ?? _cache.GetString("SelectedItems");
            var selectedItemIds = JsonConvert.DeserializeObject<List<int>>(selectedItemsJson);

            if (selectedItemIds == null || !selectedItemIds.Any())
            {
                ModelState.AddModelError(string.Empty, "No items selected.");
                return View(order);
            }

            // Get restaurant ID from TempData or cache
            var restaurantId = TempData["restaurantId"] as int?
                               ?? int.Parse(_cache.GetString("RestaurantId"));

            // Fetch menu item details from the API
            var client = _clientFactory.CreateClient();
            var itemsDetails = new List<MenuItem>();

            foreach (var itemId in selectedItemIds)
            {
                var response = await client.GetAsync($"https://localhost:7278/api/menu/byId?itemId={itemId}");
                if (response.IsSuccessStatusCode)
                {
                    var item = await response.Content.ReadAsAsync<MenuItem>();
                    itemsDetails.Add(item);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Failed to retrieve details for item ID {itemId}.");
                    return View(order);
                }
            }

            TempData["CustomerName"] = order.CustomerName;
            TempData["CustomerEmail"] = order.CustomerEmail;
            TempData["CustomerPhone"] = order.CustomerPhone;
            TempData["CustomerAddress"] = order.CustomerAddress;
            TempData["ItemsDetails"] = JsonConvert.SerializeObject(itemsDetails);
            _cache.SetString("ItemsDetails", JsonConvert.SerializeObject(itemsDetails));

            return RedirectToAction("OrderConfirmation");
        }

        [HttpGet]

        public IActionResult OrderConfirmation()
        {
            var customerName = TempData["CustomerName"] as string;
            var customerEmail = TempData["CustomerEmail"] as string;
            var customerPhone = TempData["CustomerPhone"] as string;
            var customerAddress = TempData["CustomerAddress"] as string;
            var itemsDetailsJson = TempData["ItemsDetails"] as string;

            var itemsDetailsCache = _cache.GetString("ItemsDetails");
            if (string.IsNullOrEmpty(itemsDetailsCache))
            {
                // Check if itemsDetailsJson is not null or empty before deserialization
                if (!string.IsNullOrEmpty(itemsDetailsJson))
                {
                    itemsDetailsCache = JsonConvert.DeserializeObject<string>(itemsDetailsJson);
                    _cache.SetString("ItemsDetails", itemsDetailsCache);
                }
            }
            ViewBag.CustomerName = customerName;
            ViewBag.CustomerEmail = customerEmail;
            ViewBag.CustomerPhone = customerPhone;
            ViewBag.CustomerAddress = customerAddress;
            ViewBag.ItemsDetails = JsonConvert.DeserializeObject<List<MenuItem>>(itemsDetailsCache);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitOrder([FromBody] Order order)
        {
            // API endpoint URL
            var apiUrl = "https://localhost:7278/api/order/confirm";

            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // Create a mock response or get actual data from API
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var apiResult = JsonConvert.DeserializeObject<OrderConfirmationResult>(apiResponse);

                    // Ensure you return the required properties
                    return Json(new
                    {
                        success = true,
                        customerName = apiResult.CustomerName,
                        orderId = apiResult.OrderId,
                        totalAmount = apiResult.TotalAmount,
                        orderItems = apiResult.OrderItems
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to submit order. Please try again." });
                }
            }
        }





    }
}


