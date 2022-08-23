using Godot;
using RelEcs;
using SatiRogue.Ecs.MapGenerator.Triggers;
using SatiRogue.Ecs.Play.Nodes;
namespace SatiRogue.Ecs.Play.Systems;

public class AudioSystem : GdSystem {
   public override void Run() {
      foreach (var audioTrigger in Receive<CharacterAudioTrigger>()) {
         switch (audioTrigger.Audio) {
            case "walk":
               var footsteps = GetElement<AudioNodes>().GetNode<AudioStreamPlayer3D>("Footsteps4");
               footsteps.Translation = audioTrigger.Character.Translation;
               footsteps.Play();

               break;
            case "sword":
               var sword = GetElement<AudioNodes>().GetNode<AudioStreamPlayer3D>("SwordWoosh1");
               sword.Translation = audioTrigger.Character.Translation;
               sword.Play();

               break;
         }
      }
   }
}