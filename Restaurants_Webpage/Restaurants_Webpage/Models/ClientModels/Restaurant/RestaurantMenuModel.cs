namespace Restaurants_Webpage.Models.ClientModels.Restaurant
{
    public class RestaurantMenuModel
    {
        public int IdRestaurant { get; set; }
        public string Name { get; set; }
        public AddressModel Address { get; set; }
        public IEnumerable<RestaurantDishModel> Menu { get; set; }
        public int? Grade { get; set; }
    }
}
