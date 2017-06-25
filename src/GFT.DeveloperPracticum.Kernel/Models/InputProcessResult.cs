using GFT.DeveloperPracticum.Kernel.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace GFT.DeveloperPracticum.Kernel.Models
{
    public class InputProcessResult
    {
        public bool HasInvalidInput { get; private set; }
        public Dictionary<Tuple<DishType, string>, int> ParsedInput { get; private set; }

        public InputProcessResult(
            bool hasInvalidInput,
            Dictionary<Tuple<DishType, string>, int> parsedInput
        )
        {
            HasInvalidInput = hasInvalidInput;
            ParsedInput = parsedInput;
        }
    }
}
