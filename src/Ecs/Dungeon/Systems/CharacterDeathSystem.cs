using Godot;
using Godot.Collections;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Ecs.Dungeon.Nodes.Hud;
using SatiRogue.Ecs.Dungeon.Triggers;
using SatiRogue.Ecs.MapGenerator.Components;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

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