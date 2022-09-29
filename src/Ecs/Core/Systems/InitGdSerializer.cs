using Godot.Serialization;
using SatiRogue.Debug;
using RelEcs;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Core.Systems;

public class InitGdSerializer : ISystem {
   public World World { get; set; } = null!;

   public void Run() {
      Logger.Info("Initialising GDSerializer.");
      var serializer = new Serializer();
      World.AddElement(serializer);
   }
}