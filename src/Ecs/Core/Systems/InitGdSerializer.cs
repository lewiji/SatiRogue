using Godot.Serialization;
using RelEcs;
using SatiRogue.Debug;
namespace SatiRogue.Ecs.Core.Systems; 

public class InitGdSerializer : GdSystem {

   public override void Run() {
      Logger.Info("Initialising GDSerializer.");
      var serializer = new Serializer();
      AddElement(serializer);
   }
}