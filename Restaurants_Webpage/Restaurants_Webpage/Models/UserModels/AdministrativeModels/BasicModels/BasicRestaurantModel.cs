using Restaurants_Webpage.Models.CommonModels;

namespace Restaurants_Webpage.Models.UserModels.AdministrativeModels.BasicModels
{
    public class BasicRestaurantModel
    {
        public int IdRestaurant { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public decimal BonusBudget { get; set; }
        public CommonAddressModel Address { get; set; }
    }
}
