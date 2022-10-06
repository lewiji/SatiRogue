using RelEcs;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Play.Systems;

public class EnemyAnimationSystem : ISystem {
   public World World { get; set; } = null!;
   public void Run() { }
}