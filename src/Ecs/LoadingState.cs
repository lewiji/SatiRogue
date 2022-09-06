using Godot;
using Godot.Collections;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Loading.Systems;
namespace SatiRogue.Ecs;

public class LoadingState : GameState {
   [Signal] public delegate void FinishedLoading();
   [Signal] public delegate void LoadedResources(Array<Resource> resources);
   [Signal] public delegate void RequestNextResourceLoad();

   readonly Array<Resource> _loadedResources = new();
   PreloadResources? _preloadResources;

   public override void Init(GameStateController gameStates) {
      gameStates.World.AddElement(this);
      _loadedResources.Clear();

      CompileShaders compileShaders;
      InitSystems.Add(_preloadResources = new PreloadResources()).Add(compileShaders = new CompileShaders());
      _preloadResources.Connect(nameof(PreloadResources.ResourceLoaded), this, nameof(OnResourceReceived));
      _preloadResources.Connect(nameof(PreloadResources.AllResourcesLoaded), this, nameof(OnAllResourcesLoaded));
      compileShaders.Connect(nameof(CompileShaders.ShadersCompiled), this, nameof(OnShadersCompiled));
   }

   void OnShadersCompiled() {
      Logger.Info("Preloading Finished");
      EmitSignal(nameof(FinishedLoading));
   }

   void OnResourceReceived(Resource resource) {
      _loadedResources.Add(resource);
      EmitSignal(nameof(RequestNextResourceLoad));
   }

   void OnAllResourcesLoaded() {
      EmitSignal(nameof(LoadedResources), _loadedResources);
      _loadedResources.Clear();
   }
}