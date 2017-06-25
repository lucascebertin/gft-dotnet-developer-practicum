using ElemarJR.FunctionalCSharp;
using FakeItEasy;
using FluentAssertions;
using GFT.DeveloperPracticum.Abstractions.Services;
using GFT.DeveloperPracticum.Kernel.Enumerators;
using GFT.DeveloperPracticum.Kernel.Models;
using GFT.DeveloperPracticum.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace GFT.DeveloperPracticum.UnitTests
{
    public class InputProcessorServiceTests
    {

        [Fact]
        public void Should_return_a_normalized_timeOfDay_when_provided_a_correct_string()
        {
            //arrange
            var input = new string[] { "MorNing" };
            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);
            var morningText = TimeOfDayType.Morning.ToString();

            //act
            var timeOfDay = inputProcessorService.ParseTimeOfDay(input);

            //assert
            timeOfDay.Should().Be(morningText, "Because will be found, no matter the case, if the value exists on TimeOfDayType (ex: Morning, Night)");
        }

        [Fact]
        public void Should_return_empty_timeOfDay_when_provided_a_null()
        {
            //arrange
            var input = default(string[]);
            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);
            var morningText = default(string);

            //act
            var timeOfDay = inputProcessorService.ParseTimeOfDay(input);

            //assert
            timeOfDay.Should().Be(morningText, "Because will not be found, if the value is null it's considered an error");
        }

        [Fact]
        public void Should_return_empty_timeOfDay_when_provided_a_non_registered_timeOfDay()
        {
            //arrange
            var input = new string[] { "evening" };
            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);
            var morningText = default(string);

            //act
            var timeOfDay = inputProcessorService.ParseTimeOfDay(input);

            //assert
            timeOfDay.Should().Be(morningText, "Because will not be found, if the value don't exists it's considered an error");
        }

        [Fact]
        public void Should_return_an_argumentException_when_provided_a_null()
        {
            //arrange
            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);
            var exception = default(ArgumentException);

            //act
            var setupResult = inputProcessorService.Setup(null);
            setupResult.Match(
                failure: ex => exception = ex,
                success: _ => { }
            );

            //assert
            exception.Should().BeOfType(typeof(ArgumentException), "Because the setup will fail");
        }

        [Fact]
        public void Should_return_an_unit_when_provided_a_correct_menu_item()
        {
            //arrange
            var keyPair = new Tuple<TimeOfDayType, DishType>(TimeOfDayType.Morning, DishType.Entree);
            var dish = new Dish("eggs", AllowedOrderType.Single);
            var dict = new Dictionary<Tuple<TimeOfDayType, DishType>, Dish>() {
                { keyPair, dish }
            };

            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            A.CallTo(() => dishesMenuServiceFake.TryAddDish(keyPair, dish)).Returns(true);

            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);
            var exception = default(ArgumentException);
            var unit = default(Unit);

            //act
            var setupResult = inputProcessorService.Setup(dict);
            setupResult.Match(
                failure: ex => exception = ex,
                success: _ =>
                {
                    unit = _;
                }
            );

            //assert
            exception.Should().BeNull("Because the setup will succeed and return a unit");
        }

        [Fact]
        public void Should_add_the_dish_when_this_is_the_first()
        {
            //arrange
            var dishes = new Dictionary<Tuple<DishType, string>, int>();
            var timeOfDay = TimeOfDayType.Morning;
            var dishType = DishType.Drink;
            var dish = new Dish("coffee", AllowedOrderType.Multiple);

            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, dishType, 1)).Returns(true);

            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);

            //act
            var dishAddedOrUpdated = inputProcessorService.TryAddOrUpdateDishesAmount(
                dishes,
                timeOfDay,
                dishType,
                dish.Name
            );

            //assert
            dishAddedOrUpdated.Should().BeTrue("Because the dish allow multiple orders, the rule will be satisfied and should be added");
        }

        [Fact]
        public void Should_update_a_dish_when_the_name_already_exists_and_allow_multiple_orders()
        {
            //arrange
            var dishType = DishType.Drink;
            var dish = new Dish("coffee", AllowedOrderType.Multiple);
            var timeOfDay = TimeOfDayType.Morning;
            var nextAmountToAdd = 2;

            var dishes = new Dictionary<Tuple<DishType, string>, int>() {
                { new Tuple<DishType, string>(dishType, dish.Name), 1 }
            };

            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(
                timeOfDay, 
                dishType, 
                nextAmountToAdd)
            ).Returns(true);

            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);

            //act
            var dishAddedOrUpdated = inputProcessorService.TryAddOrUpdateDishesAmount(
                dishes,
                timeOfDay,
                dishType,
                dish.Name
            );

            //assert
            dishAddedOrUpdated.Should().BeTrue("Because the dish allow multiple orders, the rule will be satisfied and should be added");
        }

        [Fact]
        public void Should_not_update_a_dish_when_the_name_already_exists_and_disallow_multiple_orders()
        {
            //arrange
            var dishType = DishType.Drink;
            var dish = new Dish("eggs", AllowedOrderType.Single);
            var timeOfDay = TimeOfDayType.Morning;
            var nextAmountToAdd = 2;

            var dishes = new Dictionary<Tuple<DishType, string>, int>() {
                { new Tuple<DishType, string>(dishType, dish.Name), 1 }
            };

            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(
                timeOfDay,
                dishType,
                nextAmountToAdd)
            ).Returns(false);

            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);

            //act
            var dishAddedOrUpdated = inputProcessorService.TryAddOrUpdateDishesAmount(
                dishes,
                timeOfDay,
                dishType,
                dish.Name
            );

            //assert
            dishAddedOrUpdated.Should().BeFalse("Because the dish disallow multiple orders, the rule will not be satisfied");
        }

        [Fact]
        public void Should_not_update_a_dish_when_the_dish_name_is_null()
        {
            //arrange
            var dishType = DishType.Drink;
            var dishName = default(string);
            var timeOfDay = TimeOfDayType.Morning;
            var nextAmountToAdd = 2;

            var dishes = new Dictionary<Tuple<DishType, string>, int>() {
                { new Tuple<DishType, string>(dishType, dishName), 1 }
            };

            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(
                timeOfDay,
                dishType,
                nextAmountToAdd)
            ).Returns(false);

            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);

            //act
            var dishAddedOrUpdated = inputProcessorService.TryAddOrUpdateDishesAmount(
                dishes,
                timeOfDay,
                dishType,
                dishName
            );

            //assert
            dishAddedOrUpdated.Should().BeFalse("Because when the dish name is null it's considered a error");
        }

        [Fact]
        public void Should_the_input_be_processed_with_success_when_looks_like_the_first_test_sample_input()
        {
            //arrange
            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            A.CallTo(() => dishesMenuServiceFake.HasDishes()).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Morning, DishType.Entree)).Returns("eggs");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Morning, DishType.Side)).Returns("toast");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Morning, DishType.Drink)).Returns("coffee");

            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);
            var input = "morning, 1, 2, 3";
            var exception = default(Exception);
            var processResult = default(InputProcessResult);
            var expectedOutput = new Dictionary<Tuple<DishType, string>, int>()
            {
                { new Tuple<DishType, string>(DishType.Entree, "eggs"), 1 },
                { new Tuple<DishType, string>(DishType.Side, "toast"), 1 },
                { new Tuple<DishType, string>(DishType.Drink, "coffee"), 1 }
            };

            //act
            inputProcessorService.Process(input).Match(
                failure: ex => exception = ex,
                success: result => processResult = result
            );

            //assert
            exception.Should().BeNull("Because the input should be processed with success");
            processResult.HasInvalidInput.Should().BeFalse();

            foreach (var parsedInput in processResult.ParsedInput)
                parsedInput.Value.Should().Be(expectedOutput[parsedInput.Key]);
        }

        [Fact]
        public void Should_the_input_be_processed_with_success_when_looks_like_the_second_test_sample_input()
        {
            //arrange
            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            A.CallTo(() => dishesMenuServiceFake.HasDishes()).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Morning, DishType.Entree)).Returns("eggs");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Morning, DishType.Side)).Returns("toast");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Morning, DishType.Drink)).Returns("coffee");

            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);
            var input = "morning, 2, 1, 3";
            var exception = default(Exception);
            var processResult = default(InputProcessResult);
            var expectedOutput = new Dictionary<Tuple<DishType, string>, int>()
            {
                { new Tuple<DishType, string>(DishType.Entree, "eggs"), 1 },
                { new Tuple<DishType, string>(DishType.Side, "toast"), 1 },
                { new Tuple<DishType, string>(DishType.Drink, "coffee"), 1 }
            };

            //act
            inputProcessorService.Process(input).Match(
                failure: ex => exception = ex,
                success: result => processResult = result
            );

            //assert
            exception.Should().BeNull("Because the input should be processed with success");
            processResult.HasInvalidInput.Should().BeFalse();

            foreach (var parsedInput in processResult.ParsedInput)
                parsedInput.Value.Should().Be(expectedOutput[parsedInput.Key]);
        }

        [Fact]
        public void Should_the_input_be_processed_with_success_when_looks_like_the_third_test_sample_input()
        {
            //arrange
            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            A.CallTo(() => dishesMenuServiceFake.HasDishes()).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Morning, DishType.Entree)).Returns("eggs");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Morning, DishType.Side)).Returns("toast");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Morning, DishType.Drink)).Returns("coffee");

            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);
            var input = "morning, 1, 2, 3, 4";
            var exception = default(Exception);
            var processResult = default(InputProcessResult);
            var expectedOutput = new Dictionary<Tuple<DishType, string>, int>()
            {
                { new Tuple<DishType, string>(DishType.Entree, "eggs"), 1 },
                { new Tuple<DishType, string>(DishType.Side, "toast"), 1 },
                { new Tuple<DishType, string>(DishType.Drink, "coffee"), 1 }
            };

            //act
            inputProcessorService.Process(input).Match(
                failure: ex => exception = ex,
                success: result => processResult = result
            );

            //assert
            exception.Should().BeNull("Because the input should be processed with success (even with a invalid selection)");
            processResult.HasInvalidInput.Should().BeTrue();

            foreach (var parsedInput in processResult.ParsedInput)
                parsedInput.Value.Should().Be(expectedOutput[parsedInput.Key]);
        }

        [Fact]
        public void Should_the_input_be_processed_with_success_when_looks_like_the_fourth_test_sample_input()
        {
            //arrange
            var timeOfDay = TimeOfDayType.Morning;

            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            A.CallTo(() => dishesMenuServiceFake.HasDishes()).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Entree, 1)).Returns(true);
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Side, 1)).Returns(true);
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Drink, A<int>.That.IsGreaterThan(0))).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.FindDish(timeOfDay, DishType.Entree)).Returns("eggs");
            A.CallTo(() => dishesMenuServiceFake.FindDish(timeOfDay, DishType.Side)).Returns("toast");
            A.CallTo(() => dishesMenuServiceFake.FindDish(timeOfDay, DishType.Drink)).Returns("coffee");

            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);
            var input = "morning, 1, 2, 3, 3, 3";
            var exception = default(Exception);
            var processResult = default(InputProcessResult);
            var expectedOutput = new Dictionary<Tuple<DishType, string>, int>()
            {
                { new Tuple<DishType, string>(DishType.Entree, "eggs"), 1 },
                { new Tuple<DishType, string>(DishType.Side, "toast"), 1 },
                { new Tuple<DishType, string>(DishType.Drink, "coffee"), 3 }
            };

            //act
            inputProcessorService.Process(input).Match(
                failure: ex => exception = ex,
                success: result => processResult = result
            );

            //assert
            exception.Should().BeNull("Because the input should be processed with success");
            processResult.HasInvalidInput.Should().BeFalse();

            foreach (var parsedInput in processResult.ParsedInput)
                parsedInput.Value.Should().Be(expectedOutput[parsedInput.Key]);
        }

        [Fact]
        public void Should_the_input_be_processed_with_success_when_looks_like_the_fifth_test_sample_input()
        {
            //arrange
            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            A.CallTo(() => dishesMenuServiceFake.HasDishes()).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Entree)).Returns("steak");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Side)).Returns("potato");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Drink)).Returns("wine");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Dessert)).Returns("cake");

            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);
            var input = "night, 1, 2, 3, 4";
            var exception = default(Exception);
            var processResult = default(InputProcessResult);
            var expectedOutput = new Dictionary<Tuple<DishType, string>, int>()
            {
                { new Tuple<DishType, string>(DishType.Entree, "steak"), 1 },
                { new Tuple<DishType, string>(DishType.Side, "potato"), 1 },
                { new Tuple<DishType, string>(DishType.Drink, "wine"), 1 },
                { new Tuple<DishType, string>(DishType.Dessert, "cake"), 1 }
            };

            //act
            inputProcessorService.Process(input).Match(
                failure: ex => exception = ex,
                success: result => processResult = result
            );

            //assert
            exception.Should().BeNull("Because the input should be processed with success");
            processResult.HasInvalidInput.Should().BeFalse();

            foreach (var parsedInput in processResult.ParsedInput)
                parsedInput.Value.Should().Be(expectedOutput[parsedInput.Key]);
        }

        [Fact]
        public void Should_the_input_be_processed_with_success_when_looks_like_the_sixth_test_sample_input()
        {
            //arrange
            var timeOfDay = TimeOfDayType.Night;
            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            A.CallTo(() => dishesMenuServiceFake.HasDishes()).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Entree, 1)).Returns(true);
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Side, A<int>.That.IsGreaterThan(0))).Returns(true);
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Drink, 1)).Returns(true);
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Dessert, 1)).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Entree)).Returns("steak");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Side)).Returns("potato");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Drink)).Returns("wine");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Dessert)).Returns("cake");

            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);
            var input = "night, 1, 2, 2, 4";
            var exception = default(Exception);
            var processResult = default(InputProcessResult);
            var expectedOutput = new Dictionary<Tuple<DishType, string>, int>()
            {
                { new Tuple<DishType, string>(DishType.Entree, "steak"), 1 },
                { new Tuple<DishType, string>(DishType.Side, "potato"), 2 },
                { new Tuple<DishType, string>(DishType.Dessert, "cake"), 1 }
            };

            //act
            inputProcessorService.Process(input).Match(
                failure: ex => exception = ex,
                success: result => processResult = result
            );

            //assert
            exception.Should().BeNull("Because the input should be processed with success");
            processResult.HasInvalidInput.Should().BeFalse();

            foreach (var parsedInput in processResult.ParsedInput)
                parsedInput.Value.Should().Be(expectedOutput[parsedInput.Key]);
        }

        [Fact]
        public void Should_the_input_be_case_insentive_and_be_processed_with_success_when_looks_like_the_first_test_sample_input()
        {
            //arrange
            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            A.CallTo(() => dishesMenuServiceFake.HasDishes()).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Morning, DishType.Entree)).Returns("eggs");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Morning, DishType.Side)).Returns("toast");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Morning, DishType.Drink)).Returns("coffee");

            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);
            var input = "MoRnInG, 1, 2, 3";
            var exception = default(Exception);
            var processResult = default(InputProcessResult);
            var expectedOutput = new Dictionary<Tuple<DishType, string>, int>()
            {
                { new Tuple<DishType, string>(DishType.Entree, "eggs"), 1 },
                { new Tuple<DishType, string>(DishType.Side, "toast"), 1 },
                { new Tuple<DishType, string>(DishType.Drink, "coffee"), 1 }
            };

            //act
            inputProcessorService.Process(input).Match(
                failure: ex => exception = ex,
                success: result => processResult = result
            );

            //assert
            exception.Should().BeNull("Because the input should be processed with success");
            processResult.HasInvalidInput.Should().BeFalse();

            foreach (var parsedInput in processResult.ParsedInput)
                parsedInput.Value.Should().Be(expectedOutput[parsedInput.Key]);
        }

        [Fact]
        public void Should_the_input_be_case_insentive_and_be_processed_with_success_when_looks_like_the_second_test_sample_input()
        {
            //arrange
            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            A.CallTo(() => dishesMenuServiceFake.HasDishes()).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Morning, DishType.Entree)).Returns("eggs");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Morning, DishType.Side)).Returns("toast");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Morning, DishType.Drink)).Returns("coffee");

            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);
            var input = "MoRnInG, 2, 1, 3";
            var exception = default(Exception);
            var processResult = default(InputProcessResult);
            var expectedOutput = new Dictionary<Tuple<DishType, string>, int>()
            {
                { new Tuple<DishType, string>(DishType.Entree, "eggs"), 1 },
                { new Tuple<DishType, string>(DishType.Side, "toast"), 1 },
                { new Tuple<DishType, string>(DishType.Drink, "coffee"), 1 }
            };

            //act
            inputProcessorService.Process(input).Match(
                failure: ex => exception = ex,
                success: result => processResult = result
            );

            //assert
            exception.Should().BeNull("Because the input should be processed with success");
            processResult.HasInvalidInput.Should().BeFalse();

            foreach (var parsedInput in processResult.ParsedInput)
                parsedInput.Value.Should().Be(expectedOutput[parsedInput.Key]);
        }

        [Fact]
        public void Should_the_input_be_case_insentive_and_be_processed_with_success_when_looks_like_the_third_test_sample_input()
        {
            //arrange
            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            A.CallTo(() => dishesMenuServiceFake.HasDishes()).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Morning, DishType.Entree)).Returns("eggs");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Morning, DishType.Side)).Returns("toast");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Morning, DishType.Drink)).Returns("coffee");

            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);
            var input = "MoRnInG, 1, 2, 3, 4";
            var exception = default(Exception);
            var processResult = default(InputProcessResult);
            var expectedOutput = new Dictionary<Tuple<DishType, string>, int>()
            {
                { new Tuple<DishType, string>(DishType.Entree, "eggs"), 1 },
                { new Tuple<DishType, string>(DishType.Side, "toast"), 1 },
                { new Tuple<DishType, string>(DishType.Drink, "coffee"), 1 }
            };

            //act
            inputProcessorService.Process(input).Match(
                failure: ex => exception = ex,
                success: result => processResult = result
            );

            //assert
            exception.Should().BeNull("Because the input should be processed with success (even with a invalid selection)");
            processResult.HasInvalidInput.Should().BeTrue();

            foreach (var parsedInput in processResult.ParsedInput)
                parsedInput.Value.Should().Be(expectedOutput[parsedInput.Key]);
        }

        [Fact]
        public void Should_the_input_be_case_insentive_and_be_processed_with_success_when_looks_like_the_fourth_test_sample_input()
        {
            //arrange
            var timeOfDay = TimeOfDayType.Morning;

            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            A.CallTo(() => dishesMenuServiceFake.HasDishes()).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Entree, 1)).Returns(true);
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Side, 1)).Returns(true);
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Drink, A<int>.That.IsGreaterThan(0))).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.FindDish(timeOfDay, DishType.Entree)).Returns("eggs");
            A.CallTo(() => dishesMenuServiceFake.FindDish(timeOfDay, DishType.Side)).Returns("toast");
            A.CallTo(() => dishesMenuServiceFake.FindDish(timeOfDay, DishType.Drink)).Returns("coffee");

            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);
            var input = "MoRnInG, 1, 2, 3, 3, 3";
            var exception = default(Exception);
            var processResult = default(InputProcessResult);
            var expectedOutput = new Dictionary<Tuple<DishType, string>, int>()
            {
                { new Tuple<DishType, string>(DishType.Entree, "eggs"), 1 },
                { new Tuple<DishType, string>(DishType.Side, "toast"), 1 },
                { new Tuple<DishType, string>(DishType.Drink, "coffee"), 3 }
            };

            //act
            inputProcessorService.Process(input).Match(
                failure: ex => exception = ex,
                success: result => processResult = result
            );

            //assert
            exception.Should().BeNull("Because the input should be processed with success");
            processResult.HasInvalidInput.Should().BeFalse();

            foreach (var parsedInput in processResult.ParsedInput)
                parsedInput.Value.Should().Be(expectedOutput[parsedInput.Key]);
        }

        [Fact]
        public void Should_the_input_be_case_insentive_and_be_processed_with_success_when_looks_like_the_fifth_test_sample_input()
        {
            //arrange
            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            A.CallTo(() => dishesMenuServiceFake.HasDishes()).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Entree)).Returns("steak");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Side)).Returns("potato");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Drink)).Returns("wine");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Dessert)).Returns("cake");

            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);
            var input = "NiGhT, 1, 2, 3, 4";
            var exception = default(Exception);
            var processResult = default(InputProcessResult);
            var expectedOutput = new Dictionary<Tuple<DishType, string>, int>()
            {
                { new Tuple<DishType, string>(DishType.Entree, "steak"), 1 },
                { new Tuple<DishType, string>(DishType.Side, "potato"), 1 },
                { new Tuple<DishType, string>(DishType.Drink, "wine"), 1 },
                { new Tuple<DishType, string>(DishType.Dessert, "cake"), 1 }
            };

            //act
            inputProcessorService.Process(input).Match(
                failure: ex => exception = ex,
                success: result => processResult = result
            );

            //assert
            exception.Should().BeNull("Because the input should be processed with success");
            processResult.HasInvalidInput.Should().BeFalse();

            foreach (var parsedInput in processResult.ParsedInput)
                parsedInput.Value.Should().Be(expectedOutput[parsedInput.Key]);
        }

        [Fact]
        public void Should_the_input_be_case_insentive_and_be_processed_with_success_when_looks_like_the_sixth_test_sample_input()
        {
            //arrange
            var timeOfDay = TimeOfDayType.Night;
            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            A.CallTo(() => dishesMenuServiceFake.HasDishes()).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Entree, 1)).Returns(true);
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Side, A<int>.That.IsGreaterThan(0))).Returns(true);
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Drink, 1)).Returns(true);
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Dessert, 1)).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Entree)).Returns("steak");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Side)).Returns("potato");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Drink)).Returns("wine");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Dessert)).Returns("cake");

            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);
            var input = "NiGhT, 1, 2, 2, 4";
            var exception = default(Exception);
            var processResult = default(InputProcessResult);
            var expectedOutput = new Dictionary<Tuple<DishType, string>, int>()
            {
                { new Tuple<DishType, string>(DishType.Entree, "steak"), 1 },
                { new Tuple<DishType, string>(DishType.Side, "potato"), 2 },
                { new Tuple<DishType, string>(DishType.Dessert, "cake"), 1 }
            };

            //act
            inputProcessorService.Process(input).Match(
                failure: ex => exception = ex,
                success: result => processResult = result
            );

            //assert
            exception.Should().BeNull("Because the input should be processed with success");
            processResult.HasInvalidInput.Should().BeFalse();

            foreach (var parsedInput in processResult.ParsedInput)
                parsedInput.Value.Should().Be(expectedOutput[parsedInput.Key]);
        }

        [Fact]
        public void Should_not_the_input_be_processed_with_success_when_timeOfDay_is_not_mapped()
        {
            //arrange
            var timeOfDay = TimeOfDayType.Night;
            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            A.CallTo(() => dishesMenuServiceFake.HasDishes()).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Entree, 1)).Returns(true);
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Side, A<int>.That.IsGreaterThan(0))).Returns(true);
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Drink, 1)).Returns(true);
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Dessert, 1)).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Entree)).Returns("steak");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Side)).Returns("potato");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Drink)).Returns("wine");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Dessert)).Returns("cake");

            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);
            var input = "afternoon, 1, 2, 2, 4";
            var exception = default(Exception);
            var processResult = default(InputProcessResult);
            var expectedOutput = new Dictionary<Tuple<DishType, string>, int>()
            {
                { new Tuple<DishType, string>(DishType.Entree, "steak"), 1 },
                { new Tuple<DishType, string>(DishType.Side, "potato"), 2 },
                { new Tuple<DishType, string>(DishType.Dessert, "cake"), 1 }
            };

            //act
            inputProcessorService.Process(input).Match(
                failure: ex => exception = ex,
                success: result => processResult = result
            );

            //assert
            exception.Should().BeNull("Because the input should not be processed with success");
            processResult.HasInvalidInput.Should().BeTrue();
            processResult.ParsedInput.Should().BeEmpty();
        }

        [Fact]
        public void Should_not_the_input_be_processed_with_success_when_null()
        {
            //arrange
            var timeOfDay = TimeOfDayType.Night;
            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            A.CallTo(() => dishesMenuServiceFake.HasDishes()).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Entree, 1)).Returns(true);
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Side, A<int>.That.IsGreaterThan(0))).Returns(true);
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Drink, 1)).Returns(true);
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Dessert, 1)).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Entree)).Returns("steak");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Side)).Returns("potato");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Drink)).Returns("wine");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Dessert)).Returns("cake");

            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);
            var input = default(string);
            var exception = default(Exception);
            var processResult = default(InputProcessResult);
            var expectedOutput = new Dictionary<Tuple<DishType, string>, int>()
            {
                { new Tuple<DishType, string>(DishType.Entree, "steak"), 1 },
                { new Tuple<DishType, string>(DishType.Side, "potato"), 2 },
                { new Tuple<DishType, string>(DishType.Dessert, "cake"), 1 }
            };

            //act
            inputProcessorService.Process(input).Match(
                failure: ex => exception = ex,
                success: result => processResult = result
            );

            //assert
            exception.Should().NotBeNull("Because the input should be provided and can't be null");
        }

        [Fact]
        public void Should_not_the_input_be_processed_with_success_when_without_at_least_one_selection()
        {
            //arrange
            var timeOfDay = TimeOfDayType.Night;
            var dishesMenuServiceFake = A.Fake<IDishesMenuService>();
            A.CallTo(() => dishesMenuServiceFake.HasDishes()).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Entree, 1)).Returns(true);
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Side, A<int>.That.IsGreaterThan(0))).Returns(true);
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Drink, 1)).Returns(true);
            A.CallTo(() => dishesMenuServiceFake.IsValidAmount(timeOfDay, DishType.Dessert, 1)).Returns(true);

            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Entree)).Returns("steak");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Side)).Returns("potato");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Drink)).Returns("wine");
            A.CallTo(() => dishesMenuServiceFake.FindDish(TimeOfDayType.Night, DishType.Dessert)).Returns("cake");

            var inputProcessorService = new InputProcessorService(dishesMenuServiceFake);
            var input = "morning";
            var exception = default(Exception);
            var processResult = default(InputProcessResult);
            var expectedOutput = new Dictionary<Tuple<DishType, string>, int>()
            {
                { new Tuple<DishType, string>(DishType.Entree, "steak"), 1 },
                { new Tuple<DishType, string>(DishType.Side, "potato"), 2 },
                { new Tuple<DishType, string>(DishType.Dessert, "cake"), 1 }
            };

            //act
            inputProcessorService.Process(input).Match(
                failure: ex => exception = ex,
                success: result => processResult = result
            );

            //assert
            exception.Should().NotBeNull("Because the input should be provided and can't be null");
        }
    }
}
