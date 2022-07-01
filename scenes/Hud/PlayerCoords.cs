using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Components;
using SatiRogue.Components.Stats;
using SatiRogue.Entities;

namespace SatiRogue.scenes.Hud; 

public partial class PlayerCoords : Label {
    private PlayerMovementComponent? _playerMovementComponent;
    [OnReady]
    private async void ConnectStatChangedSignal()
    {
        await ToSignal(GetTree(), "idle_frame");
        _playerMovementComponent = EntityRegistry.Player.GetComponent<PlayerMovementComponent>();
        _playerMovementComponent?.Connect(nameof(MovementComponent.PositionChanged), this, nameof(OnPositionChanged));
        OnPositionChanged();
    }

    private void OnPositionChanged() {
        Text = $"{_playerMovementComponent?.GridPosition.x}, {_playerMovementComponent?.GridPosition.z}";
    }
}