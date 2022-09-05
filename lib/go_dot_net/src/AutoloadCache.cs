using System;
using System.Collections.Generic;
using SatiRogue.lib.go_dot_net.src.extensions;

namespace SatiRogue.lib.go_dot_net.src;

/// <summary>
/// A static class used to cache autoloads whenever they are fetched. This
/// prevents <see cref="NodeX.Autoload{T}"/> from having to fetch the
/// root node children on every invocation.
/// </summary>
public class AutoloadCache {
   static readonly Dictionary<Type, object> _cache = new();

   /// <summary>
   /// Save a value to the cache.
   /// </summary>
   /// <param name="type">Type of the node.</param>
   /// <param name="value">Node to save.</param>
   public static void Write(Type type, object value) {
      _cache.Add(type, value);
   }

   /// <summary>
   /// Read a value from the cache.
   /// </summary>
   /// <param name="type">Type of the node.</param>
   /// <returns>Node in the cache.</returns>
   public static object Read(Type type) {
      return _cache[type];
   }

   /// <summary>
   /// Check if a value is in the cache.
   /// </summary>
   /// <param name="type">Type of the node.</param>
   /// <returns>True if the value is saved in the cache.</returns>
   public static bool Has(Type type) {
      return _cache.ContainsKey(type);
   }
}