using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Components;
using SatiRogue.Components.Stats;
using SatiRogue.Entities;

namespace SatiRogue.scenes.Hud; 

[Tool]
public partial class StatBar3D : Spatial {
   private float _percent;
   public ShaderMaterial? ShaderMaterial;
   private StatsComponent? _statsComponent;
   [OnReadyGet("MultiMeshInstance", OrNull = true, Export = true)] private MultiMeshInstance? _multiMeshInstance;
   [OnReady]
   private void SetInstanceTransforms() {
      if (_multiMeshInstance == null) return;
      ShaderMaterial = _multiMeshInstance.MaterialOverride as ShaderMaterial;
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
         _percent = Mathf.Clamp(value, 0f, 1f);
         ShaderMaterial?.SetShaderParam("percent_full", _percent);
      }
   }
}