using SatiRogue.Ecs.Play.Nodes.Actors;
namespace SatiRogue.Ecs.MapGenerator.Triggers;

public class CharacterAudioTrigger {
   public string Audio;
   public Character Character;

   public CharacterAudioTrigger(Character character, string audio) {
      Character = character;
      Audio = audio;
   }
}