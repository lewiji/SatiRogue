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

   public virtual void Init(GameStateController gameStates) { }
}