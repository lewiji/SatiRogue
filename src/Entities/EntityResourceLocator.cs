using System.Collections.Generic;
using Godot;
using SatiRogue.Components;
using SatiRogue.Components.Behaviours;
using SatiRogue.scenes.Hud;

namespace SatiRogue.Entities;

/// <summary>
///    It's a struct that contains a dictionary of resource paths and a set of stats.
/// </summary>
public struct EntityResourceBundle {
   public EntityResourceBundle()
   {
      Components = new Component[] { };
      ResourcePaths = new Dictionary<string, string>();
   }

   public Dictionary<string, string> ResourcePaths { get; set; }
   public Component[] Components { get; set; }
}

/// <summary>
///    An enum containing all possible Enemy names.
/// </summary>
public enum EntityTypes {
   Maw,
   Ratfolk,
   Harpy
}

/// <summary>
///    A dictionary that maps an `EnemyTypes` enum to a `EnemyResourceBundle` class
/// </summary>
public static class EntityResourceLocator {
   public static Dictionary<EntityTypes, EntityResourceBundle> ResourceBundles = new() {
      {
         EntityTypes.Maw,
         new EntityResourceBundle {
            ResourcePaths = {
               {nameof(SpriteFrames), "res://resources/enemies/maw/maw_purple_sprite_Frames.tres"},
               {nameof(Material), "res://resources/enemies/maw/maw_purple_spatial_mat.tres"}
            },
            Components = new Component[] {new EnemyBehaviourTreeComponent()}
         }
      }, {
         EntityTypes.Ratfolk,
         new EntityResourceBundle {
            ResourcePaths = {
               {nameof(SpriteFrames), "res://resources/enemies/ratfolk/ratfolk_axe_spriteframes.tres"},
               {nameof(Material), "res://resources/enemies/ratfolk/ratfolk_axe_spatial_mat.tres"}
            },
            Components = new Component[] {new EnemyBehaviourTreeComponent()}
         }
      }, {
         EntityTypes.Harpy,
         new EntityResourceBundle {
            ResourcePaths = {
               {nameof(SpriteFrames), "res://resources/enemies/harpy/harpy_blue_spriteframes.tres"},
               {nameof(Material), "res://resources/enemies/harpy/harpy_blue_spatial_mat.tres"}
            },
            Components = new Component[] {new EnemyBehaviourTreeComponent()}
         }
      }
   };

   public static Dictionary<string, NodePath> SceneNodePaths = new ();

   public static readonly PackedScene StatBar3DScene = GD.Load<PackedScene>("res://scenes/Hud/StatBar3D.tscn");
   
   private static readonly Dictionary<EntityTypes, Dictionary<string, Resource>> _resourceCache = new();

   /// <summary>
   ///    If the resource is cached, return it. If it's not cached, load it from the resource bundle and cache it
   /// </summary>
   /// <param name="entity">The type of enemy you want to get the resource for.</param>
   /// <param name="resourceKey">
   ///    The key of the resource you want to load. If you don't specify this, it will default to the name of the type you're
   ///    trying to load.
   /// </param>
   /// <returns>
   ///    A resource of type T.
   /// </returns>
   public static T? GetResource<T>(EntityTypes entity, string? resourceKey = null) where T : Resource {
      resourceKey ??= typeof(T).Name;

      if (!_resourceCache.ContainsKey(entity))
         _resourceCache.Add(entity, new Dictionary<string, Resource>());

      if (_resourceCache[entity].TryGetValue(resourceKey, out var theResource))
         return theResource as T;

      if (ResourceBundles.ContainsKey(entity) && ResourceBundles[entity].ResourcePaths.TryGetValue(resourceKey, out var resourcePath)) {
         theResource = GD.Load<T>(resourcePath);
         _resourceCache[entity].Add(resourceKey, theResource);
      }

      return theResource as T;
   }

   public static IEnumerable<Component> GetComponents(EntityTypes entity) {
      return ResourceBundles.ContainsKey(entity) ? ResourceBundles[entity].Components : new Component[] { };
   }
}