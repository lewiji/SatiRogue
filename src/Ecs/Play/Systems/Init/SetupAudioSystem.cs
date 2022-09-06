using Godot;
using SatiRogue.Ecs.Play.Nodes;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems.Init;

public class SetupAudioSystem : GdSystem {
   static readonly AudioStream Ambience = GD.Load<AudioStream>("res://audio/Ambience Dark Chamber Loop.ogg");

   public override void Run() {
      var audioNodes = new AudioNodes();
      GetElement<PlayState>().AddChild(audioNodes);
      AddElement(audioNodes);

      var ambienceAudio = new AudioStreamPlayer {
         Stream = Ambience,
         Bus = "ambience",
         Autoplay = true
      };
      audioNodes.AddChild(ambienceAudio);
      Spawn(ambienceAudio).Add<AmbientAudio>();

      AddSfx(audioNodes, "Footsteps4", "res://audio/Barefoot Dirt footsteps 4.wav", "footsteps", true, 0.5f, false);
      AddSfx(audioNodes, "SwordWoosh1", "res://audio/Sword Woosh 1.wav", "sfx");
   }

   void AddSfx(AudioNodes audioNodes,
      string name,
      string streamPath,
      string bus,
      bool varyPitch = false,
      float unitSize = 5f,
      bool interruptable = true) {
      var footstepAudio = new AudioStreamPlayer3D {
         Name = name,
         Stream = GD.Load<AudioStream>(streamPath),
         Bus = bus,
         Autoplay = false,
         UnitSize = unitSize,
         AttenuationFilterCutoffHz = 7000
      };
      footstepAudio.SetMeta("VaryPitch", varyPitch);
      footstepAudio.SetMeta("Interruptable", interruptable);
      audioNodes.AddChild(footstepAudio);
   }
}

public class AmbientAudio { }
public class FootstepAudio { }