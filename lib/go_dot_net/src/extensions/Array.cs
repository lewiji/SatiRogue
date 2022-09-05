using System;
using System.Collections.Generic;
using Godot.Collections;
using Array = Godot.Collections.Array;
namespace SatiRogue.lib.go_dot_net.src.extensions;

/// <summary>
/// Extension on Godot arrays that allows them to be converted to dotnet
/// lists.
/// </summary>
public static class ArrayX {
   /// <summary>
   /// Converts a Godot array into a C# list. Runs in O(n), so try not to use
   /// on huge values.
   /// </summary>
   /// <param name="array">Godot array (receiver).</param>
   /// <typeparam name="T">Type of the items in the array.</typeparam>
   /// <returns>A dotnet list containing the Godot array contents.</returns>
   public static List<T> ToDotNet<T>(this Array array) {
      var list = new List<T>(array.Count);

      foreach (var item in array) {
         if (item is not T) {
            throw new Exception($"Array contains an item of type {item?.GetType()} " + $"but expected type {typeof(T)}");
         }
         list.Add((T) item);
      }
      return list;
   }

   /// <summary>
   /// Converts a Godot array into a C# list. Runs in O(n), so try not to use
   /// on huge values.
   /// </summary>
   /// <param name="array">Godot array (receiver).</param>
   /// <typeparam name="T">Type of the items in the array.</typeparam>
   /// <returns>A dotnet list containing the Godot array contents.</returns>
   public static List<T?> ToDotNet<T>(this Array<T?> array) {
      var list = new List<T?>(array.Count);

      foreach (dynamic? item in array) {
         if (item is not T and not null) {
            throw new Exception($"Array contains an item of type {item?.GetType()} " + $"but expected type {typeof(T)}");
         }
         list.Add((T?) item);
      }
      return list;
   }
}