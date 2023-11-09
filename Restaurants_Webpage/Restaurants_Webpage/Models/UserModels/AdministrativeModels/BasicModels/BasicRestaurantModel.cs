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
        public int DishesCount { get; set; }
        public int WorkersCount { get; set; }
        public int TotalReservationsCount { get; set; }
        public int NewReservationsCount { get; set; }
        public int CanceledReservationsCount { get; set; }
        public int RatedReservationsCount { get; set; }
    }
}
