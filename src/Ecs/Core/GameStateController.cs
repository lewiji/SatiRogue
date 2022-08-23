using System.Collections.Generic;
using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Play.Nodes;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Core;

public class DeltaTime {
   public float Value;
}

public class PhysicsDeltaTime {
   public float Value;
}

public class GameStateController : Node {
   private readonly Stack<GameState> _stack = new();
   public readonly World World = new();

   public GameStateController() {
      GD.Randomize();
      World.AddElement(this);
      World.AddElement(new DeltaTime());
      World.AddElement(new PhysicsDeltaTime());
      World.AddElement(new Entities());
      World.AddElement(new MapGeometry());
      World.AddElement(new AudioNodes());
   }

   public override void _Ready() {
      Name = "GameStateController";
      World.AddElement(GetTree());
      AddChild(World.GetElement<Entities>());
      AddChild(World.GetElement<MapGeometry>());
      AddChild(World.GetElement<AudioNodes>());
   }

   /*public override void _UnhandledInput(InputEvent e) {
      if (_stack.Count == 0) {
         return;
      }

      World.Send(e);
      e.Dispose();
   }*/

   public override void _Process(float delta) {
      if (_stack.Count == 0) {
         return;
      }

      var currentState = _stack.Peek();
      World.GetElement<DeltaTime>().Value = delta;
      currentState.ProcessSystems.Run(World);
   }

   public override void _PhysicsProcess(float delta) {
      if (_stack.Count == 0) {
         return;
      }

      var currentState = _stack.Peek();
      World.GetElement<PhysicsDeltaTime>().Value = delta;
      currentState.PhysicsSystems.Run(World);
      World.Tick();
   }

   public override void _ExitTree() {
      foreach (var state in _stack) {
         state.ExitSystems.Run(World);
      }
   }

   public void PushState(GameState newState) {
      CallDeferred(nameof(PushStateDeferred), newState);
   }

   public void PopState() {
      CallDeferred(nameof(PopStateDeferred));
   }

   public void ChangeState(GameState newState) {
      CallDeferred(nameof(ChangeStateDeferred), newState);
   }

   private void PopStateDeferred() {
      if (_stack.Count == 0) return;

      var currentState = _stack.Pop();
      currentState.ExitSystems.Run(World);
      RemoveChild(currentState);
      currentState.QueueFree();

      if (_stack.Count <= 0) return;

      currentState = _stack.Peek();
      World.ReplaceElement(currentState);
      currentState.ContinueSystems.Run(World);
   }

   private void PushStateDeferred(GameState newState) {
      if (_stack.Count > 0) {
         var currentState = _stack.Peek();

         if (currentState.GetType() == newState.GetType()) {
            Logger.Warn($"{currentState.GetType()} already at the top of the stack!");

            return;
         }

         currentState.PauseSystems.Run(World);
         currentState.Visible = false;
      }

      newState.Name = newState.GetType().Name;
      _stack.Push(newState);
      AddChild(newState);

      if (World.HasElement<GameState>()) World.ReplaceElement(newState);
      else World.AddElement(newState);

      newState.SetupSystems(this);
      newState.InitSystems.Run(World);
   }

   private void ChangeStateDeferred(GameState newState) {
      if (_stack.Count > 0) {
         var currentState = _stack.Pop();
         currentState.ExitSystems.Run(World);
         RemoveChild(currentState);
         currentState.QueueFree();
      }

      newState.Name = newState.GetType().Name;
      _stack.Push(newState);
      AddChild(newState);
      World.ReplaceElement(newState);
      newState.SetupSystems(this);
      newState.InitSystems.Run(World);
   }
}