using Godot;
using SatiRogue.Ecs.Dungeon.Components;

namespace SatiRogue.Ecs.Dungeon.Nodes;

public partial class DebugUi : MarginContainer {
   [Signal] public delegate void WarpToStairsEventHandler();
   [Signal] public delegate void GodModeChangedEventHandler(bool enabled);
   [Signal] public delegate void HealPlayerEventHandler();
   
   Control? _bgContainer;
   Control? _controlsContainer;
   Button? _toggleDebugButton;
   Label? _playerPosLabel;
   Label? _stairsPosLabel;
   Label? _turnLabel;
   Button? _warpStairsButton;
   Button? _healButton;
   CheckButton? _godModeCheckButton;
   public bool Enabled { get; set; }

   public override void _Ready()
   {
	   _bgContainer = GetNode<Control>("%BgContainer");
	   _controlsContainer = GetNode<Control>("%ControlsContainer");
	   _toggleDebugButton = GetNode<Button>("%ToggleDebug");
		 _playerPosLabel = GetNode<Label>("%PlayerPos");
	   _stairsPosLabel = GetNode<Label>("%StairsPos");
		 _turnLabel = GetNode<Label>("%Turn");
	   _warpStairsButton = GetNode<Button>("%WarpToStairsButton");
		 _healButton = GetNode<Button>("%ReplenishHealthButton");
	   _godModeCheckButton = GetNode<CheckButton>("%GodModeCheckButton");
	   SetupScene();
   }

   void SetupScene() {
      _bgContainer?.Hide();
      _controlsContainer?.Hide();
      if (!Enabled) return;
      
      _toggleDebugButton?.Connect("pressed",new Callable(this,nameof(OnTogglePanel)));
      _warpStairsButton?.Connect("pressed",new Callable(this,nameof(OnWarpStairsPressed)));
      _healButton?.Connect("pressed",new Callable(this,nameof(OnHealPressed)));
      _godModeCheckButton?.Connect("toggled",new Callable(this,nameof(OnGodModeToggled)));
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

   public void SetTurn(TurnType turnType) {
      if (_turnLabel != null) _turnLabel.Text = turnType.ToString();
   }
}