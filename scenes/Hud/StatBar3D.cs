using System;
using Godot;
using GodotOnReady.Attributes;
using Object = Godot.Object;
namespace SatiRogue.scenes.Hud;

public partial class StatBar3D : Spatial {
   public bool Hidden = false;

   [OnReadyGet("AnimationPlayer", Export = true)] private AnimationPlayer? _animationPlayer;
   [OnReadyGet("MultiMeshInstance", OrNull = true, Export = true)] private MultiMeshInstance? _multiMeshInstance;
   [OnReadyGet("Tween", Export = true)] private Tween? _tween;

   private float _interpolatedPercent = 0f;
   private float _percent;
   private ShaderMaterial? _shaderMaterial;

   private static Color _bgColor = new(0.464285714286f, 0, 0);
   private static Color _fgColor = new(0.4892f, 0.5504f, 1f);
   private static Transform _bgTransform = new(Basis.Identity, new Vector3(0, 0, 0));
   private static Transform _fgTransform = new(Basis.Identity, new Vector3(0, 0.01f, 0.015f));

   [Export] public float Percent {
      get => _percent;
      set {
         _percent = Mathf.Clamp(value, 0f, 1f);

         if (Mathf.IsEqualApprox(_interpolatedPercent, _percent)) return;

         if (_tween == null) return;
         if (_tween.IsActive()) _tween.StopAll();
         _tween.InterpolateProperty(this, nameof(_interpolatedPercent), null, _percent, 0.062f, Tween.TransitionType.Sine);
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
      _multiMeshInstance.Multimesh.SetInstanceTransform(0, _bgTransform);
      _multiMeshInstance.Multimesh.SetInstanceCustomData(0, _bgColor);
      // Bar
      _multiMeshInstance.Multimesh.SetInstanceTransform(1, _fgTransform);
      _multiMeshInstance.Multimesh.SetInstanceCustomData(1, _fgColor);
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

   public void OnDead() {
      _animationPlayer?.Play("die");
   }
}