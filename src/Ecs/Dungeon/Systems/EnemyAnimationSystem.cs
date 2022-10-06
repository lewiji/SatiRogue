using RelEcs;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Dungeon.Systems;

public class EnemyAnimationSystem : ISystem {
   public World World { get; set; } = null!;
   public void Run() { }
}