using System.Collections.Generic;
using Godot;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Nodes;
using SatiRogue.Ecs.Dungeon.Triggers;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public class AudioSystem : ISystem {
   public World World { get; set; } = null!;
   Dictionary<string, AudioStreamPlayer3D>? _audioStreams;

   public void Run() {
      _audioStreams ??= new Dictionary<string, AudioStreamPlayer3D> {
         {"walk", World.GetElement<AudioNodes>().GetNode<AudioStreamPlayer3D>("Footsteps4")},
         {"sword", World.GetElement<AudioNodes>().GetNode<AudioStreamPlayer3D>("SwordWoosh1")}
      };

      foreach (var audioTrigger in this.Receive<CharacterAudioTrigger>()) {
         Play(audioTrigger.Audio, audioTrigger.Character.Translation);
      }
   }

   void Play(string name, Vector3? translation = null) {
      if (!_audioStreams!.TryGetValue(name, out var audioStreamPlayer3D))
         return;

      if (audioStreamPlayer3D.Playing && audioStreamPlayer3D.HasMeta("Interruptable")
                                      && audioStreamPlayer3D.GetMeta("Interruptable") is bool and false)
         return;

      audioStreamPlayer3D.Translation = translation.GetValueOrDefault();

      if (audioStreamPlayer3D.HasMeta("VaryPitch") && audioStreamPlayer3D.GetMeta("VaryPitch") is bool and true) {
         audioStreamPlayer3D.PitchScale = (float) GD.RandRange(0.9f, 1.1f);
         audioStreamPlayer3D.UnitDb = (float) GD.RandRange(-0.8f, 0f);
      }

      audioStreamPlayer3D.Play();
   }
}