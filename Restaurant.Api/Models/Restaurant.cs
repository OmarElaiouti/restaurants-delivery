namespace Restaurant.Api.Models
{
    public class Restaurant
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public ICollection<MenuItem> MenuItems { get; set; }
    }

}
