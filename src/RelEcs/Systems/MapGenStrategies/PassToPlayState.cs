using RelEcs;

namespace SatiRogue.RelEcs.Systems.MapGenStratgies; 

public class PassToPlayState : GDSystem {
   public override void Run() {
      GetElement<Main>().OnMapGenInitFinished();
   }
}