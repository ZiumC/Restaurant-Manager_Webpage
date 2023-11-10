namespace Restaurants_Webpage.Models.UserModels.AdministrativeModels.ExtendedModels
{
    public class ExtendedDishModel
    {
        public int IdDish { get; set; }
        public string DishName { get; set; }
        public decimal DishPrice { get; set; }
        public IEnumerable<string> Restaurants { get; set; }
    }
}
