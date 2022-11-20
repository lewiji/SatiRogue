using Godot;
using Godot.Collections;
using SatiRogue.Debug;
using SatiRogue.Ecs.Loading.Nodes;
using RelEcs;
using SatiRogue.resources;
using SatiRogue.Tools;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Loading.Systems;

public class CompileShaders : Reference, ISystem {
   

   [Signal]
   public delegate void ShadersCompiled();

   static readonly PackedScene ShaderCompilerScene = GD.Load<PackedScene>("res://src/Ecs/Loading/Nodes/ShaderCompiler.tscn");
   ShaderCompiler? _shaderCompiler;
   SatiConfig? _satiConfig;
   int _shadersToCompile = 0;

   public void Run(World world) {
      _shaderCompiler = ShaderCompilerScene.Instance<ShaderCompiler>();
      world.GetElement<LoadingState>().AddChild(_shaderCompiler);
      world.AddOrReplaceElement(_shaderCompiler);
      world.AddOrReplaceElement(this);
      _satiConfig ??= world.GetElement<SatiConfig>();
   }

   public void OnResourceReceived(Resource resource) {
      if (_satiConfig != null && _satiConfig.DisableManualShaderPrecompiler)
         return;

      if (_shaderCompiler!.ProcessResourcePreloader(resource)) {
         _shadersToCompile += 1;
      }
   }

   public async void OnAllResourcesLoaded() {
      if (_shadersToCompile > 0) {
         GD.Print($"CompileShaders: {_shadersToCompile} to compile");
         await ToSignal(_shaderCompiler!.GetTree().CreateTimer(0.5f), "timeout");
      }

      OnFinished();
   }

   async void OnFinished() {
      EmitSignal(nameof(ShadersCompiled));
   }
}