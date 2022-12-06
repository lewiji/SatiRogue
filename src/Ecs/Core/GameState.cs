using Godot;
using RelEcs;
using SatiRogue.Tools;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Core;

public partial class GameState : Control {
   public readonly SatiSystemGroup InitSystems;
   public readonly SatiSystemGroup ProcessSystems;
   public readonly SatiSystemGroup PhysicsSystems;
   public readonly SatiSystemGroup ContinueSystems;
   public readonly SatiSystemGroup PauseSystems;
   public readonly SatiSystemGroup ExitSystems;

   protected GameState(GameStateController gameStateController) {
      InitSystems = new SatiSystemGroup(gameStateController);
      ProcessSystems = new SatiSystemGroup(gameStateController);;
      PhysicsSystems = new SatiSystemGroup(gameStateController);;
      ContinueSystems = new SatiSystemGroup(gameStateController);;
      PauseSystems = new SatiSystemGroup(gameStateController);;
      ExitSystems = new SatiSystemGroup(gameStateController);;
   }

   public override void _EnterTree() {
      SizeFlagsHorizontal = (int) SizeFlags.ExpandFill;
      SizeFlagsVertical = (int) SizeFlags.ExpandFill;
      AnchorRight = 1f;
      AnchorBottom = 1f;
      MouseFilter = MouseFilterEnum.Ignore;
   }

   public void SetupSystems(GameStateController gameStates) {
      Init(gameStates);
   }

   public virtual void Init(GameStateController gameStates) { }
}