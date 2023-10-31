namespace Restaurants_Webpage.Models.UserModels.ClientModels
{
    public class ClientComplaintModel
    {
        public int IdComplaint { get; set; }
        public DateTime ComplaintDate { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
