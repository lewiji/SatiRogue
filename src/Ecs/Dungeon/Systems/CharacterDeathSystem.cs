using Godot;
using Godot.Collections;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Ecs.Dungeon.Nodes.Hud;
using SatiRogue.Ecs.Dungeon.Triggers;
using SatiRogue.Ecs.MapGenerator.Components;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public class CharacterDeathSystem : Reference, ISystem {
   public World World { get; set; } = null!;

   public void Run() {
      foreach (var (entity, character) in this.QueryBuilder<Entity, Character>().Has<Dead>().Build()) {
         character.Alive = false;
         World.RemoveComponent<Dead>(entity.Identity);

         if (character is Player player) {
            var timer = character.GetTree().CreateTimer(0.618f);
            timer.Connect("timeout", this, nameof(HandlePlayerDeath));
            player.AnimationPlayer.Play("on_death");
         } else {
            var timer = character.GetTree().CreateTimer(character.Particles.Lifetime);
            timer.Connect("timeout", this, nameof(FreeEntity), new Array {character});
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