﻿using Restaurants_Webpage.Models.CommonModels;

namespace Restaurants_Webpage.Models.Restaurant
{
    public class RestaurantMenuModel
    {
        public int IdRestaurant { get; set; }
        public string Name { get; set; }
        public AddressModel Address { get; set; }
        public IEnumerable<RestaurantDishModel> Menu { get; set; }
        public int? Grade { get; set; }
    }
}