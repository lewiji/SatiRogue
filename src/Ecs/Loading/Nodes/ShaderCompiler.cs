using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using GodotOnReady.Attributes;
using SatiRogue.Debug;
namespace SatiRogue.Ecs.Loading.Nodes;

public partial class ShaderCompiler : CanvasLayer {
   static readonly PackedScene SpatialWigglerScene = GD.Load<PackedScene>("res://src/Ecs/Loading/Nodes/SpatialShaderWiggler.tscn");
   static readonly PackedScene CanvasItemWigglerScene = GD.Load<PackedScene>("res://src/Ecs/Loading/Nodes/CanvasItemWiggler.tscn");

   [OnReadyGet("%SpatialShaderRoot/%SpatialWigglers")] Spatial _spatialWigglers = null!;
   [OnReadyGet("MarginContainer/CanvasItemWigglers")] Control _canvasItemWigglers = null!;

   public async Task ProcessResourcePreloader(Array<Resource> resources) {
      Logger.Info("Processing materials.");

      foreach (var res in resources) {
         if (res is not Material material) continue;

         Logger.Debug(material.ResourcePath);

         switch (material) {
            case SpatialMaterial spatialMaterial:
               InstanceSpatialWiggler(spatialMaterial);
               break;
            case ShaderMaterial shaderMaterial:
               InstanceWigglerByShaderMode(shaderMaterial, material);
               break;
            case CanvasItemMaterial canvasItemMaterial:
               InstanceCanvasItemWiggler(canvasItemMaterial);
               break;
            case ParticlesMaterial particlesMaterial:
               InstanceParticlesWiggler(particlesMaterial);
               break;
         }
      }

      await ToSignal(GetTree().CreateTimer(0.3f), "timeout");
   }

   public override void _ExitTree() { }

   void InstanceWigglerByShaderMode(ShaderMaterial shaderMaterial, Material material) {
      var mode = shaderMaterial.Shader.GetMode();

      switch (mode) {
         case Shader.Mode.Spatial:
            InstanceSpatialWiggler(material);
            break;
         case Shader.Mode.CanvasItem:
            InstanceCanvasItemWiggler(material);
            break;
         case Shader.Mode.Particles:
            InstanceParticlesWiggler(material);
            break;
      }
   }

   void InstanceSpatialWiggler(Material material) {
      var spatialWiggler = SpatialWigglerScene.Instance<Spatial>();
      spatialWiggler.GetNode<MeshInstance>("MeshInstance").MaterialOverride = material;
      spatialWiggler.Translation = new Vector3((float) GD.RandRange(-5, 5), (float) GD.RandRange(-5, 5), (float) GD.RandRange(0, 20));
      _spatialWigglers.AddChild(spatialWiggler);
   }

   void InstanceCanvasItemWiggler(Material material) {
      var canvasItemWiggler = CanvasItemWigglerScene.Instance<CanvasItemWiggler>();
      canvasItemWiggler.GetNode<TextureRect>("TextureRect").Material = material;
      _canvasItemWigglers.AddChild(canvasItemWiggler);
   }

   void InstanceParticlesWiggler(Material material) {
      Logger.Warn($"{material}: ParticlesWiggler not implemented");
   }
}