using System;
using Godot;
using GodotOnReady.Attributes;
using Object = Godot.Object;
namespace SatiRogue.scenes.Hud;

[Tool]
public partial class StatBar3D : Spatial {
   [OnReadyGet("AnimationPlayer", Export = true)] private AnimationPlayer? _animationPlayer;
   private float _interpolatedPercent = 0f;
   [OnReadyGet("MultiMeshInstance", OrNull = true, Export = true)] private MultiMeshInstance? _multiMeshInstance;
   private float _percent;
   private ShaderMaterial? _shaderMaterial;
   [OnReadyGet("Tween", Export = true)] private Tween? _tween;

   public bool Hidden = false;

   [Export] public float Percent {
      get => _percent;
      set {
         _percent = Mathf.Clamp(value, 0f, 1f);

         if (Mathf.IsEqualApprox(_interpolatedPercent, _percent)) return;

         if (_tween == null) return;
         if (_tween.IsActive()) _tween.StopAll();
         _tween.InterpolateProperty(this, nameof(_interpolatedPercent), null, _percent, 0.2f, Tween.TransitionType.Sine);
         _tween.Start();

         if (!Hidden && _percent < 0.999f) {
            Visible = true;
         }
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
      _multiMeshInstance.Multimesh.SetInstanceTransform(1, new Transform(Basis.Identity, new Vector3(0, 0.01f, 0.015f)));
      _multiMeshInstance.Multimesh.SetInstanceCustomData(1, new Color(0.4892f, 0.5504f, 1f));
   }

   [OnReady]
   private void SetupTween() {
      if (_tween == null) return;
      _tween.Connect("tween_step", this, nameof(OnTweenStep));
      _tween.Connect("tween_all_completed", this, nameof(OnTweenCompleted));
   }

   private void OnTweenStep(Object obj, NodePath key, float elapsed, float value) {
      _shaderMaterial?.SetShaderParam("percent_full", value);
   }

   private void OnTweenCompleted() {
      _shaderMaterial?.SetShaderParam("percent_full", _percent);

      if (Hidden || Math.Abs(_percent - 1f) < 0.01f) {
         Visible = false;
      }
   }

   /*public override void _Process(float delta) {
      if (_tween == null) {
         _shaderMaterial?.SetShaderParam("percent_full", _percent);
      } else if (_tween.IsActive()) {
         _shaderMaterial?.SetShaderParam("percent_full", _interpolatedPercent);
      }
   }*/

   public void OnDead() {
      _animationPlayer?.Play("die");
   }
}