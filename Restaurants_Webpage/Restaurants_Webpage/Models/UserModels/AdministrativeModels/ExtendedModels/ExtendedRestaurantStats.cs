using Restaurants_Webpage.Models.UserModels.AdministrativeModels.BasicModels.Stats;

namespace Restaurants_Webpage.Models.UserModels.AdministrativeModels.ExtendedModels
{
    public class ExtendedRestaurantStats
    {
        public int idRestaurant { get; set; }
        public string Name { get; set; }
        public double? Grade { get; set; }
        public BasicReservationsStatsModel Reservations { get; set; }
        public BasicComplaintsStatsModel Complaints { get; set; }
        public BasicEmployeesStatsModel Employees { get; set; }
    }
}
