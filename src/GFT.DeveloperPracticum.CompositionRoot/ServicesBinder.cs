using DryIoc;
using GFT.DeveloperPracticum.Abstractions.Services;
using GFT.DeveloperPracticum.Services;

namespace GFT.DeveloperPracticum.CompositionRoot
{
    public static class ServicesBinder
    {
        public static void Bind(IContainer container)
        {
            container.Register<IDishesMenuService, DishesMenuService>(
                reuse: Reuse.Transient
            );

            container.Register<IInputProcessorService, InputProcessorService>(
                reuse: Reuse.Transient
            );

            container.Register<IOutputProcessorService, OutputProcessorService>(
                reuse: Reuse.Transient
            );

            container.Register<IProcessManagerService, ProcessManagerService>(
                reuse: Reuse.Transient
            );
        }
    }
}
