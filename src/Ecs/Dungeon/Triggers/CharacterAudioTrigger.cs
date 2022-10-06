using SatiRogue.Ecs.Dungeon.Nodes.Actors;
namespace SatiRogue.Ecs.Dungeon.Triggers;

public class CharacterAudioTrigger {
   public string Audio;
   public Character Character;

   public CharacterAudioTrigger(Character character, string audio) {
      Character = character;
      Audio = audio;
   }
}