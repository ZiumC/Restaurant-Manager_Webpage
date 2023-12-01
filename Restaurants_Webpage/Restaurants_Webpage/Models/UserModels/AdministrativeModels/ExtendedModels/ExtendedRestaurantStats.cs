using Restaurants_Webpage.Models.UserModels.AdministrativeModels.BasicModels.Stats;

namespace Restaurants_Webpage.Models.UserModels.AdministrativeModels.ExtendedModels
{
    public class ExtendedRestaurantStats
    {
        public string RestaurantName { get; set; }
        public double? RestaurantGrade { get; set; }
        public int ReservationsCount { get; set; }
        public BasicComplaintsStatsModel Complaints { get; set; }
        public BasicEmployeesStatsModel Employees { get; set; }
    }
}
