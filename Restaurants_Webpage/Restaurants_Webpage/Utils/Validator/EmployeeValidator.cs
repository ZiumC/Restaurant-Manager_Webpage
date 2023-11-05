using Restaurants_Webpage.Models.UserModels.EmployeeModels;

namespace Restaurants_Webpage.Utils.Validator
{
    public class EmployeeValidator
    {
        public static bool IsDefectedEmployee(EmployeeModel employeeModel, IConfiguration config) 
        {
            if (string.IsNullOrEmpty(employeeModel.FirstName)) 
            {
                return true;
            }

            if (string.IsNullOrEmpty(employeeModel.LastName))
            {
                return true;
            }

            var pesel = employeeModel.PESEL;
            if (string.IsNullOrEmpty(pesel))
            {
                return true;
            }

            if (pesel.Count() < 0 || pesel.Count() > 11) 
            {
                return true;
            }

            if (employeeModel.Salary < 0)
            {
                return true;
            }

            decimal result;
            if (decimal.TryParse(config["ApplicationSettings:BasicBonus"], out result)) 
            {
                if (employeeModel.BonusSalary < result)
                {
                    return true;
                }
            }

            if (string.IsNullOrEmpty(employeeModel.Address.City))
            {
                return true;
            }

            if (string.IsNullOrEmpty(employeeModel.Address.Street))
            {
                return true;
            }

            if (string.IsNullOrEmpty(employeeModel.Address.BuildingNumber))
            {
                return true;
            }

            return false;
        }
    }
}
