using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Components;
using SatiRogue.Components.Stats;
using SatiRogue.Entities;

namespace SatiRogue.scenes.Hud; 

[Tool]
public partial class StatBar3D : Spatial {
   [OnReadyGet("MultiMeshInstance", OrNull = true, Export = true)] private MultiMeshInstance? _multiMeshInstance;
   [OnReadyGet("AnimationPlayer", Export = true)] private AnimationPlayer? _animationPlayer;
   [OnReadyGet("Tween", Export = true)] private Tween? _tween;
   
   private ShaderMaterial? _shaderMaterial;
   private float _percent;
   private float _interpolatedPercent = 0f;

   [Export] public float Percent {
      get => _percent;
      set {
         _percent = Mathf.Clamp(value, 0f, 1f);
         if (Mathf.IsEqualApprox(_interpolatedPercent, _percent)) return;

         if (_tween == null) return;
         if (_tween.IsActive()) _tween.StopAll();
         _tween.InterpolateProperty(this, nameof(_interpolatedPercent), null, _percent, 0.16f, Tween.TransitionType.Sine);
         _tween.Start();
      }
   }

   [OnReady] private void SetupMultiMesh() {
      // MultiMesh
      _shaderMaterial = _multiMeshInstance?.MaterialOverride as ShaderMaterial;
      if (_multiMeshInstance == null) return;
      _multiMeshInstance.Multimesh.InstanceCount = 2;
      // Frame
      _multiMeshInstance.Multimesh.SetInstanceTransform(0, new Transform(Basis.Identity, new Vector3(0, 0, 0)));
      _multiMeshInstance.Multimesh.SetInstanceCustomData(0, new Color(0.464285714286f, 0, 0));
      // Bar
      _multiMeshInstance.Multimesh.SetInstanceTransform(1, new Transform(Basis.Identity, new Vector3(0, 0, 0.025f)));
      _multiMeshInstance.Multimesh.SetInstanceCustomData(1, new Color(0.4892f, 0.5504f, 1f));
   }

   public override void _Process(float delta) {
      if (_tween == null) {
         _shaderMaterial?.SetShaderParam("percent_full", _percent);
      } else if (_tween.IsActive()) {
         _shaderMaterial?.SetShaderParam("percent_full", _interpolatedPercent);
      }
   }

   public void OnDead() {
      _animationPlayer?.Play("die");
   }
}