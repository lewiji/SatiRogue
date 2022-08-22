using RelEcs;

namespace SatiRogue.Ecs.Play.Systems;

public class TurnHandlerInitSystem : GdSystem {
   public override void Run() {
      var turnHandlerEntity = Spawn().Add(new Components.Turn()).Id();
      AddElement(GetComponent<Components.Turn>(turnHandlerEntity));
   }
}