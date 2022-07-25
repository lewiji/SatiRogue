using SatiRogue.RelEcs.Systems;

namespace SatiRogue.RelEcs.States; 

public class PlayState : GameState
{
   public override void Init(GameStateController gameStates)
   {
      InitSystems
         .Add(new SpatialMapSystem())
         .Add(new SpawnPlayerSystem());
      
      OnTurnSystems
         .Add(new MovementSystem());

      ProcessSystems
         .Add(new InputSystem());

      PhysicsSystems
         .Add(new InterpolateWalkAnimationSystem());
   }
}