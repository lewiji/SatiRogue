using Godot;
using Godot.Collections;
using SatiRogue.Ecs.Loading.Nodes;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Loading.Systems;

public class CompileShaders : GdSystem {
   [Signal] public delegate void ShadersCompiled();

   static readonly PackedScene ShaderCompilerScene = GD.Load<PackedScene>("res://src/Ecs/Loading/Nodes/ShaderCompiler.tscn");
   ShaderCompiler? _shaderCompiler;

   public override void Run() {
      _shaderCompiler = ShaderCompilerScene.Instance<ShaderCompiler>();
      GetElement<LoadingState>().AddChild(_shaderCompiler);
      GetElement<LoadingState>().Connect(nameof(LoadingState.LoadedResources), this, nameof(OnResourcesReceived));
      AddElement(_shaderCompiler);
      AddElement(this);
   }

   async void OnResourcesReceived(Array<Resource> resources) {
      await _shaderCompiler!.ProcessResourcePreloader(resources);
      EmitSignal(nameof(ShadersCompiled));
   }
}