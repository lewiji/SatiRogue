using System.Threading.Tasks;
using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Debug;

namespace SatiRogue.Ecs.Loading.Nodes; 

public partial class ShaderCompiler : CanvasLayer {
   private static readonly PackedScene SpatialWigglerScene = GD.Load<PackedScene>("res://src/Ecs/Loading/Nodes/SpatialShaderWiggler.tscn");
   private static readonly PackedScene CanvasItemWigglerScene = GD.Load<PackedScene>("res://src/Ecs/Loading/Nodes/CanvasItemWiggler.tscn");
   
   [OnReadyGet("%SpatialShaderRoot/%SpatialWigglers")] private Spatial _spatialWigglers = null!;
   [OnReadyGet("MarginContainer/CanvasItemWigglers")] private Control _canvasItemWigglers = null!;
   
   public async Task ProcessResourcePreloader(ResourcePreloader preloader) {
      Logger.Info("Processing materials.");
      foreach (var res in preloader.GetResourceList()) {
         if (preloader.GetResource(res) is not Material material) continue;
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

      await ToSignal(GetTree().CreateTimer(1.618f), "timeout");
   }
   private void InstanceWigglerByShaderMode(ShaderMaterial shaderMaterial, Material material) {
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

   private void InstanceSpatialWiggler(Material material) {
      var spatialWiggler = SpatialWigglerScene.Instance<Spatial>();
      spatialWiggler.GetNode<MeshInstance>("MeshInstance").MaterialOverride = material;
      spatialWiggler.Translation = new Vector3((float)GD.RandRange(-5, 5), (float)GD.RandRange(-5, 5), (float)GD.RandRange(0, 20));
      _spatialWigglers.AddChild(spatialWiggler);
   }

   private void InstanceCanvasItemWiggler(Material material) {
      var canvasItemWiggler = CanvasItemWigglerScene.Instance<CanvasItemWiggler>();
      canvasItemWiggler.GetNode<TextureRect>("TextureRect").Material = material;
      _canvasItemWigglers.AddChild(canvasItemWiggler);
   }
   
   private void InstanceParticlesWiggler(Material material) {
      Logger.Warn($"{material}: ParticlesWiggler not implemented");
   }
}