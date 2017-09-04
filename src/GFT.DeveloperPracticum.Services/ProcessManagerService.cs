using ElemarJR.FunctionalCSharp;

using GFT.DeveloperPracticum.Abstractions.Services;
using GFT.DeveloperPracticum.Kernel.Enumerators;
using GFT.DeveloperPracticum.Kernel.Models;

using System;
using System.Collections.Generic;

namespace GFT.DeveloperPracticum.Services
{
    public class ProcessManagerService : IProcessManagerService
    {
        private readonly IInputProcessorService _inputProcessorService;
        private readonly IOutputProcessorService _outputProcessorService;

        public ProcessManagerService(
            IInputProcessorService inputProcessorService,
            IOutputProcessorService outputProcessorService
        )
        {
            _inputProcessorService = inputProcessorService
                ?? throw new ArgumentNullException(nameof(inputProcessorService));

            _outputProcessorService = outputProcessorService
                ?? throw new ArgumentNullException(nameof(outputProcessorService));
        }

        public Try<Exception, string> Process(
            IReadOnlyDictionary<(TimeOfDayType, DishType), Dish> menu,
            string input
        )
        {
            var setupResult =_inputProcessorService.Setup(menu);

            return setupResult.Match(
                failure: ex => $"Setup failed, reason: { ex.Message }",
                success: unit =>
                {
                    var inputProcessResult = _inputProcessorService.Process(input);

                    return inputProcessResult.Match(
                        failure: ex => {
                            //log ex
                            return $"error";
                        }, 
                        success: inputParsed => 
                        {
                            var outputProcessResult = _outputProcessorService.Process(inputParsed);

                            return outputProcessResult.Match(
                                failure: ex => $"Output processing failed, reason: { ex.Message }",
                                success: output => output
                            );
                        }
                    );
                }
            );
        }
    }
}
