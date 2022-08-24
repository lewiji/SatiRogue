using Godot;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Loading.Systems;

namespace SatiRogue.Ecs;

public class LoadingState : GameState {
   [Signal] public delegate void FinishedLoading();
   public override void Init(GameStateController gameStates) {
      gameStates.World.AddElement(this);
      CompileShaders compileShaders;
      InitSystems.Add(new PreloadResources())
         .Add(compileShaders = new CompileShaders());

      compileShaders.Connect(
         nameof(CompileShaders.ShadersCompiled), this, nameof(OnShadersCompiled));
   }

   void OnShadersCompiled() {
      EmitSignal(nameof(FinishedLoading));
   }
}