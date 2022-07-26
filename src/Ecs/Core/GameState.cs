using Godot;
using RelEcs;

namespace SatiRogue.Ecs.Core; 

public class GameState : Spatial {
   public readonly SystemGroup InitSystems = new();
   public readonly SystemGroup ProcessSystems = new();
   public readonly SystemGroup PhysicsSystems = new();
   public readonly SystemGroup ContinueSystems = new();
   public readonly SystemGroup PauseSystems = new();
   public readonly SystemGroup ExitSystems = new();

   public void SetupSystems(GameStateController gameStates) {
      Init(gameStates);
      
      InitSystems.Ready(gameStates.World);
      ProcessSystems.Ready(gameStates.World);
      PhysicsSystems.Ready(gameStates.World);
      ContinueSystems.Ready(gameStates.World);
      PauseSystems.Ready(gameStates.World);
      ExitSystems.Ready(gameStates.World);
   }
   public virtual void Init(GameStateController gameStates) { }
   
}