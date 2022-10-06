using RelEcs;
using SatiRogue.Ecs.Core.Nodes;
namespace SatiRogue.Ecs.Session;

public class InitPersistentPlayerData : ISystem {
   public World World { get; set; }

   public void Run() {
      var playerStore = new PersistentPlayerData();
      World.AddElement(playerStore);
   }
}