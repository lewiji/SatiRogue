using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using SatiRogue.Debug;
using RelEcs;
using World = RelEcs.World;
using SatiRogue.Tools;
using Thread = System.Threading.Thread;

namespace SatiRogue.Ecs.Loading.Systems;

public class PreloadResources : Reference, ISystem {
   public World World { get; set; } = null!;

   [Signal]
   public delegate void ResourceLoaded(Resource res);

   [Signal]
   public delegate void ResourceFailed(string path);

   [Signal]
   public delegate void AllResourcesLoaded();

   static readonly string[] ResourcePaths = {
      "res://assets/overworld/WallShaderShadows.tres",
      "res://assets/overworld/WallShader.tres",
      "res://scenes/ThreeDee/res/FogTileShaderMaterial.tres",
      "res://resources/enemies/flash_overlay_shadermaterial.tres",
      "res://resources/hud/StatBar3DShaderMat.tres",
      "res://assets/overworld/OverworldMatShader.material",
      "res://src/Ecs/Dungeon/Nodes/Items/HealthMaterial.tres",
      "res://resources/particles/blood_particle_material.tres",
      "res://resources/enemies/fire_elemental/FireElementalSpatialMaterial.tres",
      "res://resources/enemies/harpy/harpy_blue_spatial_mat.tres",
      "res://resources/enemies/maw/maw_purple_spatial_mat.tres",
      "res://resources/enemies/ratfolk/ratfolk_axe_spatial_mat.tres",
      "res://resources/props/room_spatialmaterial.tres",
      "res://src/Character/CharacterPositionMarkerMaterial.tres",
      "res://src/Ecs/Dungeon/Nodes/Items/ChestMaterial.tres",
      "res://resources/particles/enemy_blood_spatial_material.tres",
      "res://assets/Super Pixel Objects 2021 Edition/PNG/outline_none/ankh_large/frame0000_spatial_material.tres",
      "res://src/Ecs/Dungeon/Nodes/Items/CoinsSpatialMaterial.tres",
      "res://src/Ecs/Dungeon/Nodes/Items/ChestSpatialMaterial.tres",
      "res://src/Ecs/Dungeon/Nodes/Items/CoinsParticlesMaterial.tres",
      "res://src/Ecs/Dungeon/Nodes/Items/ArrowMaterial.tres",
      "res://resources/enemies/ratfolk/ratfolk_axe_spriteframes.tres",
      "res://resources/props/stairs_spriteframes.tres",
      "res://resources/enemies/harpy/harpy_blue_spriteframes.tres",
      "res://scenes/res/SpriteFramesPlayer.tres",
      "res://resources/enemies/fire_elemental/FireElementalSpriteFrames.tres",
      "res://src/Ecs/Dungeon/Nodes/Items/ChestSpriteFrames.tres"
   };
   readonly Stack<string> _resourcesToLoad = new(ResourcePaths);
   Thread? _loadingThread;
   CompileShaders? _compileShaders;

   public void Run() {
      if (_resourcesToLoad.Count <= 0) {
         Logger.Info("Resources already loaded.");
         return;
      }

      _loadingThread = new Thread(LoadAllResources);
      _loadingThread.Start();
   }

   void LoadAllResources() {
      var resQueue = World.GetElement<ResourceQueue>();
      resQueue.Connect(nameof(ResourceQueue.ResourceLoaded), this, nameof(OnResourceLoaded));
      resQueue.Connect(nameof(ResourceQueue.AllLoaded), this, nameof(OnResourcesFinished));
      foreach (var resourcePath in ResourcePaths) {
         resQueue.QueueResource(resourcePath);
      }
      resQueue.LoadQueuedResources();
   }

   void OnResourceLoaded(Resource res) {
      _compileShaders ??= World.GetElement<CompileShaders>();
      _compileShaders.OnResourceReceived(res);
   }

   void OnResourcesFinished() {
      Logger.Info("All resources loaded");
      var resQueue = World.GetElement<ResourceQueue>();
      resQueue.Disconnect(nameof(ResourceQueue.ResourceLoaded), this, nameof(OnResourceLoaded));
      resQueue.Disconnect(nameof(ResourceQueue.AllLoaded), this, nameof(OnResourcesFinished));
      _loadingThread?.Join();
      _loadingThread = null;
      EmitSignal(nameof(AllResourcesLoaded));
   }
}