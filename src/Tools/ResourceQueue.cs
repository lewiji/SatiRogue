using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Godot;
using SatiRogue.Debug;
using Thread = System.Threading.Thread;

namespace SatiRogue.Tools;

public class ResourceQueue : Resource {
   [Signal] public delegate void ResourceLoaded(Resource resource);
   [Signal] public delegate void AllLoaded();
   
   static readonly HashSet<string> _toLoad = new();

   public void QueueResource(string path) {
      _toLoad.Add(path);
   }

   public async void LoadQueuedResources() {
      while (_toLoad.Count > 0) {
         var path = _toLoad.First();
         _toLoad.Remove(path);
         var res = await Load<Resource>(path);
         EmitSignal(nameof(ResourceLoaded), res);
      }
      EmitSignal(nameof(AllLoaded));
   }
   
   static async Task<TResource?> Load<TResource>(string path, Delegates.ReportProgress? reportProgress = null)
      where TResource : Resource {
      if (ResourceLoader.HasCached(path)) {
         GD.Print("Is in cache: " + path);
         return ResourceLoader.Load<TResource>(path);
      }

      var reader = LoadInBackground<TResource>(path);

      while (await reader.WaitToReadAsync()) {
         if (!reader.TryRead(out var progress)) {
            continue;
         }

         if (progress.Finished) {
            return progress.Resource as TResource;
         }

         reportProgress?.Invoke(path, progress.Progress);
      }

      throw new InvalidOperationException("We should never be here.");
   }

   static ChannelReader<ResourceLoadingProgress<T>> LoadInBackground<T>(string path) where T : Resource {
      Logger.Debug($"Loading in background: {path}");

      var result = Channel.CreateUnbounded<ResourceLoadingProgress<T>>();

      Task.Run(async () => {
         var interactiveLoader = ResourceLoader.LoadInteractive(path);

         do {
            var error = interactiveLoader.Poll();

            if (error == Error.FileEof || error != Error.Ok) {
               if (error != Error.FileEof) {
                  GD.Print("Loading failed. Error: " + error);
               }
               // send resource
               await result.Writer.WriteAsync(new ResourceLoadingProgress<T>(path, (T) interactiveLoader.GetResource()));
               result.Writer.Complete();
               return;
            }

            // send a progress update
            await result.Writer.WriteAsync(new ResourceLoadingProgress<T>(path,
               interactiveLoader.GetStage() / (float) interactiveLoader.GetStageCount()));
         }
         while (true);
      });

      return result.Reader;
   }
}

public static class Delegates {
   public delegate ResourceLoadingProgress<Resource> ReportProgress(string path, float progress);
}

public class ResourceLoadingProgress<T> {
   public ResourceLoadingProgress(string path, Resource resource) {
      Path = path;
      Resource = resource;
      Finished = true;
      Progress = 1.0f;
   }

   public ResourceLoadingProgress(string path, float progress) {
      Path = path;
      Progress = progress;
      Finished = false;
      Resource = null;
   }

   public string Path { get; set; }
   public bool Finished { get; set; }
   public Resource? Resource { get; set; }
   public float Progress { get; set; }
}