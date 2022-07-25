using SatiRogue.RelEcs.Systems;

namespace SatiRogue.RelEcs.States; 

public class PlayState : GameState
{
   public override void Init(GameStateController gameStates)
   {
      InitSystems
         .Add(new SpatialMapSystem())
         .Add(new SpawnPlayerSystem());

      ProcessSystems
         .Add(new InputSystem())
         .Add(new MovementSystem());

      PhysicsSystems
         .Add(new InterpolateWalkAnimationSystem());
   }
}