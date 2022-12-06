using System.Collections.Generic;
using Godot;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Nodes;
using SatiRogue.Ecs.Dungeon.Triggers;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public partial class AudioSystem : ISystem {
   
   Dictionary<string, AudioStreamPlayer3D>? _audioStreams;

   public void Run(World world) {
      _audioStreams ??= new Dictionary<string, AudioStreamPlayer3D> {
         {"walk", world.GetElement<AudioNodes>().GetNode<AudioStreamPlayer3D>("Footsteps4")},
         {"sword", world.GetElement<AudioNodes>().GetNode<AudioStreamPlayer3D>("SwordWoosh1")}
      };

      foreach (var audioTrigger in world.Receive<CharacterAudioTrigger>(this)) {
         Play(audioTrigger.Audio, audioTrigger.Character.Position);
      }
   }

   void Play(string name, Vector3? translation = null) {
      if (!_audioStreams!.TryGetValue(name, out var audioStreamPlayer3D))
         return;

      if (audioStreamPlayer3D.Playing && audioStreamPlayer3D.HasMeta("Interruptable")
                                      && audioStreamPlayer3D.GetMeta("Interruptable").Obj is bool and false)
         return;

      audioStreamPlayer3D.Position = translation.GetValueOrDefault();

      if (audioStreamPlayer3D.HasMeta("VaryPitch") && audioStreamPlayer3D.GetMeta("VaryPitch").Obj is bool and true) {
         audioStreamPlayer3D.PitchScale = (float) GD.RandRange(0.9f, 1.1f);
         audioStreamPlayer3D.VolumeDb = (float) GD.RandRange(-0.8f, 0f);
      }

      audioStreamPlayer3D.Play();
   }
}