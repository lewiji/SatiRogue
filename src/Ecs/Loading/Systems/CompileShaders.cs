using Godot;
using RelEcs;
using SatiRogue.Ecs.Loading.Nodes;
namespace SatiRogue.Ecs.Loading.Systems; 

public class CompileShaders : GdSystem {
   [Signal] public delegate void ShadersCompiled();
   private static readonly PackedScene ShaderCompilerScene = GD.Load<PackedScene>("res://src/Ecs/Loading/Nodes/ShaderCompiler.tscn");
   private ShaderCompiler? _shaderCompiler;
   public override void Run() {
      _shaderCompiler = ShaderCompilerScene.Instance<ShaderCompiler>();
      _shaderCompiler.Connect("ready", this, nameof(OnShaderCompilerReady));
      GetElement<LoadingState>().AddChild(_shaderCompiler);
      AddElement(_shaderCompiler);
      AddElement(this);
   }

   async void OnShaderCompilerReady() {
      var resPreloader = GetElement<ResourcePreloader>();
      await _shaderCompiler!.ProcessResourcePreloader(resPreloader!);
      EmitSignal(nameof(ShadersCompiled));
   }
}