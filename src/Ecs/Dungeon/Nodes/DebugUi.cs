using Godot;
using GodotOnReady.Attributes;

namespace SatiRogue.Ecs.Dungeon.Nodes;

public partial class DebugUi : MarginContainer {
   [OnReadyGet("%BgContainer")]
   Control _bgContainer;

   [OnReadyGet("%ControlsContainer")]
   Control _controlscontainer;

   [OnReadyGet("%ToggleDebug")]
   Button _toggleDebugButton;

   [OnReadyGet("%PlayerPos")]
   Label _playerPosLabel;

   [OnReadyGet("%StairsPos")]
   Label _stairsPosLabel;

   [OnReadyGet("%WarpToStairsButton")]
   Button _warpStairsButton;

   [OnReadyGet("%ReplenishHealthButton")]
   Button _healButton;

   [OnReadyGet("%GodModeCheckButton")]
   CheckButton _godModeCheckButton;

   [OnReady]
   void SetupScene() {
      _bgContainer.Hide();
      _controlscontainer.Hide();
      _toggleDebugButton.Connect("pressed", this, nameof(OnTogglePanel));
   }

   void OnTogglePanel() {
      _bgContainer.Visible = !_bgContainer.Visible;
      _controlscontainer.Visible = !_controlscontainer.Visible;
   }

   public void SetPlayerPos(Vector3 pos) {
      _playerPosLabel.Text = $"({pos.x},{pos.z})";
   }

   public void SetStairsPos(Vector3 pos) {
      _stairsPosLabel.Text = $"({pos.x},{pos.z})";
   }
}