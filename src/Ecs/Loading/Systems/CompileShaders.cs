using Godot;
using RelEcs;
using SatiRogue.Ecs.Loading.Nodes;
namespace SatiRogue.Ecs.Loading.Systems; 

public class CompileShaders : GdSystem {
   private static readonly PackedScene ShaderCompilerScene = GD.Load<PackedScene>("res://src/Ecs/Loading/Nodes/ShaderCompiler.tscn");
   private ShaderCompiler? _shaderCompiler;
   public override void Run() {
      _shaderCompiler = ShaderCompilerScene.Instance<ShaderCompiler>();
      _shaderCompiler.Connect("ready", this, nameof(OnShaderCompilerReady));
      GetElement<LoadingState>().AddChild(_shaderCompiler);
      AddElement(_shaderCompiler);
   }

   void OnShaderCompilerReady() {
      _shaderCompiler?.ProcessResourcePreloader(GetElement<ResourcePreloader>());
   }
}