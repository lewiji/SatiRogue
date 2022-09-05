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

   ResourceInteractiveLoader? _currentLoader;
   string _resourcePath = "";
   readonly Array<Resource> _loadedResources = new();
   PreloadResources? _preloadResources;

   public override void Init(GameStateController gameStates) {
      gameStates.World.AddElement(this);
      _loadedResources.Clear();

      CompileShaders compileShaders;
      InitSystems.Add(_preloadResources = new PreloadResources()).Add(compileShaders = new CompileShaders());

      _preloadResources.Connect(nameof(PreloadResources.LoadingResource), this, nameof(OnResourceToLoad));
      _preloadResources.Connect(nameof(PreloadResources.AllResourcesLoaded), this, nameof(OnAllResourcesLoaded));
      compileShaders.Connect(nameof(CompileShaders.ShadersCompiled), this, nameof(OnShadersCompiled));
   }

   void OnShadersCompiled() {
      EmitSignal(nameof(FinishedLoading));
   }

   void OnResourceToLoad(string resourcePath, ResourceInteractiveLoader loader) {
      _currentLoader = loader;
      _resourcePath = resourcePath;
   }

   public override void _Process(float delta) {
      if (_currentLoader == null) return;
      var err = _currentLoader.Poll();

      // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
      switch (err) {
         case Error.Ok:
            // Loaded a chunk of the resource
            break;
         case Error.FileEof:
            _loadedResources.Add(_currentLoader.GetResource());
            ResetCurrentLoader();
            break;
         default:
            Logger.Error($"Failed to load resource: {_resourcePath}");
            ResetCurrentLoader();
            break;
      }
   }

   void ResetCurrentLoader() {
      _currentLoader = null;
      _resourcePath = "";
      CallDeferred(nameof(LoadNext));
   }

   void OnAllResourcesLoaded() {
      EmitSignal(nameof(LoadedResources), _loadedResources);
   }

   void LoadNext() {
      EmitSignal(nameof(RequestNextResourceLoad));
   }
}