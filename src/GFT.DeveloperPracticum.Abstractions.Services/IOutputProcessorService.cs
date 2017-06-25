using System;
using ElemarJR.FunctionalCSharp;
using GFT.DeveloperPracticum.Kernel.Models;

namespace GFT.DeveloperPracticum.Services
{
    public interface IOutputProcessorService
    {
        Try<InvalidOperationException, string> Process(InputProcessResult inputResult);
    }
}