using Godot;
using GodotOnReady.Attributes;
namespace SatiRogue.Ecs.Play.Nodes.Hud;

public partial class Inventory : Control {
   [OnReadyGet("AnimationPlayer")] private AnimationPlayer? _animationPlayer;
   public bool IsOpen;

   public void Open() {
      if (IsOpen) return;
      _animationPlayer?.Play("open");
      IsOpen = true;
   }

   public void Close() {
      if (!IsOpen) return;
      _animationPlayer?.Play("close");
      IsOpen = false;
   }
}