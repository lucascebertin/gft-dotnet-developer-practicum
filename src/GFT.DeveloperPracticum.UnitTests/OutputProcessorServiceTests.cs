using FakeItEasy;
using FluentAssertions;
using GFT.DeveloperPracticum.Kernel.Enumerators;
using GFT.DeveloperPracticum.Kernel.Models;
using GFT.DeveloperPracticum.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace GFT.DeveloperPracticum.UnitTests
{
    public class OutputProcessorServiceTests
    {
        [Fact]
        public void Should_the_output_be_returned_with_no_multiplier_factor_when_providing_single_orders()
        {
            //arrange
            var outputProcessorService = new OutputProcessorService();
            var exception = default(Exception);
            var result = default(string);

            var input = new InputProcessResult(
                false,
                new Dictionary<(DishType, string), int>()
                {
                    { (DishType.Entree, "eggs"), 1 },
                    { (DishType.Side, "toast"), 1 },
                    { (DishType.Drink, "coffee"), 1 }
                }
            );

            outputProcessorService.Process(input).Match(
                failure: ex => exception = ex,
                success: output => result = output
            );

            exception.Should().BeNull();
            result.Should().Be("eggs, toast, coffee", "Because the output is sorted to follow the correct order and there is no multiple orders or input errors");
        }

        [Fact]
        public void Should_the_output_be_returned_with_no_multiplier_factor_and_with_error_text_when_providing_single_orders_and_invalid_inputs()
        {
            //arrange
            var outputProcessorService = new OutputProcessorService();
            var exception = default(Exception);
            var result = default(string);

            var input = new InputProcessResult(
                true,
                new Dictionary<(DishType, string), int>
                {
                    { (DishType.Entree, "eggs"), 1 },
                    { (DishType.Side, "toast"), 1 },
                    { (DishType.Drink, "coffee"), 1 }
                }
            );

            outputProcessorService.Process(input).Match(
                failure: ex => exception = ex,
                success: output => result = output
            );

            exception.Should().BeNull();
            result.Should().Be("eggs, toast, coffee, error", "Because the output is sorted to follow the correct order and there is no multiple orders but input errors");
        }

        [Fact]
        public void Should_the_output_be_returned_with_multiplier_factor_when_providing_multiple_orders()
        {
            //arrange
            var outputProcessorService = new OutputProcessorService();
            var exception = default(Exception);
            var result = default(string);

            var input = new InputProcessResult(
                false,
                new Dictionary<(DishType, string), int>()
                {
                    { (DishType.Entree, "eggs"), 1 },
                    { (DishType.Side, "toast"), 1 },
                    { (DishType.Drink, "coffee"), 3 }
                }
            );

            outputProcessorService.Process(input).Match(
                failure: ex => exception = ex,
                success: output => result = output
            );

            exception.Should().BeNull();
            result.Should().Be("eggs, toast, coffee(x3)", "Because the output is sorted to follow the correct order, there is multiple orders and no input errors");
        }

        [Fact]
        public void Should_the_output_dont_be_returned_when_providing_no_orders()
        {
            //arrange
            var outputProcessorService = new OutputProcessorService();
            var exception = default(Exception);
            var result = default(string);

            outputProcessorService.Process(null).Match(
                failure: ex => exception = ex,
                success: output => result = output
            );

            exception.Should().NotBeNull("Because the input being null is considered an error");
        }

        [Fact]
        public void Should_the_output_dont_be_returned_when_providing_orders_less_than_zero()
        {
            //arrange
            var outputProcessorService = new OutputProcessorService();
            var exception = default(Exception);
            var result = default(string);

            var input = new InputProcessResult(
                false,
                new Dictionary<(DishType, string), int>()
                {
                    { (DishType.Entree, "eggs"), -1 },
                    { (DishType.Side, "toast"), -2 },
                    { (DishType.Drink, "coffee"), -4 }
                }
            );

            outputProcessorService.Process(null).Match(
                failure: ex => exception = ex,
                success: output => result = output
            );

            exception.Should().NotBeNull("Because the input being less then zero is considered an error");
        }
    }
}
