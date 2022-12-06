using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using SatiRogue.Debug;
using RelEcs;
using World = RelEcs.World;
using SatiRogue.Tools;
using Thread = System.Threading.Thread;

namespace SatiRogue.Ecs.Loading.Systems;

public partial class PreloadResources : RefCounted, ISystem {
   

   [Signal]
   public delegate void ResourceLoadedEventHandler(Resource res);

   [Signal]
   public delegate void ResourceFailedEventHandler(string path);

   [Signal]
   public delegate void AllResourcesLoadedEventHandler();

   World? _world;

   static readonly string[] ResourcePaths = {
      "res://resources/level_meshes/dungeon_voxel.mesh",
      "res://resources/character_atlas_shader_material.tres",
      "res://scenes/ThreeDee/res/FogTileShaderMaterial.tres",
      "res://resources/hud/StatBar3DShaderMat.tres",
      "res://src/Ecs/Dungeon/Nodes/Items/HealthMaterial.tres",
      "res://resources/particles/blood_particle_set.tres",
      "res://src/Character/CharacterPositionMarkerMaterial.tres",
      "res://src/Ecs/Dungeon/Nodes/Items/ChestMaterial.tres",
      "res://assets/Super Pixel Objects 2021 Edition/PNG/outline_none/ankh_large/frame0000_spatial_material.tres",
      "res://src/Ecs/Dungeon/Nodes/Items/ChestSpatialMaterial.tres",
      "res://src/Ecs/Dungeon/Nodes/Items/coins_particles_set.tres",
      "res://src/Ecs/Dungeon/Nodes/Items/ArrowMaterial.tres",
      "res://resources/enemies/ratfolk/ratfolk_axe_spriteframes.tres",
      "res://resources/props/stairs_spriteframes.tres",
      "res://resources/enemies/harpy/harpy_blue_spriteframes.tres",
      "res://scenes/res/SpriteFramesPlayer.tres",
      "res://resources/enemies/fire_elemental/FireElementalSpriteFrames.tres",
      "res://src/Ecs/Dungeon/Nodes/Items/ChestSpriteFrames.tres"
   };
   readonly Stack<string> _resourcesToLoad = new(ResourcePaths);
   CompileShaders? _compileShaders;

   public void Run(World world)
   {
      _world ??= world;
      if (_resourcesToLoad.Count <= 0) {
         Logger.Info("Resources already loaded.");
         return;
      }
      CallDeferred(nameof(LoadAllResources));
   }

   void LoadAllResources() {
      var resQueue = _world!.GetElement<ResourceQueue>();
      resQueue.Connect(nameof(ResourceQueue.ResourceLoaded),new Callable(this,nameof(OnResourceLoaded)));
      resQueue.Connect(nameof(ResourceQueue.AllLoaded),new Callable(this,nameof(OnResourcesFinished)));
      foreach (var resourcePath in ResourcePaths) {
         resQueue.QueueResource(resourcePath);
      }
      resQueue.LoadQueuedResources();
   }

   void OnResourceLoaded(Resource res) {
      _compileShaders ??= _world!.GetElement<CompileShaders>();
      _compileShaders.OnResourceReceived(res);
   }

   void OnResourcesFinished() {
      Logger.Info("All resources loaded");
      var resQueue = _world!.GetElement<ResourceQueue>();
      resQueue.Disconnect(nameof(ResourceQueue.ResourceLoaded),new Callable(this,nameof(OnResourceLoaded)));
      resQueue.Disconnect(nameof(ResourceQueue.AllLoaded),new Callable(this,nameof(OnResourcesFinished)));
      EmitSignal(nameof(AllResourcesLoaded));
   }
}