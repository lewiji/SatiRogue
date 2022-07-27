using RelEcs;

namespace SatiRogue.Ecs.Play.Systems; 

public class TurnHandlerInitSystem : GDSystem {
   public override void Run() {
      var turnHandlerEntity = Spawn()
         .Add(new Components.Turn())
         .Id();
      AddElement(GetComponent<Components.Turn>(turnHandlerEntity));
   }
}