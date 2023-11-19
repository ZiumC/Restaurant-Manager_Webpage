using Restaurants_Webpage.Models.CommonModels;

namespace Restaurants_Webpage.Models.UserModels.ClientModels.ClientRestaurantModels
{
    public class RestaurantMenuModel
    {
        public int IdRestaurant { get; set; }
        public string Name { get; set; }
        public CommonAddressModel Address { get; set; }
        public IEnumerable<RestaurantDishModel> Menu { get; set; }
        public double? Grade { get; set; }
    }
}
