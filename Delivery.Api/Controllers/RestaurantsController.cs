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
    public class RestaurantsController : ControllerBase
    {
        private readonly RestaurantDeliveryDbContext _context;
        private readonly IMapper _mapper;

        public RestaurantsController(RestaurantDeliveryDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet("bycity")]
        public async Task<IActionResult> GetRestaurantsByCity(int cityId)
        {
            var restaurants = await _context.Restaurants.Include(r=>r.City)
                .Where(r => r.CityId == cityId)
                .ToListAsync();
            var restaurantsFound = _mapper.Map<List<RestaurantDto>>(restaurants);

            return Ok(restaurantsFound);
        }


        [HttpGet("cities")]
        public async Task<IActionResult> GetCities()
        {
            var cities = await _context.Cities 
                .ToListAsync();
            var citiesFound = _mapper.Map<List<CityDto>>(cities);

            return Ok(citiesFound);
        }


    }
}
