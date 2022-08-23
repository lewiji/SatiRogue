using Godot;
using RelEcs;

namespace SatiRogue.Ecs.Core;

public class GameState : Control {
   public readonly SystemGroup InitSystems = new();
   public readonly SystemGroup ProcessSystems = new();
   public readonly SystemGroup PhysicsSystems = new();
   public readonly SystemGroup ContinueSystems = new();
   public readonly SystemGroup PauseSystems = new();
   public readonly SystemGroup ExitSystems = new();

   public override void _EnterTree() {
      SizeFlagsHorizontal = (int) SizeFlags.ExpandFill;
      SizeFlagsVertical = (int) SizeFlags.ExpandFill;
      AnchorRight = 1f;
      AnchorBottom = 1f;
   }

   public void SetupSystems(GameStateController gameStates) {
      Init(gameStates);
   }

   public virtual void Init(GameStateController gameStates) { }
}