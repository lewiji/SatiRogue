using RelEcs;

namespace SatiRogue.Ecs.MapGenerator.Systems; 

public class PassToPlayState : GDSystem {
   public override void Run() {
      GetElement<Main>().OnMapGenInitFinished();
   }
}