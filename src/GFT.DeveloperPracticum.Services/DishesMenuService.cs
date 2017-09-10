using GFT.DeveloperPracticum.Abstractions.Services;
using GFT.DeveloperPracticum.Kernel.Enumerators;
using GFT.DeveloperPracticum.Kernel.Models;
using System;
using System.Collections.Generic;

namespace GFT.DeveloperPracticum.Services
{
    public sealed class DishesMenuService : IDishesMenuService
    {
        private bool disposedValue;

        private readonly IDictionary<(TimeOfDayType, DishType), Dish> _dishes =
            new Dictionary<(TimeOfDayType, DishType), Dish>();

        public bool TryAddDish((TimeOfDayType, DishType) id, Dish dish)
        {
            if ( dish == null)
                return false;

            var canAddDish = !_dishes.ContainsKey(id)
                && !_dishes.Values.Contains(dish);

            if (canAddDish)
                _dishes.Add(id, dish);

            return canAddDish;
        }

        public string FindDish(TimeOfDayType timeOfDay, DishType dishType)
        {
            var key = (timeOfDay, dishType);
            var output = string.Empty;

            if (_dishes.ContainsKey(key))
            {
                var dish = _dishes[key];
                output = dish.Name;
            }
            else
                output = string.Empty;

            return output;
        }

        public bool IsValidAmount(TimeOfDayType timeOfDay, DishType dishType, int amount)
        {
            var key =  (timeOfDay, dishType);
            var valid = false;

            if (_dishes.ContainsKey(key) && amount >= 1)
            {
                valid = amount == 1
                    || (amount > 1
                        && _dishes[key].AllowedOrderAmount == AllowedOrderType.Multiple);
            }

            return valid;
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    _dishes.Clear();

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public bool HasDishes()
        {
            return _dishes.Count > 0;
        }
    }
}
