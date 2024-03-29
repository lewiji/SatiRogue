using Godot;
using Godot.Collections;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Loading.Systems;
using SatiRogue.Tools;

namespace SatiRogue.Ecs;

public class LoadingState : GameState {
   [Signal]
   public delegate void FinishedLoading();

   PreloadResources? _preloadResources;

   public override void Init(GameStateController gameStates) {
      gameStates.World.AddOrReplaceElement(this);

      CompileShaders compileShaders;
      InitSystems
         .Add(new CreateResourceQueue())
         .Add(_preloadResources = new PreloadResources())
         .Add(compileShaders = new CompileShaders());
      _preloadResources.Connect(nameof(PreloadResources.AllResourcesLoaded), compileShaders, nameof(CompileShaders.OnAllResourcesLoaded));
      compileShaders.Connect(nameof(CompileShaders.ShadersCompiled), this, nameof(OnShadersCompiled));
   }

   void OnShadersCompiled() {
      Logger.Info("Preloading Finished");
      EmitSignal(nameof(FinishedLoading));
   }

   public LoadingState(GameStateController gameStateController) : base(gameStateController) { }
}