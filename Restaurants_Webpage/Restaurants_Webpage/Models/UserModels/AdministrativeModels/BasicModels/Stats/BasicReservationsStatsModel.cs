namespace Restaurants_Webpage.Models.UserModels.AdministrativeModels.BasicModels.Stats
{
    public class BasicReservationsStatsModel
    {
        public int AllReservations { get; set; }
        public int New { get; set; }
        public int Confirmed { get; set; }
        public int Canceled { get; set; }
        public int Rated { get; set; }
    }
}
