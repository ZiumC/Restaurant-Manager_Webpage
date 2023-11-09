using Restaurants_Webpage.Models.CommonModels;
using Restaurants_Webpage.Models.UserModels.AdministrativeModels.BasicModels;

namespace Restaurants_Webpage.Models.UserModels.AdministrativeModels.ExtendedModels
{
    public class ExtendedRestaurantModel
    {
        public int IdRestaurant { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public decimal BonusBudget { get; set; }
        public CommonAddressModel Address { get; set; }
        public IEnumerable<ExtendedDishModel> RestaurantDishes { get; set; }
        public IEnumerable<BasicEmployeeModel> RestaurantWorkers { get; set; }
    }
}
