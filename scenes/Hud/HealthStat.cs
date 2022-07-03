using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Components;
using SatiRogue.Components.Stats;
using SatiRogue.Entities;

namespace SatiRogue.scenes.Hud;

public partial class HealthStat : MarginContainer
{
    private StatHealthComponent? _playerHealthComponent;

    [OnReadyGet("HBoxContainer/TextureProgress/CenterContainer/Label", Export = true, OrNull = true)] 
    private Label? _healthLabel { get; set; }
    
    [OnReadyGet("HBoxContainer/TextureProgress", Export = true, OrNull = true)] 
    private TextureProgress? _progress { get; set; }
    
    [OnReady(Order = 1)]
    private async void ConnectStatChangedSignal()
    {
        await ToSignal(GetTree(), "idle_frame");
        _playerHealthComponent = EntityRegistry.Player.GetComponent<StatHealthComponent>();
        _playerHealthComponent.Connect(nameof(StatsComponent.Changed), this, nameof(OnHealthChanged));
        _progress.MaxValue = _playerHealthComponent.MaxValue;
        _progress.Value = _playerHealthComponent.Value;
        _progress.MinValue = _playerHealthComponent.Value;
        OnHealthChanged(_playerHealthComponent.Value);
    }

    private void OnHealthChanged(int health)
    {
        _progress.Value = _playerHealthComponent.Value;
        if (_healthLabel != null) _healthLabel.Text = health.ToString();
    }
    
    

}