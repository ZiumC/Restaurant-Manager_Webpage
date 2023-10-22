namespace Restaurants_Webpage.Models.ClientModels.Restaurant
{
    public class RestaurantModel
    {
        public int IdRestaurant { get; set; }
        public string Name { get; set; }
        public AddressModel Address { get; set; }
        public int MenuCount { get; set; }
        public int? Grade { get; set; }
    }
}
