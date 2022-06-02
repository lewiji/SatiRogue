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
      _statBar3D.OnDead();
   }
}