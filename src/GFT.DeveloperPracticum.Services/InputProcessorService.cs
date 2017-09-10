using ElemarJR.FunctionalCSharp;
using GFT.DeveloperPracticum.Abstractions.Services;
using GFT.DeveloperPracticum.Kernel.Enumerators;
using GFT.DeveloperPracticum.Kernel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GFT.DeveloperPracticum.Services
{
    public sealed class InputProcessorService : IInputProcessorService
    {
        private readonly IDishesMenuService _dishesMenuService;
        private readonly IList<string> _availableTimesOfDay;

        public InputProcessorService(IDishesMenuService dishesMenuService)
        {
            _dishesMenuService = dishesMenuService
                ?? throw new ArgumentNullException(nameof(dishesMenuService));

            _availableTimesOfDay = Enum.GetNames(typeof(TimeOfDayType)).ToList();
        }

        public Try<ArgumentException, Unit> Setup(
            IReadOnlyDictionary<(TimeOfDayType, DishType), Dish> menuData
        )
        {
            if (menuData == null || menuData.Count == 0)
                return new ArgumentException("Menu data is null");

            if (menuData != null && menuData.Count > 0)
            {
                foreach (var menuItem in menuData)
                {
                    if (!_dishesMenuService.TryAddDish(menuItem.Key, menuItem.Value))
                    {
                        var builder = new StringBuilder();

                        builder.Append($"Duplicated dish found, time of day: {menuItem.Key.Item1}, ");
                        builder.Append($"dish type: {menuItem.Key.Item2}, ");
                        builder.Append($"dish: {menuItem.Value.Name}, ");
                        builder.Append($"allowed orders amount: {menuItem.Value.AllowedOrderAmount}");

                        return new ArgumentException(builder.ToString());
                    }
                }
            }

            return new Unit();
        }

        public Try<Exception, InputProcessResult> Process(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new ArgumentNullException("Input is empty, provide arguments starting with time of day and after a comma delimited dish types.");

            if (!_dishesMenuService.HasDishes())
                return new InvalidOperationException("Menu data is empty, call Setup and provide a valid menu data collection first.");

            var hasInvalidInput = false;
            var timeOfDay = default(TimeOfDayType);
            var parsedInput = new Dictionary<(DishType, string), int>();

            var slicedInput = input.Split(
                new char[] { ',' },
                StringSplitOptions.RemoveEmptyEntries);

            if (slicedInput != null && slicedInput.Length > 1)
            {
                var cleanedInputList = slicedInput.Select(p => p.Trim()).ToList();
                var timeOfDayFound = ParseTimeOfDay(cleanedInputList);

                hasInvalidInput = (string.IsNullOrEmpty(timeOfDayFound)
                    || !Enum.TryParse(timeOfDayFound, out timeOfDay));

                for (int i = 1; i < cleanedInputList.Count && !hasInvalidInput; i++)
                {
                    var item = cleanedInputList[i];

                    hasInvalidInput = !int.TryParse(item, out int identifierDishType)
                        || !Enum.IsDefined(typeof(DishType), identifierDishType);

                    if (!hasInvalidInput)
                    {
                        var dishType = (DishType)identifierDishType;
                        var dish = _dishesMenuService.FindDish(timeOfDay, dishType);

                        hasInvalidInput = string.IsNullOrEmpty(dish);

                        if (!hasInvalidInput)
                        {
                            hasInvalidInput = !TryAddOrUpdateDishesAmount(
                                parsedInput,
                                timeOfDay,
                                dishType,
                                dish
                            );
                        }
                    }
                }
            }
            else
            {
                return new InvalidOperationException("You must enter a comma delimited list of dish types with at least one selection.");
            }

            return new InputProcessResult(
                hasInvalidInput,
                parsedInput
            );
        }

        public string ParseTimeOfDay(IList<string> cleanedInputList)
        {
            if (cleanedInputList == null || cleanedInputList.Count == 0)
                return null;

            var firstInput = cleanedInputList.First();

            return _availableTimesOfDay.FirstOrDefault(
                str => string.Compare(
                    firstInput,
                    str,
                    StringComparison.CurrentCultureIgnoreCase
                ) == 0
            );
        }

        public bool TryAddOrUpdateDishesAmount(
            Dictionary<(DishType, string), int> dishes,
            TimeOfDayType timeOfDay,
            DishType dishType,
            string dish
        )
        {
            if (dishes == null || string.IsNullOrWhiteSpace(dish))
                return false;

            var hasValidAmount = true;
            var key =(dishType, dish);

            if (dishes.ContainsKey(key))
            {
                var next = dishes[key] + 1;

                hasValidAmount = _dishesMenuService.IsValidAmount(
                    timeOfDay,
                    dishType,
                    next
                );

                if (hasValidAmount)
                    dishes[key]++;
            }
            else
            {
                dishes.Add(key, 1);
            }

            return hasValidAmount;
        }
    }
}
