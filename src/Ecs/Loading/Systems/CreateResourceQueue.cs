using Godot;
using RelEcs;
using SatiRogue.Tools;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Loading.Systems; 

public class CreateResourceQueue : Reference, ISystem {
   public World World { get; set; }

   public void Run() {
      var resQueue = new ResourceQueue();
      World.AddOrReplaceElement(resQueue);
   }
}