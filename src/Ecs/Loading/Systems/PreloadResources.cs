using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using SatiRogue.Debug;
using RelEcs;
using World = RelEcs.World;
using SatiRogue.Tools;

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
      "res://src/Ecs/Play/Nodes/Items/HealthMaterial.tres",
      "res://resources/particles/blood_particle_material.tres",
      "res://resources/enemies/fire_elemental/FireElementalSpatialMaterial.tres",
      "res://resources/props/room_spatialmaterial.tres",
      "res://src/Character/CharacterPositionMarkerMaterial.tres",
      "res://src/Ecs/Play/Nodes/Items/ChestMaterial.tres",
      "res://resources/particles/enemy_blood_spatial_material.tres",
      "res://assets/Super Pixel Objects 2021 Edition/PNG/outline_none/ankh_large/frame0000_spatial_material.tres",
      "res://src/Ecs/Play/Nodes/Items/CoinsSpatialMaterial.tres",
      "res://src/Ecs/Play/Nodes/Items/ChestSpatialMaterial.tres",
      "res://src/Ecs/Play/Nodes/Items/CoinsParticlesMaterial.tres",
      "res://src/Ecs/Play/Nodes/Items/ArrowMaterial.tres",
      "res://resources/enemies/ratfolk/ratfolk_axe_spriteframes.tres",
      "res://resources/props/stairs_spriteframes.tres",
      "res://resources/enemies/harpy/harpy_blue_spriteframes.tres",
      "res://scenes/res/SpriteFramesPlayer.tres",
      "res://resources/enemies/fire_elemental/FireElementalSpriteFrames.tres",
      "res://src/Ecs/Play/Nodes/Items/ChestSpriteFrames.tres"
   };
   readonly Stack<string> _resourcesToLoad = new(ResourcePaths);
   private int _resourcesLoaded = 0;
   private int _resourcesFailed = 0;
   private int _resourceCount = 0;

   public void Run() {
      if (_resourcesToLoad.Count <= 0) {
         Logger.Info("Resources already loaded.");
         return;
      }
      Logger.Info($"Preloading {ResourcePaths.Length} resources.");
      Connect(nameof(ResourceLoaded), this, nameof(OnResourceLoaded));
      Connect(nameof(ResourceFailed), this, nameof(OnResourceFailed));
      _resourceCount = _resourcesToLoad.Count;
      
      while (_resourcesToLoad.Count > 0) {
         LoadNextResource();
      }
   }

   async Task<Resource?> LoadNextResource() {
      if (_resourcesToLoad.Count > 0) {
         var resourcePath = _resourcesToLoad.Pop();

         try {
            var resource = await ResourceQueue.Load<Resource>(resourcePath);
            EmitSignal(nameof(ResourceLoaded), resource);
            return resource;
         }
         catch (AsyncResourceLoader.ResourceInteractiveLoaderException loaderException) {
            Logger.Error(loaderException.Message);
            EmitSignal(nameof(ResourceFailed), resourcePath);
            //World.GetElement<LoadingState>().EmitSignal(nameof(LoadingState.RequestNextResourceLoad));
         }
      }
      return null;
   }

   void OnResourceLoaded(Resource res) {
      _resourcesLoaded += 1;
      CheckResourcesFinished();
   }

   void OnResourceFailed(string path) {
      _resourcesFailed += 1;
      CheckResourcesFinished();
   }

   void CheckResourcesFinished() {
      if (_resourcesFailed + _resourcesLoaded < _resourceCount) return;
      Logger.Info("All resources loaded");
      EmitSignal(nameof(AllResourcesLoaded));
   }
}