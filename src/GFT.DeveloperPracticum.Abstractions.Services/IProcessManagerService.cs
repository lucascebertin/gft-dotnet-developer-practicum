using System;
using System.Collections.Generic;
using ElemarJR.FunctionalCSharp;
using GFT.DeveloperPracticum.Kernel.Enumerators;
using GFT.DeveloperPracticum.Kernel.Models;

namespace GFT.DeveloperPracticum.Abstractions.Services
{
    public interface IProcessManagerService
    {
        Try<Exception, string> Process(IReadOnlyDictionary<(TimeOfDayType, DishType), Dish> menu, string input);
    }
}