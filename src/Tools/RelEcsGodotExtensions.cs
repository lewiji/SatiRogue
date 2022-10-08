using System.Runtime.CompilerServices;
using Godot;
using RelEcs;
using SatiRogue.Ecs.Core;
using World = RelEcs.World;

namespace SatiRogue.Tools;

public static class RelEcsGodotExtensions {
   public static void AddOrReplaceElement<T>(this World world, T element) where T : class {
      if (world.HasElement<T>())
         world.ReplaceElement(element);
      else
         world.AddElement(element);
   }
   
   public static T? TryGetElement<T>(this World world) where T : class {
      return world.HasElement<T>() ? world.GetElement<T>() : null;
   }

   public static SystemGroup Add(this SystemGroup systemGroup, ISystem aSystem, World world) {
      systemGroup.Add(aSystem);
      aSystem.World = world;
      return systemGroup;
   }

   public static Entity? GetEntity(this Node node) {
      if (node.HasMeta("Entity") && node.GetMeta("Entity") is Marshallable<Entity> entity) {
         return entity.Value;
      }
      return null;
   }
}

public class SatiSystemGroup {
   readonly SystemGroup _systemGroup = new();
   readonly World _world;
   public SatiSystemGroup(GameStateController gsc) {
      _world = gsc.World;
   }
      
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public SatiSystemGroup Add(ISystem aSystem) {
      aSystem.World = _world;
      _systemGroup.Add(aSystem);
      return this;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Run(World world)
   {
      _systemGroup.Run(world);
   }
}