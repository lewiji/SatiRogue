using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Debug;
namespace SatiRogue.Ecs.Play.Nodes.Hud;

public partial class StairsConfirmation : Control {
   [Signal] public delegate void StairsConfirmed();
   [Signal] public delegate void StairsCancelled();

   [OnReadyGet("%PopupPanel")] PopupPanel _popupPanel = null!;
   [OnReadyGet("%YesButton")] Button _yesButton = null!;
   [OnReadyGet("%NoButton")] Button _noButton = null!;

   [OnReady] void ConnectButtons() {
      _yesButton.Connect("pressed", this, nameof(OnYesPressed));
      _noButton.Connect("pressed", this, nameof(OnNoPressed));
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
   }

   public void Popup() {
      _popupPanel?.PopupCenteredClamped(_popupPanel.RectMinSize);
   }
}