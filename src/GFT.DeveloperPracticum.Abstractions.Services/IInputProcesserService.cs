using System;
using System.Collections.Generic;
using ElemarJR.FunctionalCSharp;
using GFT.DeveloperPracticum.Kernel.Enumerators;
using GFT.DeveloperPracticum.Kernel.Models;

namespace GFT.DeveloperPracticum.Abstractions.Services
{
    public interface IInputProcessorService
    {
        Try<ArgumentException, Unit> Setup(IReadOnlyDictionary<(TimeOfDayType, DishType), Dish> menuData);
        Try<Exception, InputProcessResult> Process(string input);
        bool TryAddOrUpdateDishesAmount(Dictionary<(DishType, string), int> dishes, TimeOfDayType timeOfDay, DishType dishType, string dish);
        string ParseTimeOfDay(IList<string> cleanedInputList);
    }
}