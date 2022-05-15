using System.Collections.Generic;
using Godot;

namespace SatiRogue.Entities;

/// <summary>
/// It's a struct that contains a dictionary of resource paths and a set of stats.
/// </summary>
public struct EnemyResourceBundle {
   public Dictionary<string, string> ResourcePaths { get; set; } = new();
   public EntityStats Stats { get; set; }
}

/// <summary>
/// An enum containing all possible Enemy names.
/// </summary>
public enum EnemyTypes {
   Maw,
   Ratfolk,
   Harpy
}

/// <summary>
/// A dictionary that maps an `EnemyTypes` enum to a `EnemyResourceBundle` class
/// </summary>
public static class EnemyResourceLocator {
   public static Dictionary<EnemyTypes, EnemyResourceBundle> ResourceBundles = new() {
      {
         EnemyTypes.Maw,
         new EnemyResourceBundle {
            ResourcePaths = {
               {nameof(SpriteFrames), "res://resources/enemies/maw/maw_purple_sprite_Frames.tres"},
               {nameof(Material), "res://resources/enemies/maw/maw_purple_spatial_mat.tres"}
            },
            Stats = new EntityStats {MaxHealth = 3, Level = 2}
         }
      }, {
         EnemyTypes.Ratfolk,
         new EnemyResourceBundle {
            ResourcePaths = {
               {nameof(SpriteFrames), "res://resources/enemies/ratfolk/ratfolk_axe_spriteframes.tres"},
               {nameof(Material), "res://resources/enemies/ratfolk/ratfolk_axe_spatial_mat.tres"}
            },
            Stats = new EntityStats {MaxHealth = 1, Level = 1}
         }
      }, {
         EnemyTypes.Harpy,
         new EnemyResourceBundle {
            ResourcePaths = {
               {nameof(SpriteFrames), "res://resources/enemies/harpy/harpy_blue_spriteframes.tres"},
               {nameof(Material), "res://resources/enemies/harpy/harpy_blue_spatial_mat.tres"}
            },
            Stats = new EntityStats {MaxHealth = 4, Level = 3}
         }
      }
   };

   private static readonly Dictionary<EnemyTypes, Dictionary<string, Resource>> _resourceCache = new();


   /// <summary>
   ///    If the resource is cached, return it. If it's not cached, load it from the resource bundle and cache it
   /// </summary>
   /// <param name="enemy">The type of enemy you want to get the resource for.</param>
   /// <param name="resourceKey">
   ///    The key of the resource you want to load. If you don't specify this, it will default to the name of the type you're
   ///    trying to load.
   /// </param>
   /// <returns>
   ///    A resource of type T.
   /// </returns>
   public static T? GetResource<T>(EnemyTypes enemy, string? resourceKey = null) where T : Resource {
      resourceKey ??= typeof(T).Name;

      if (!_resourceCache.ContainsKey(enemy)) 
         _resourceCache.Add(enemy, new Dictionary<string, Resource>());

      if (_resourceCache[enemy].TryGetValue(resourceKey, out var theResource))
         return theResource as T;

      if (ResourceBundles.ContainsKey(enemy) && ResourceBundles[enemy].ResourcePaths.TryGetValue(resourceKey, out var resourcePath)) {
         theResource = GD.Load<T>(resourcePath);
         _resourceCache[enemy].Add(resourceKey, theResource);
      }

      return theResource as T;
   }
}