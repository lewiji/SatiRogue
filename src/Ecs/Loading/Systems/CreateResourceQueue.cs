using Godot;
using RelEcs;
using SatiRogue.Tools;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Loading.Systems; 

public class CreateResourceQueue : Reference, ISystem {
   

   public void Run(World world) {
      var resQueue = new ResourceQueue();
      world.AddOrReplaceElement(resQueue);
      world.GetElement<LoadingState>().AddChild(resQueue);
   }
}