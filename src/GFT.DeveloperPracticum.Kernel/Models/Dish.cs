using ElemarJR.FunctionalCSharp;
using GFT.DeveloperPracticum.Kernel.Enumerators;
using GFT.DeveloperPracticum.Kernel.Helpers;
using System;
using System.Text;

namespace GFT.DeveloperPracticum.Kernel.Models
{
    public sealed class Dish
    {
        public string Name { get; private set; }
        public AllowedOrderType AllowedOrderAmount { get; private set; }

        public Dish(
            string name,
            AllowedOrderType allowedOrderAmount
        )
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
            AllowedOrderAmount = allowedOrderAmount;
        }

        public override int GetHashCode() =>
            HashCodeHelper.CustomHashCode(
                Option.Of(
                    new object[] {
                        Name,
                        AllowedOrderAmount,
                    }
                )
            );

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"The dish is { Name.ToLower() } ");
            builder.Append($"and the allowed amout of orders is { AllowedOrderAmount.ToString().ToLower() }");
            return builder.ToString();
        }

        public override bool Equals(object value)
        {
            var dish = value as Dish;

            return !ReferenceEquals(null, dish)
                && string.Equals(Name, dish.Name)
                && Equals(AllowedOrderAmount, dish.AllowedOrderAmount);
        }

        public static bool operator ==(Dish left, Dish right) =>
            ObjectComparisonHelper.Same(left, right);

        public static bool operator !=(Dish left, Dish right) =>
            ObjectComparisonHelper.NotSame(left, right);
    }
}
