using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems.Init;

public class TurnHandlerInitSystem : ISystem {
   public World World { get; set; } = null!;

   public void Run() {
      var turnHandlerEntity = this.Spawn().Add(new Turn()).Id();
      World.AddElement(this.GetComponent<Turn>(turnHandlerEntity));
   }
}