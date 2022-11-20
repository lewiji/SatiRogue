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
   
   World? _world;

   public void Run(World world)
   {
      _world ??= world;
      foreach (var (entity, character) in world.Query<Entity, Character>().Has<Dead>().Build()) {
         character.Alive = false;
         world.RemoveComponent<Dead>(entity);

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
      var mapData = _world!.GetElement<MapGenData>();

      var entity = character.GetMeta("Entity") as Marshallable<Entity>;

      if (entity!.Value.IsNone)
         return;
      var gridPos = _world!.GetComponent<GridPositionComponent>(entity.Value);
      var currentCell = mapData.GetCellAt(gridPos.Position);
      currentCell.Occupants.Remove(character.GetInstanceId());
      _world!.DespawnAndFree(entity.Value);
   }

   void HandlePlayerDeath() {
      _world!.GetElement<DeathScreen>().FadeToDeath();
   }
}