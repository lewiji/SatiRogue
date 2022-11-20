using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Tools;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems.Init;

public class TurnHandlerInitSystem : ISystem {
   

   public void Run(World world) {
      var turnHandlerEntity = world.Spawn().Add(new Turn()).Id();
      world.AddOrReplaceElement(world.GetComponent<Turn>(turnHandlerEntity));
   }
}