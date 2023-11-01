using Restaurants_Webpage.Models.CommonModels;

namespace Restaurants_Webpage.Models.UserModels.EmployeeModels
{
    public class EmployeeModel
    {
        public int IdEmployee { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PESEL { get; set; }
        public decimal Salary { get; set; }
        public decimal BonusSalary { get; set; }
        public DateTime HiredDate { get; set; }
        public DateTime? FirstPromotionChefDate { get; set; }
        public string IsOwner { get; set; }
        public AddressModel Address { get; set; }
        public List<EmployeeCertificateModel>? Certificates { get; set; }
    }
}
