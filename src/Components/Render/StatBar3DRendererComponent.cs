using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Components.Stats;
using SatiRogue.Entities;
using SatiRogue.scenes;
using SatiRogue.scenes.Hud;

namespace SatiRogue.Components.Render; 

public partial class StatBar3DRendererComponent : SpatialRendererComponent {
   private StatBar3D? _statBar3D;
   private StatsComponent? _statsComponent;
   protected override void CreateVisualNodes() {
      _statBar3D = (StatBar3D) EntityResourceLocator.StatBar3DScene.Instance();
      //_statBar3D.Scale = new Vector3(0.05f,0.05f,0.05f);
      RootNode?.AddChild(_statBar3D);
      _statBar3D.Translate(new Vector3(0, GridEntity.GetComponent<EnemyMeshRendererComponent>().YOffset, 0));
      SetupEntityDataBindings();
   }
   
   private void SetupEntityDataBindings() {
      /* Getting the health component from the parent entity. */
      var statComponent = GridEntity?.GetComponent<StatHealthComponent>();
      _statsComponent = statComponent;
      statComponent?.Connect(nameof(StatsComponent.Changed), this, nameof(OnStatChanged));
      if (statComponent != null) OnStatChanged(statComponent.Value);
   }
   
   private void OnStatChanged(int newValue) {
      if (_statsComponent != null && _statBar3D != null) _statBar3D.Percent = (float) newValue / _statsComponent.MaxValue;
   }
   
   [OnReady]
   private void ConnectSignals() {
      GridEntity?.Connect(nameof(Entity.Died), this, nameof(OnDead));
   }

   private void OnDead() {
      var tween = new Tween();
      RootNode.AddChild(tween);
      _statBar3D.ShaderMaterial.SetShaderParam("emission", Colors.Red);
      tween.InterpolateProperty(_statBar3D.ShaderMaterial, 
         "shader_param/emission_energy", 
         _statBar3D.ShaderMaterial.GetShaderParam("shader_param/emission_energy"), 
         1.618f,
         0.1f,
         Tween.TransitionType.Expo, 
         Tween.EaseType.In);
      tween.InterpolateProperty(_statBar3D.ShaderMaterial, 
         "shader_param/emission_energy", 
         1.618f, 
         0f,
         0.1f,
         Tween.TransitionType.Expo, 
         Tween.EaseType.In,
         0.1f);
      tween.InterpolateProperty(_statBar3D.ShaderMaterial, 
         "shader_param/emission_energy", 
         0f, 
         1.618f,
         0.1f,
         Tween.TransitionType.Expo, 
         Tween.EaseType.In,
         0.2f);
      tween.InterpolateProperty(_statBar3D.ShaderMaterial, 
         "shader_param/emission_energy", 
         1.618f, 
         0f,
         0.1f,
         Tween.TransitionType.Expo, 
         Tween.EaseType.In,
         0.4f);
      tween.InterpolateProperty(_statBar3D,
         "scale", 
         null, 
         Vector3.Zero,
         0.12f,
         Tween.TransitionType.Cubic, 
         Tween.EaseType.In,
         0.26f);
      tween.Start();
   }
}