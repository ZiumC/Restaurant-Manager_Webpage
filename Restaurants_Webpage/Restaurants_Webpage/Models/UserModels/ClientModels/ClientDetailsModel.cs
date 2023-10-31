namespace Restaurants_Webpage.Models.UserModels.ClientModels
{
    public class ClientDetailsModel
    {
        public int IdClient { get; set; }
        public string Name { get; set; }
        public string IsBusinessman { get; set; }
        public IEnumerable<ClientReservationModel>? ClientReservations { get; set; }
    }
}
