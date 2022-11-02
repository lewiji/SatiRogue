using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Tools;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems.Init;

public class TurnHandlerInitSystem : ISystem {
   public World World { get; set; } = null!;

   public void Run() {
      var turnHandlerEntity = World.Spawn().Add(new Turn()).Id();
      World.AddOrReplaceElement(World.GetComponent<Turn>(turnHandlerEntity));
   }
}