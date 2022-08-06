using Godot.Collections;
using RelEcs;
using SatiRogue.Ecs.MapGenerator.Triggers;

namespace SatiRogue.Ecs.Play.Systems; 

public class CharacterDeathSystem : GDSystem {
   public override void Run() {
      foreach (var charDiedTrigger in Receive<CharacterDiedTrigger>()) {
         var timer = charDiedTrigger.Character.GetTree().CreateTimer(0.618f);
         timer.Connect("timeout", this, nameof(FreeEntity), new Array{charDiedTrigger.Entity});
      }
   }

   private void FreeEntity(Entity entity) {
      DespawnAndFree(entity);
   }
}