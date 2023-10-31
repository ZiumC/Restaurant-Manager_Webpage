using System.ComponentModel.DataAnnotations;

namespace Restaurants_Webpage.Models.UserModels
{
    public class AccountRegisterModel
    {
        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string login { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string email { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string password1 { get; set; }
        public string password2 { get; set; }
        public string registerMeAsEmployee { get; set; }
        public string pesel { get; set; }
        public DateTime hiredDate { get; set; }
    }
}
