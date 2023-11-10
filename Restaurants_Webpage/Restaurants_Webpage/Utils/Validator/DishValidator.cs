using Restaurants_Webpage.Models.UserModels.AdministrativeModels.BasicModels;

namespace Restaurants_Webpage.Utils.Validator
{
    public class DishValidator
    {
        public static bool IsDefectedDish(BasicDishModel dish)
        {

            if (string.IsNullOrEmpty(dish.Name))
            {
                return true;
            }

            if (dish.Price <= 0)
            {
                return true;
            }

            return false;
        }
    }
}
