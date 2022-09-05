using Godot;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Core.Systems;

public partial class InitFade : GdSystem {
   static readonly PackedScene FadeScene = GD.Load<PackedScene>("res://src/Ecs/Core/Nodes/Fade.tscn");

   public override void Run() {
      var fade = FadeScene.Instance<Fade>();
      World.GetElement<CoreState>().AddChild(fade);
      World.AddElement(fade);
   }
}