using System.Collections.Generic;
using Godot;
using SatiRogue.Ecs.Play.Nodes;
using SatiRogue.Ecs.Play.Triggers;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems;

public class AudioSystem : GdSystem {
   Dictionary<string, AudioStreamPlayer3D>? _audioStreams;

   public override void Run() {
      _audioStreams ??= new Dictionary<string, AudioStreamPlayer3D> {
         {"walk", GetElement<AudioNodes>().GetNode<AudioStreamPlayer3D>("Footsteps4")},
         {"sword", GetElement<AudioNodes>().GetNode<AudioStreamPlayer3D>("SwordWoosh1")}
      };

      foreach (var audioTrigger in Receive<CharacterAudioTrigger>()) {
         Play(audioTrigger.Audio, audioTrigger.Character.Translation);
      }
   }

   void Play(string name, Vector3? translation = null) {
      if (!_audioStreams!.TryGetValue(name, out var audioStreamPlayer3D)) return;

      if (audioStreamPlayer3D.Playing && audioStreamPlayer3D.HasMeta("Interruptable")
                                      && audioStreamPlayer3D.GetMeta("Interruptable") is bool and false) return;

      audioStreamPlayer3D.Translation = translation.GetValueOrDefault();

      if (audioStreamPlayer3D.HasMeta("VaryPitch") && audioStreamPlayer3D.GetMeta("VaryPitch") is bool and true) {
         audioStreamPlayer3D.PitchScale = (float) GD.RandRange(0.9f, 1.1f);
         audioStreamPlayer3D.UnitDb = (float) GD.RandRange(-0.8f, 0f);
      }

      audioStreamPlayer3D.Play();
   }
}