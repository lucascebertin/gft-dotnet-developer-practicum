using FluentAssertions;
using GFT.DeveloperPracticum.Kernel.Enumerators;
using GFT.DeveloperPracticum.Kernel.Models;
using GFT.DeveloperPracticum.Services;
using System;
using Xunit;

namespace GFT.DeveloperPracticum.UnitTests
{
    public class DishesMenuServiceTests
    {
        [Fact]
        public void Should_allow_to_add_a_menu_dish_with_timeOfDay_and_dishType_when_empty()
        {
            //arrange
            using (var dishesMenuService = new DishesMenuService())
            {
                var tuple = (TimeOfDayType.Morning, DishType.Entree);
                var dish = new Dish("eggs", AllowedOrderType.Single);

                //act
                var addDishResult = dishesMenuService.TryAddDish(tuple, dish);

                //assert
                addDishResult.Should().BeTrue("Because there is no other dish that could cause a duplicated key");
            }
        }

        [Fact]
        public void Should_not_allow_to_add_a_menu_dish_with_timeOfDay_and_dishType_when_its_already_added()
        {
            //arrange
            using (var dishesMenuService = new DishesMenuService())
            {
                var tuple =(TimeOfDayType.Morning, DishType.Entree);
                var dish = new Dish("eggs", AllowedOrderType.Single);

                //act
                dishesMenuService.TryAddDish(tuple, dish);
                var addDishResult = dishesMenuService.TryAddDish(tuple, dish);

                //assert
                addDishResult.Should().BeFalse("Because the dish is already there");
            }
        }

        [Fact]
        public void Should_not_allow_to_add_a_null_value_for_dish()
        {
            //arrange
            using (var dishesMenuService = new DishesMenuService())
            {
                var tuple = (TimeOfDayType.Morning, DishType.Entree);

                //act
                var addDishResult = dishesMenuService.TryAddDish(tuple, null);

                //assert
                addDishResult.Should().BeFalse("Because the dish will be rejected and will not be added to the dictionary");
            }
        }



        [Fact]
        public void Should_has_dishes_when_a_dish_is_added()
        {
            //arrange
            using (var dishesMenuService = new DishesMenuService())
            {
                var tuple = (TimeOfDayType.Morning, DishType.Entree);
                var dish = new Dish("eggs", AllowedOrderType.Single);

                //act
                var addDishResult = dishesMenuService.TryAddDish(tuple, dish);
                var hasDishes = dishesMenuService.HasDishes();

                //assert
                addDishResult.Should().BeTrue("Because there is no other dish that could cause a duplicated key");
                hasDishes.Should().BeTrue("Because if the dish was added, the dictionary will be populated");
            }
        }

        [Fact]
        public void Should_has_no_dishes_when_the_tryAddDish_not_called_first()
        {
            //arrange
            using (var dishesMenuService = new DishesMenuService())
            {
                var tuple = (TimeOfDayType.Morning, DishType.Entree);
                var dish = new Dish("eggs", AllowedOrderType.Single);

                //act
                var hasDishes = dishesMenuService.HasDishes();

                //assert
                hasDishes.Should().BeFalse("Because the dictionary of dishes is empty");
            }
        }

        [Fact]
        public void Should_find_a_dish_when_a_call_to_tryAddDish_has_success()
        {
            //arrange
            using (var dishesMenuService = new DishesMenuService())
            {
                var tuple = (TimeOfDayType.Morning, DishType.Entree);
                var dish = new Dish("eggs", AllowedOrderType.Single);

                //act
                var addDishResult = dishesMenuService.TryAddDish(tuple, dish);
                var dishFound = dishesMenuService.FindDish(TimeOfDayType.Morning, DishType.Entree);

                //assert
                addDishResult.Should().BeTrue("Because there is no other dish that could cause a duplicated key");
                dishFound.Should().Be(dish.Name, "Because the dish was added to the dictionary and can be retrieved by the key-pair (id) time of day and dish type");
            }
        }

        [Fact]
        public void Should_be_empty_when_there_is_no_dish_with_the_timeOfDay_and_dishType_provided()
        {
            //arrange
            using (var dishesMenuService = new DishesMenuService())
            {
                var tuple = (TimeOfDayType.Morning, DishType.Entree);
                var dish = new Dish("eggs", AllowedOrderType.Single);

                //act
                var addDishResult = dishesMenuService.TryAddDish(tuple, dish);
                var dishFound = dishesMenuService.FindDish(TimeOfDayType.Night, DishType.Entree);

                //assert
                addDishResult.Should().BeTrue("Because there is no other dish that could cause a duplicated key");
                dishFound.Should().BeEmpty("Because the dish is not in to the dictionary and cannot be retrieved by the key-pair (id) time of day and dish type");
            }
        }

        [Fact]
        public void Should_be_valid_when_the_amount_of_orders_match_with_the_allowedOrderType()
        {
            //arrange
            using (var dishesMenuService = new DishesMenuService())
            {
                var tuple = (TimeOfDayType.Morning, DishType.Entree);
                var dish = new Dish("eggs", AllowedOrderType.Single);

                //act
                var addDishResult = dishesMenuService.TryAddDish(tuple, dish);
                var isValid = dishesMenuService.IsValidAmount(TimeOfDayType.Morning, DishType.Entree, 1);

                //assert
                addDishResult.Should().BeTrue("Because there is no other dish that could cause a duplicated key");
                isValid.Should().BeTrue("Because the allowed amount of orders is configured to be single");
            }
        }

        [Fact]
        public void Should_be_invalid_when_the_amount_of_orders_is_higher_than_the_allowedOrderType()
        {
            //arrange
            using (var dishesMenuService = new DishesMenuService())
            {
                var tuple = (TimeOfDayType.Morning, DishType.Entree);
                var dish = new Dish("eggs", AllowedOrderType.Single);

                //act
                var addDishResult = dishesMenuService.TryAddDish(tuple, dish);
                var isValid = dishesMenuService.IsValidAmount(TimeOfDayType.Morning, DishType.Entree, 2);

                //assert
                addDishResult.Should().BeTrue("Because there is no other dish that could cause a duplicated key");
                isValid.Should().BeFalse("Because the allowed amount of orders is configured to be single and the amount is higher");
            }
        }

        [Fact]
        public void Should_be_valid_when_the_amount_of_orders_is_multiple_and_match_with_the_allowedOrderType()
        {
            //arrange
            using (var dishesMenuService = new DishesMenuService())
            {
                var tuple = (TimeOfDayType.Morning, DishType.Entree);
                var dish = new Dish("coffee", AllowedOrderType.Multiple);

                //act
                var addDishResult = dishesMenuService.TryAddDish(tuple, dish);
                var isValid = dishesMenuService.IsValidAmount(TimeOfDayType.Morning, DishType.Entree, 2);

                //assert
                addDishResult.Should().BeTrue("Because there is no other dish that could cause a duplicated key");
                isValid.Should().BeTrue("Because the allowed amount of orders is configured to be multiple and the amount is multiple too");
            }
        }

        [Fact]
        public void Should_be_valid_when_the_amount_of_orders_is_single_but_the_allowedOrderType_is_multiple()
        {
            //arrange
            using (var dishesMenuService = new DishesMenuService())
            {
                var tuple = (TimeOfDayType.Morning, DishType.Entree);
                var dish = new Dish("coffee", AllowedOrderType.Multiple);

                //act
                var addDishResult = dishesMenuService.TryAddDish(tuple, dish);
                var isValid = dishesMenuService.IsValidAmount(TimeOfDayType.Morning, DishType.Entree, 1);

                //assert
                addDishResult.Should().BeTrue("Because there is no other dish that could cause a duplicated key");
                isValid.Should().BeTrue("Because the allowed amount of orders is configured to be single and the amount is multiple, considering 1..N");
            }
        }

        [Fact]
        public void Should_be_invalid_when_the_amount_of_orders_is_negative()
        {
            //arrange
            using (var dishesMenuService = new DishesMenuService())
            {
                var tuple = (TimeOfDayType.Morning, DishType.Entree);
                var dish = new Dish("coffee", AllowedOrderType.Multiple);

                //act
                var addDishResult = dishesMenuService.TryAddDish(tuple, dish);
                var isValid = dishesMenuService.IsValidAmount(TimeOfDayType.Morning, DishType.Entree, -1);

                //assert
                addDishResult.Should().BeTrue("Because there is no other dish that could cause a duplicated key");
                isValid.Should().BeFalse("Because a negative number is considered a error");
            }
        }

        [Fact]
        public void Should_no_have_dishes_when_the_dispose_is_called()
        {
            //arrange
            var dishesMenuService = new DishesMenuService();
            var tuple = (TimeOfDayType.Morning, DishType.Entree);
            var dish = new Dish("coffee", AllowedOrderType.Multiple);

            //act
            var addDishResult = dishesMenuService.TryAddDish(tuple, dish);
            dishesMenuService.Dispose();
            var hasDishes = dishesMenuService.HasDishes();
            //assert
            addDishResult.Should().BeTrue("Because there is no other dish that could cause a duplicated key");
            hasDishes.Should().BeFalse("Because after a dispose, the dictionary will be cleaned");
        }
    }
}
