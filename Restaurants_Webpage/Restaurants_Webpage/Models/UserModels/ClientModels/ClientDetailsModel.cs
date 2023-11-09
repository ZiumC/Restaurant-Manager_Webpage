using Restaurants_Webpage.Models.CommonModels;

namespace Restaurants_Webpage.Models.UserModels.ClientModels
{
    public class ClientDetailsModel
    {
        public int IdClient { get; set; }
        public string Name { get; set; }
        public string IsBusinessman { get; set; }
        public IEnumerable<CommonReservationModel>? ClientReservations { get; set; }
    }
}
