using Godot.Serialization;
using SatiRogue.Debug;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Core.Systems;

public class InitGdSerializer : GdSystem {
   public override void Run() {
      Logger.Info("Initialising GDSerializer.");
      var serializer = new Serializer();
      AddElement(serializer);
   }
}