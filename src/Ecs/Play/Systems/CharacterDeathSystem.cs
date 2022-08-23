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
            timer.Connect("timeout", this, nameof(HandlePlayerDeath));
            GetElement<Nodes.Actors.Player>().AnimationPlayer.Play("on_death");
         } else {
            var timer = charDiedTrigger.Character.GetTree().CreateTimer(0.618f);
            timer.Connect("timeout", this, nameof(FreeEntity), new Array {charDiedTrigger.Character});
         }
      }
   }

   private void FreeEntity(Character character) {
      var mapData = GetElement<MapGenData>();

      var entity = character.GetMeta("Entity") as Entity;
      var gridPos = GetComponent<GridPositionComponent>(entity);
      var currentCell = mapData.GetCellAt(gridPos.Position);
      currentCell.Occupants.Remove(character.GetInstanceId());
      DespawnAndFree(entity);
   }

   private void HandlePlayerDeath() {
      GetElement<DeathScreen>().FadeToDeath();
   }
}