using Godot;
using GodotOnReady.Attributes;
namespace SatiRogue.Ecs.Play.Nodes.Hud;

public partial class Loot : Control {
   private int _numLoots;
   [OnReadyGet("HBoxContainer/RichTextLabel")] private RichTextLabel _richTextLabel = null!;
   public int NumLoots {
      get => _numLoots;
      set {
         _numLoots = value;
         _richTextLabel.BbcodeText = $" {_numLoots}";
      }
   }
}