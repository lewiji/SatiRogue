using EcsRx.Infrastructure;
using EcsRx.Plugins.Batching;
using EcsRx.Plugins.Computeds;
using EcsRx.Plugins.Persistence;
using EcsRx.Plugins.ReactiveSystems;
using EcsRx.Plugins.ReactiveSystems.Extensions;
using EcsRx.Plugins.Views;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Ninject;

namespace RoguelikeMono;

public abstract class EcsApplication : EcsRxApplication
{
    public override IDependencyContainer Container { get; } = new NinjectDependencyContainer();

    protected override void LoadPlugins()
    {
        RegisterPlugin(new ReactiveSystemsPlugin());
        RegisterPlugin(new ComputedsPlugin());
        RegisterPlugin(new ViewsPlugin());
        RegisterPlugin(new BatchPlugin());
        RegisterPlugin(new PersistencePlugin());
    }

    protected override void StartSystems()
    {
        this.StartAllBoundReactiveSystems();
    }
}
