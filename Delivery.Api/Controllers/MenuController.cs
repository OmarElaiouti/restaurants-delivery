using AutoMapper;
using Delivery.Api.Dtos;
using Delivery.Api.Models.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly RestaurantDeliveryDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderController> _logger;
        public MenuController(RestaurantDeliveryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuByRestaurantId(int restaurantId)
        {
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);
            if (restaurant == null)
            {
                return NotFound();
            }

            var menuItems = await _context.MenuItems
                .Include(mi=>mi.Restaurant)
                .ThenInclude(r=>r.City)
                .Where(m => m.RestaurantId == restaurantId)
                .ToListAsync();

            var menuToSend = _mapper.Map<List<MenuItemDto>>(menuItems);
            menuToSend.ForEach(mi => mi.Restaurantcity = restaurant.City.CityName);

            return Ok(menuToSend); 
        }


        [HttpGet("byId")]
        public async Task<IActionResult> GetMenuItemById(int ItemId)
        {
            var menuItem = await _context.MenuItems.FindAsync(ItemId);

            return Ok(menuItem);
        }
    }
}
