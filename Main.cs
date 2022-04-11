using Godot;
using RoguelikeMono.Tools;
using RoguelikeMono.Tools.Components;
using RoguelikeMono.Tools.Systems;
using SystemsRx.ReactiveData;

namespace RoguelikeMono
{
    public partial class Main : Spatial
    {
        private RoguelikeMonoEcsApplication? _ecsApplication;
        private Rng _rng = new Rng();

        public override void _Ready()
        {
            _ecsApplication = new RoguelikeMonoEcsApplication();
            _ecsApplication.StartApplication();
        }

        public override void _ExitTree()
        {
            _ecsApplication?.StopApplication();
        }
    }

    public class RoguelikeMonoEcsApplication : EcsApplication
    {
        protected override void ApplicationStarted()
        {
            SystemExecutor.AddSystem(new LoggingGroupSystem());
            
            var defaultPool = EntityDatabase.GetCollection();
            var entity = defaultPool.CreateEntity();

            var canTalkComponent = new CanLogComponent {Message = new ReactiveProperty<string>("Hello World")};
            entity.AddComponents(new[] {canTalkComponent});
        }
    }
}
