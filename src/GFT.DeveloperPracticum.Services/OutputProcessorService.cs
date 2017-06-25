using ElemarJR.FunctionalCSharp;
using GFT.DeveloperPracticum.Kernel.Models;
using GFT.DeveloperPracticum.Services;
using System;
using System.Linq;

namespace GFT.DeveloperPracticum.Services
{
    public class OutputProcessorService : IOutputProcessorService
    {
        public Try<InvalidOperationException, string> Process(
            InputProcessResult inputResult
        )
        {
            if (inputResult == null 
                || inputResult.ParsedInput == null 
                || inputResult.ParsedInput.Count == 0)
                return new InvalidOperationException("The parsed input is empty");

            var lessThanZero = inputResult.ParsedInput
                .Values
                .ToList()
                .Exists(value => 
                    value <= 0
                );

            if (lessThanZero)
                return new InvalidOperationException("The given orders amount can't be null or lesser than zero");

            var orderedOutput = inputResult
                    .ParsedInput
                    .OrderBy(o => (int)o.Key.Item1)
                    .Select(p =>
                    {
                        var hasMultiplier = p.Value > 1;

                        return hasMultiplier
                            ? $"{p.Key.Item2}(x{p.Value})"
                            : $"{p.Key.Item2}";
                    });

            var joinedOutput = string.Join(", ", orderedOutput);

            if (inputResult.HasInvalidInput)
            {
                joinedOutput = !string.IsNullOrWhiteSpace(joinedOutput)
                    ? $"{joinedOutput}, error"
                    : "error";
            }

            return joinedOutput;
        }
    }
}
