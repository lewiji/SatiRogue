using SatiRogue.Ecs.Play.Components;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems.Init;

public class TurnHandlerInitSystem : GdSystem {
   public override void Run() {
      var turnHandlerEntity = Spawn().Add(new Turn()).Id();
      AddElement(GetComponent<Turn>(turnHandlerEntity));
   }
}