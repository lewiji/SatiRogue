using Godot;
using GodotOnReady.Attributes;

namespace SatiRogue.Ecs.Dungeon.Nodes;

public partial class DebugUi : MarginContainer {
   [Signal] public delegate void WarpToStairs();
   [Signal] public delegate void GodModeChanged(bool enabled);
   [Signal] public delegate void HealPlayer();
   
   [OnReadyGet("%BgContainer")] Control? _bgContainer;
   [OnReadyGet("%ControlsContainer")] Control? _controlsContainer;
   [OnReadyGet("%ToggleDebug")] Button? _toggleDebugButton;
   [OnReadyGet("%PlayerPos")] Label? _playerPosLabel;
   [OnReadyGet("%StairsPos")] Label? _stairsPosLabel;
   [OnReadyGet("%WarpToStairsButton")] Button? _warpStairsButton;
   [OnReadyGet("%ReplenishHealthButton")] private Button? _healButton;
   [OnReadyGet("%GodModeCheckButton")] CheckButton? _godModeCheckButton;

   [OnReady] void SetupScene() {
      _bgContainer?.Hide();
      _controlsContainer?.Hide();
      _toggleDebugButton?.Connect("pressed", this, nameof(OnTogglePanel));
      _warpStairsButton?.Connect("pressed", this, nameof(OnWarpStairsPressed));
      _healButton?.Connect("pressed", this, nameof(OnHealPressed));
      _godModeCheckButton?.Connect("toggled", this, nameof(OnGodModeToggled));
   }

   void OnWarpStairsPressed() {
      EmitSignal(nameof(WarpToStairs));
   }
   void OnHealPressed() {
      EmitSignal(nameof(HealPlayer));
   }

   void OnGodModeToggled(bool isToggled) {
      EmitSignal(nameof(GodModeChanged), isToggled);
   }

   void OnTogglePanel() {
      if (_bgContainer != null) _bgContainer.Visible = !_bgContainer.Visible;
      if (_controlsContainer != null) _controlsContainer.Visible = !_controlsContainer.Visible;
   }

   public void SetPlayerPos(Vector3 pos) {
      if (_playerPosLabel != null) _playerPosLabel.Text = $"({pos.x},{pos.z})";
   }

   public void SetStairsPos(Vector3 pos) {
      if (_stairsPosLabel != null) _stairsPosLabel.Text = $"({pos.x},{pos.z})";
   }
}