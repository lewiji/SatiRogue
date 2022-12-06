using RelEcs;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
namespace SatiRogue.Ecs.Dungeon.Triggers;

public partial class CharacterDiedTrigger {
   public Character Character;
   public Entity Entity;

   public CharacterDiedTrigger(Character character, Entity entity) {
      Character = character;
      Entity = entity;
   }
}