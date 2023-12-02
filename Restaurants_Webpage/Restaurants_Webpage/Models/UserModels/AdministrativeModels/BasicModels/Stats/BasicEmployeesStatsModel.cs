namespace Restaurants_Webpage.Models.UserModels.AdministrativeModels.BasicModels.Stats
{
    public class BasicEmployeesStatsModel
    {
        public decimal TotalSalary { get; set; }
        public decimal TotalBonus { get; set; }
        public int AllEmployees { get; set; }
        public int Owner { get; set; }
        public int Chef { get; set; }
        public int ChefHelper { get; set; }
        public int Waiter { get; set; }
    }
}
