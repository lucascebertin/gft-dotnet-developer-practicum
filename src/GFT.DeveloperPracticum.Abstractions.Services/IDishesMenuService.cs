using System;
using GFT.DeveloperPracticum.Kernel.Enumerators;
using GFT.DeveloperPracticum.Kernel.Models;

namespace GFT.DeveloperPracticum.Abstractions.Services
{
    public interface IDishesMenuService : IDisposable
    {
        string FindDish(TimeOfDayType timeOfDay, DishType dishType);
        bool IsValidAmount(TimeOfDayType timeOfDay, DishType dishType, int amount);
        bool TryAddDish(Tuple<TimeOfDayType, DishType> id, Dish dish);
        bool HasDishes();
    }
}