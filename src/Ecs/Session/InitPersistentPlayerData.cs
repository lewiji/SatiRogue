using RelEcs;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Tools;

namespace SatiRogue.Ecs.Session;

public partial class InitPersistentPlayerData : ISystem {
   

   public void Run(World world) {
      var playerStore = new PersistentPlayerData();
      world.AddOrReplaceElement(playerStore);
   }
}