namespace Restaurants_Webpage.Models.Restaurant
{
    public class RestaurantModel
    {
        public int IdRestaurant { get; set; }
        public string Name { get; set; }
        public AddressModel Address { get; set; }
        public int MenuCount { get; set; }
        public double? Grade { get; set; }
    }
}
