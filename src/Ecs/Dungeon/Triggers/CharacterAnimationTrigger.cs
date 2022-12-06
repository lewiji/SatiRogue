using SatiRogue.Ecs.Dungeon.Nodes.Actors;
namespace SatiRogue.Ecs.Dungeon.Triggers;

public partial class CharacterAnimationTrigger {
   public Character Character;
   public string Animation;

   public CharacterAnimationTrigger(Character character, string animation) {
      Character = character;
      Animation = animation;
   }

   public void Deconstruct(out Character character, out string name) {
      character = Character;
      name = Animation;
   }
}