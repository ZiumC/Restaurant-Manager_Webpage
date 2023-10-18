namespace Restaurants_Webpage.Models.DataModels.Index
{
    public class RestaurantIndexModel
    {
        public int IdRestaurant { get; set; }
        public string Name { get; set; }
        public AddressIndexModel Address { get; set; }
        public int MenuCount { get; set; }
        public int? Grade { get; set; }
    }
}
