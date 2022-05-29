using Godot;
using GodotOnReady.Attributes;

namespace SatiRogue.scenes.Hud; 

[Tool]
public partial class StatBar3D : Spatial {
   [OnReadyGet("MultiMeshInstance", OrNull = true, Export = true)] private MultiMeshInstance? _multiMeshInstance;
   private float _percent;
   private ShaderMaterial? _shaderMaterial;
   
   [OnReady]
   private void SetInstanceTransforms() {
      if (_multiMeshInstance == null) return;
      _shaderMaterial = _multiMeshInstance.Multimesh.Mesh.SurfaceGetMaterial(0) as ShaderMaterial;
      _multiMeshInstance.Multimesh.InstanceCount = 2;
      // Frame
      _multiMeshInstance.Multimesh.SetInstanceTransform(0, new Transform(Basis.Identity, new Vector3(0, 0, 0)));
      _multiMeshInstance.Multimesh.SetInstanceCustomData(0, new Color(0.464285714286f, 0, 0));
      
      // Bar
      _multiMeshInstance.Multimesh.SetInstanceTransform(1, new Transform(Basis.Identity, new Vector3(0, 0, 0.025f)));
      _multiMeshInstance.Multimesh.SetInstanceCustomData(1, new Color(0.4892f, 0.5504f, 1f));
   }

   [Export]
   public float Percent {
      get => _percent;
      set {
         _percent = value;
         if (_shaderMaterial != null) {
            //var gradTex = (GradientTexture) _shaderMaterial.GetShaderParam("texture_progress_gradient");
            //var linearPercent = _percent * (0.93f - 0.15f) + 0.15f;

            //gradTex.Gradient.SetOffset(1, linearPercent);
         }
      }
   }
}