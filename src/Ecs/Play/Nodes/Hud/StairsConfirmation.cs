using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Debug;
namespace SatiRogue.Ecs.Play.Nodes.Hud;

public partial class StairsConfirmation : Control {
   [Signal] delegate void OnStairsConfirmed();
   [Signal] delegate void OnStairsCancelled();

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
      EmitSignal(nameof(OnStairsConfirmed));
   }

   void OnNoPressed() {
      Logger.Info("NO TO STEPS!");
      _popupPanel.Hide();
      EmitSignal(nameof(OnStairsCancelled));
   }

   public void Popup() {
      _popupPanel?.PopupCenteredClamped(_popupPanel.RectMinSize);
   }
}