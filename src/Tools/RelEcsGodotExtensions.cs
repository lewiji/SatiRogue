using RelEcs;
using SatiRogue.Ecs.Core;

namespace SatiRogue.Tools;

public static class RelEcsGodotExtensions {
   public static void AddOrReplaceElement<T>(this World world, T element) where T : class {
      if (world.HasElement<T>())
         world.ReplaceElement(element);
      else
         world.AddElement(element);
   }
}