using SatiRogue.RelEcs.Systems;

namespace SatiRogue.RelEcs.States; 

public class PlayState : GameState
{
   public override void Init(GameStateController gameStates)
   {
      InitSystems.Add(new SpawnPlayerSystem());

      UpdateSystems
         .Add(new MovementSystem());
   }
}