using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Components;
using SatiRogue.Components.Stats;
using SatiRogue.Entities;

namespace SatiRogue.scenes.Hud;

public partial class HealthStat : MarginContainer {
    [OnReadyGet("HBoxContainer/TextureProgress/CenterContainer/Label", Export = true, OrNull = true)] 
    private Label? HealthLabel { get; set; }
    
    [OnReadyGet("HBoxContainer/TextureProgress", Export = true, OrNull = true)] 
    private TextureProgress? Progress { get; set; }
    
    private StatHealthComponent? _playerHealthComponent;

    private void OnHealthChanged(int health) {
        if (Progress != null && _playerHealthComponent != null) Progress.Value = _playerHealthComponent.Value;
        if (HealthLabel != null) HealthLabel.Text = health.ToString();
    }

    public override void _Process(float delta) {
        if (Progress == null || _playerHealthComponent != null) return;
        _playerHealthComponent = EntityRegistry.Player?.GetComponent<StatHealthComponent>();
        if (_playerHealthComponent == null) return;
        
        // On first successful get of player health component, connect signals, set values
        _playerHealthComponent.Connect(nameof(StatsComponent.Changed), this, nameof(OnHealthChanged));
        Progress.MaxValue = _playerHealthComponent.MaxValue;
        Progress.Value = _playerHealthComponent.Value;
        Progress.MinValue = _playerHealthComponent.Value;
        OnHealthChanged(_playerHealthComponent.Value);
    }
}