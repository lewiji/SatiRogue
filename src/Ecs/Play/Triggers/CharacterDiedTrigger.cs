using SatiRogue.Ecs.Play.Nodes.Actors;
using RelEcs;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Play.Triggers;

public class CharacterDiedTrigger {
   public Character Character;
   public Entity Entity;

   public CharacterDiedTrigger(Character character, Entity entity) {
      Character = character;
      Entity = entity;
   }
}