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
   private int _shadersToCompile = 0;

   public void Run() {
      _shaderCompiler = ShaderCompilerScene.Instance<ShaderCompiler>();
      World.GetElement<LoadingState>().AddChild(_shaderCompiler);
      World.AddElement(_shaderCompiler);
      World.AddElement(this);
   }
   
   public void OnResourceReceived(Resource resource) {
      if (_shaderCompiler!.ProcessResourcePreloader(resource)) {
         _shadersToCompile += 1;
      }
   }

   public async void OnAllResourcesLoaded() {
      if (_shadersToCompile > 0) {
         await ToSignal(_shaderCompiler!.GetTree().CreateTimer(0.35f), "timeout");
      }
      
      OnFinished();
   }

   async void OnFinished() {
      EmitSignal(nameof(ShadersCompiled));
   }
}