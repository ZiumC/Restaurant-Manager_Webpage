using System.ComponentModel.DataAnnotations;

namespace Restaurants_Webpage.Models.CommonModels
{
    public class CommonReservationModel
    {
        public int IdReservation { get; set; }
        public DateTime ReservationDate { get; set; }
        public string Status { get; set; }
        public int? ReservationGrade { get; set; }
        public int HowManyPeoples { get; set; }
        public CommonComplaintModel? ReservationComplaint { get; set; }
    }
}
