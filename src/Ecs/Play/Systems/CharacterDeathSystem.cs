using Godot.Collections;
using RelEcs;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.MapGenerator.Triggers;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.Ecs.Play.Nodes.Hud;
namespace SatiRogue.Ecs.Play.Systems;

public class CharacterDeathSystem : GdSystem {
   public override void Run() {
      foreach (var charDiedTrigger in Receive<CharacterDiedTrigger>()) {
         charDiedTrigger.Character.Alive = false;

         if (charDiedTrigger.Character is Nodes.Actors.Player player) {
            var timer = charDiedTrigger.Character.GetTree().CreateTimer(0.618f);
            timer.Connect("timeout", this, nameof(HandlePlayerDeath), new Array {charDiedTrigger.Entity, player});
            GetElement<Nodes.Actors.Player>().AnimationPlayer.Play("on_death");
         } else {
            var timer = charDiedTrigger.Character.GetTree().CreateTimer(0.618f);
            timer.Connect("timeout", this, nameof(FreeEntity), new Array {charDiedTrigger.Entity, charDiedTrigger.Character});
         }
      }
   }

   private void FreeEntity(Entity entity, Character character) {
      var mapData = GetElement<MapGenData>();
      var gridPos = GetComponent<GridPositionComponent>(entity);
      var currentCell = mapData.GetCellAt(gridPos.Position);
      currentCell.Occupants.Remove(character.GetInstanceId());
      DespawnAndFree(entity);
   }

   private void HandlePlayerDeath(Entity entity, Nodes.Actors.Player player) {
      GetElement<Fade>().FadeToDeath();
   }
}