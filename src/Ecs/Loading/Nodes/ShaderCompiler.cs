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

   [OnReadyGet("%SpatialShaderRoot/%SpatialWigglers")]
   Spatial _spatialWigglers = null!;

   [OnReadyGet("MarginContainer/CanvasItemWigglers")]
   Control _canvasItemWigglers = null!;

   public bool ProcessResourcePreloader(Resource res) {
      if (res is Mesh mesh && res.ResourceName == "dungeon_tile") {
         CallDeferred(nameof(InstanceMultiMeshWiggler), mesh);
      }
      if (res is not Material material)
         return false;

      switch (material) {
         case SpatialMaterial spatialMaterial:
            CallDeferred(nameof(InstanceSpatialWiggler), spatialMaterial);
            break;
         case ShaderMaterial shaderMaterial:
            CallDeferred(nameof(InstanceWigglerByShaderMode), shaderMaterial, material);
            break;
         case CanvasItemMaterial canvasItemMaterial:
            CallDeferred(nameof(InstanceCanvasItemWiggler), canvasItemMaterial);
            break;
         case ParticlesMaterial particlesMaterial:
            CallDeferred(nameof(InstanceParticlesWiggler), particlesMaterial);
            break;
      }
      return true;
   }

   public override void _ExitTree() { }

   void InstanceWigglerByShaderMode(ShaderMaterial shaderMaterial, Material material) {
      var mode = shaderMaterial.Shader.GetMode();

      switch (mode) {
         case Shader.Mode.Spatial:
            CallDeferred(nameof(InstanceSpatialWiggler), material);
            break;
         case Shader.Mode.CanvasItem:
            CallDeferred(nameof(InstanceCanvasItemWiggler), material);
            break;
         case Shader.Mode.Particles:
            CallDeferred(nameof(InstanceParticlesWiggler), material);
            break;
      }
   }

   void InstanceMultiMeshWiggler(Mesh mesh) {
      Logger.Info("Instancing multimesh wiggler");
      var mmInst = new MultiMeshInstance();
      mmInst.PhysicsInterpolationMode = PhysicsInterpolationModeEnum.Off;
      var mMesh = new MultiMesh();
      mmInst.Multimesh = mMesh;
      mMesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3d;
      mMesh.CustomDataFormat = MultiMesh.CustomDataFormatEnum.Data8bit;
      mMesh.Mesh = mesh;
      _spatialWigglers.AddChild(mmInst);
      mMesh.InstanceCount = 5;

      for (var i = 0; i < 5; i++) {
         mMesh.SetInstanceTransform(i, new Transform(Basis.Identity, new Vector3(i, i, i)));
         mMesh.SetInstanceCustomData(i, Color.Color8((byte) i, 0, 0, 0));
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