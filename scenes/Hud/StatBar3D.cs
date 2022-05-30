using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Components;
using SatiRogue.Components.Stats;
using SatiRogue.Entities;

namespace SatiRogue.scenes.Hud; 

[Tool]
public partial class StatBar3D : Spatial {
   private float _percent;
   private ShaderMaterial? _shaderMaterial;
   private StatsComponent? _statsComponent;
   [OnReadyGet("MultiMeshInstance", OrNull = true, Export = true)] private MultiMeshInstance? _multiMeshInstance;
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

      CallDeferred(nameof(SetupEntityDataBindings));
   }

   private void SetupEntityDataBindings() {
      if (GetParent() is not MeshRendererEntity meshEntity) return;
      
      /* Getting the health component from the parent entity. */
      var statComponent = meshEntity.Entity?.GetComponent<StatHealthComponent>();
      _statsComponent = statComponent;
      statComponent?.Connect(nameof(StatsComponent.Changed), this, nameof(OnStatChanged));
      if (statComponent != null) Percent = statComponent.Percentage;
   }
   
   private void OnStatChanged(int newValue) {
      if (_statsComponent != null) Percent = (float) newValue / _statsComponent.MaxValue;
   }

   [Export]
   public float Percent {
      get => _percent;
      set {
         _percent = Mathf.Clamp(value, 0f, 1f);
         _shaderMaterial?.SetShaderParam("percent_full", _percent);
      }
   }
}