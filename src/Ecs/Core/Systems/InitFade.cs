using Godot;
using SatiRogue.Ecs.Core.Nodes;
using RelEcs;
using SatiRogue.Tools;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Core.Systems;

public partial class InitFade : ISystem {
   
   static readonly PackedScene FadeScene = GD.Load<PackedScene>("res://src/Ecs/Core/Nodes/Fade.tscn");

   public void Run(World world) {
      var fade = FadeScene.Instance<Fade>();
      world.GetElement<CoreState>().AddChild(fade);
      world.AddOrReplaceElement(fade);
   }
}