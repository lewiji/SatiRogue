using Godot;
using Godot.Collections;
using SatiRogue.Debug;
using SatiRogue.Ecs.Loading.Nodes;
using RelEcs;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Loading.Systems;

public class CompileShaders : Reference, ISystem {
   public World World { get; set; } = null!;

   [Signal]
   public delegate void ShadersCompiled();

   static readonly PackedScene ShaderCompilerScene = GD.Load<PackedScene>("res://src/Ecs/Loading/Nodes/ShaderCompiler.tscn");
   ShaderCompiler? _shaderCompiler;

   public void Run() {
      _shaderCompiler = ShaderCompilerScene.Instance<ShaderCompiler>();
      World.GetElement<LoadingState>().AddChild(_shaderCompiler);
      World.GetElement<LoadingState>().Connect(nameof(LoadingState.LoadedResources), this, nameof(OnResourcesReceived));
      World.AddElement(_shaderCompiler);
      World.AddElement(this);
   }

   async void OnResourcesReceived(Array<Resource> resources) {
      await _shaderCompiler!.ProcessResourcePreloader(resources);
      EmitSignal(nameof(ShadersCompiled));
   }
}