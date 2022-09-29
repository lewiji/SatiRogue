using Godot;
using Godot.Collections;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.Ecs.Play.Nodes.Hud;
using SatiRogue.Ecs.Play.Triggers;
using RelEcs;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Play.Systems;

public class CharacterDeathSystem : Reference, ISystem {
   public World World { get; set; } = null!;

   public void Run() {
      foreach (var charDiedTrigger in this.Receive<CharacterDiedTrigger>()) {
         charDiedTrigger.Character.Alive = false;

         if (charDiedTrigger.Character is Player player) {
            var timer = charDiedTrigger.Character.GetTree().CreateTimer(0.618f);
            timer.Connect("timeout", this, nameof(HandlePlayerDeath));
            player.AnimationPlayer.Play("on_death");
         } else {
            var timer = charDiedTrigger.Character.GetTree().CreateTimer(0.618f);
            timer.Connect("timeout", this, nameof(FreeEntity), new Array {charDiedTrigger.Character});
         }
      }
   }

   void FreeEntity(Character character) {
      var mapData = World.GetElement<MapGenData>();

      var entity = character.GetMeta("Entity") as Marshallable<Entity>;

      if (entity!.Value.IsNone)
         return;
      var gridPos = this.GetComponent<GridPositionComponent>(entity.Value);
      var currentCell = mapData.GetCellAt(gridPos.Position);
      currentCell.Occupants.Remove(character.GetInstanceId());
      this.DespawnAndFree(entity.Value);
   }

   void HandlePlayerDeath() {
      World.GetElement<DeathScreen>().FadeToDeath();
   }
}