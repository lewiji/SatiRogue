using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using SatiRogue.Debug;
using SatiRogue.lib.RelEcsGodot.src;
using SatiRogue.Tools;
namespace SatiRogue.Ecs.Loading.Systems;

public class PreloadResources : GdSystem {
   [Signal] public delegate void ResourceLoaded(Resource res);
   [Signal] public delegate void AllResourcesLoaded();

   static readonly string[] ResourcePaths = {
      "res://assets/overworld/WallShaderShadows.tres",
      "res://src/Ecs/Menu/Nodes/MenuBgShader.gdshader",
      "res://src/Ecs/Menu/Nodes/MenuButtonsPanelBgShader.gdshader",
      "res://resources/enemies/flash_overlay_shader.gdshader",
      "res://assets/overworld/WallShader.tres",
      "res://src/Ecs/Menu/Nodes/MenuButtonShaderMaterial.tres",
      "res://src/Ecs/Core/Nodes/FadeShaderMaterial.tres",
      "res://src/Ecs/Menu/Nodes/MenuButtonsPanelBgShaderMaterial.tres",
      "res://assets/overworld/PolySurfaceShader.tres",
      "res://resources/hud/StatBar3DShader.gdshader",
      "res://scenes/ThreeDee/res/FogTileShaderMaterial.tres",
      "res://resources/enemies/flash_overlay_shadermaterial.tres",
      "res://src/Ecs/Menu/Nodes/MenuBgShaderMaterial.tres",
      "res://resources/hud/StatBar3DShaderMat.tres",
      "res://src/Ecs/Menu/Nodes/MenuButtonShader.gdshader",
      "res://assets/overworld/OverworldMatShader.material",
      "res://src/Ecs/Play/Nodes/Items/HealthMaterial.tres",
      "res://assets/overworld/OverworldMat.material",
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
      "res://scenes/res/debug/DebugPortalMeshMaterial.tres",
      "res://src/Ecs/Play/Nodes/Items/ArrowMaterial.tres",
      "res://assets/heroic_items/ArrowOrigin/ArrowOrigin3_spatial_material.tres",
      "res://resources/enemies/ratfolk/ratfolk_axe_spriteframes.tres",
      "res://resources/props/stairs_spriteframes.tres",
      "res://resources/enemies/harpy/harpy_blue_spriteframes.tres",
      "res://scenes/res/SpriteFramesPlayer.tres",
      "res://resources/enemies/fire_elemental/FireElementalSpriteFrames.tres",
      "res://src/Ecs/Play/Nodes/Items/ChestSpriteFrames.tres"
   };
   readonly Stack<string> _resourcesToLoad = new(ResourcePaths);

   public override void Run() {
      if (_resourcesToLoad.Count <= 0) {
         Logger.Info("Resources already loaded.");
         return;
      }
      Logger.Info($"Preloading {ResourcePaths.Length} resources.");
      GetElement<LoadingState>().Connect(nameof(LoadingState.RequestNextResourceLoad), this, nameof(LoadNextResource));
      LoadNextResource();
   }

   async void LoadNextResource() {
      if (_resourcesToLoad.Count > 0) {
         var resourcePath = _resourcesToLoad.Pop();

         try {
            var state = GetElement<LoadingState>();
            var resource = await state.LoadAsync<Resource>(resourcePath);
            EmitSignal(nameof(ResourceLoaded), resource);
         }
         catch (AsyncResourceLoader.ResourceInteractiveLoaderException loaderException) {
            Logger.Error(loaderException.Message);
            GetElement<LoadingState>().EmitSignal(nameof(LoadingState.RequestNextResourceLoad));
         }
      } else {
         EmitSignal(nameof(AllResourcesLoaded));
      }
   }
}