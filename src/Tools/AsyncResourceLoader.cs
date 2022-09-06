using System;
using System.Threading.Tasks;
using Godot;
namespace SatiRogue.Tools;

public static class AsyncResourceLoader {
   public static async Task<T> LoadAsync<T>(this Node node, string path) where T : Resource {
      using var loader = ResourceLoader.LoadInteractive(path);
      Error err;

      do {
         err = loader.Poll();
         await node.ToSignal(node.GetTree(), "idle_frame");
      }
      while (err == Error.Ok);

      if (err != Error.FileEof) throw new ResourceInteractiveLoaderException(err);
      return (T) loader.GetResource();
   }

   public class ResourceInteractiveLoaderException : Exception {
      readonly Error _err;

      public ResourceInteractiveLoaderException(Error err) {
         _err = err;
      }

      public override string Message { get => $"ResourceInteractiveLoader failed with error: {_err}"; }
   }
}