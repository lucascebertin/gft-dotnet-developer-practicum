using Xunit;
using FluentAssertions;
using GFT.DeveloperPracticum.Kernel.Enumerators;
using GFT.DeveloperPracticum.Kernel.Models;

namespace GFT.DeveloperPracticum.UnitTests
{
    public class DishBehaviorTests
    {
        [Fact]
        public void Should_hashcode_be_unique_by_instance()
        {
            //arrange
            var dish = new Dish("eggs", AllowedOrderType.Single);

            //act
            var hashCode = dish.GetHashCode();
            var testHashCode = dish.GetHashCode();

            //assert
            testHashCode.Should()
                .Be(hashCode, "Because they run the same algorithm to calculate the hash.");
        }

        [Fact]
        public void Should_hashcode_be_different_for_different_instances()
        {
            //arrange
            var firstDish = new Dish("eggs", AllowedOrderType.Single);
            var secondDish = new Dish("eggs", AllowedOrderType.Multiple);

            //act
            var firstHashcode = firstDish.GetHashCode();
            var secondHashcode = secondDish.GetHashCode();

            //assert
            secondHashcode.Should()
                .NotBe(firstHashcode, "Because the algorithm calculates the hash based on the property values");
        }

        [Fact]
        public void Should_instance_be_comparable_with_equal_operator()
        {
            //arrange
            var firstDish = new Dish("eggs", AllowedOrderType.Single);
            var secondDish = new Dish("eggs", AllowedOrderType.Single);

            //act
            var comparison = (firstDish == secondDish);
            
            //assert
            comparison.Should()
                .Be(true, "Because the algorithm calculates the hash based on the property values");
        }

        [Fact]
        public void Should_instance_be_comparable_with_not_equal_operator()
        {
            //arrange
            var firstDish = new Dish("eggs", AllowedOrderType.Single);
            var secondDish = new Dish("eggs", AllowedOrderType.Multiple);

            //act
            var comparison = (firstDish != secondDish);

            //assert
            comparison.Should()
                .Be(true, "Because the algorithm calculates the hash based on the property values");
        }
    }
}


//var inputs = new string[] {
//    "morning, 1, 2, 3",
//    "morning, 2, 1, 3",
//    "morning, 1, 2, 3, 4",
//    "morning, 1, 2, 3, 3, 3",
//    "night, 1, 2, 3, 4",
//    "night, 1, 2, 2, 4",
//    "nIghT, 1, 2, 2, 4",
//    "n1ghT, 1, 2, 2, 4",
//    "n1ghT, 1, a, 2, 4",
//    "nighT, 1, a, 2, 4",
//};