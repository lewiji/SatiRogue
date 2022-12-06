using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Dungeon.Systems;
namespace SatiRogue.Ecs.Dungeon.Nodes.Hud;

public partial class StairsConfirmation : Control {
   [Signal] public delegate void StairsConfirmedEventHandler();
   [Signal] public delegate void StairsCancelledEventHandler();

   PopupPanel _popupPanel = null!;
   Button _yesButton = null!;
   Button _noButton = null!;

   public override void _Ready()
   {
	   _popupPanel = GetNode<PopupPanel>("%PopupPanel");
	   _yesButton = GetNode<Button>("%YesButton");
	   _noButton = GetNode<Button>("%NoButton");
	   ConnectButtons();
   }

   void ConnectButtons() {
      _yesButton.Connect("pressed",new Callable(this,nameof(OnYesPressed)));
      _noButton.Connect("pressed",new Callable(this,nameof(OnNoPressed)));
   }

   void OnYesPressed() {
      Logger.Info("YES TO STEPS!");
      _popupPanel.Hide();
      EmitSignal(nameof(StairsConfirmed));
   }

   void OnNoPressed() {
      Logger.Info("NO TO STEPS!");
      _popupPanel.Hide();
      EmitSignal(nameof(StairsCancelled));
      InputSystem.Paused = false;
   }

   public void Popup() {
      _popupPanel?.PopupCenteredClamped(_popupPanel.MinSize);
   }
}