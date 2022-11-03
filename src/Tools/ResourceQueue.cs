using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using SatiRogue.Debug;

namespace SatiRogue.Tools;

public class ResourceQueue : Node {
   [Signal] public delegate void ResourceLoaded(Resource resource);
   [Signal] public delegate void AllLoaded();
   
   readonly Queue<string> _toLoad = new();
   Thread? _thread;
   Mutex? _toLoadMutex;
   Semaphore? _toLoadSemaphore;
   bool _exitThread = false;

   public override void _Ready() {
      _toLoadMutex = new();
      _toLoadSemaphore = new();
      _exitThread = false;
      _thread = new();

      Connect(nameof(ResourceLoaded), this, nameof(OnResourceLoaded));
      Connect(nameof(AllLoaded), this, nameof(OnQueueFinished));
   }

   public void QueueResource(string path) {
      _toLoadMutex?.Lock();
      _toLoad.Enqueue(path);
      _toLoadMutex?.Unlock();
   }

   public void LoadQueuedResources() {
      if (_thread == null) 
         throw new Exception("Loading thread not initialised.");

      _thread.Start(this, nameof(LoaderThread), null, Thread.Priority.Low);
      _toLoadSemaphore?.CallDeferred("post");
   }

   void LoaderThread() {
      Logger.Info("Loader thread running...");

      while (true) {
         _toLoadSemaphore?.Wait();
         
         _toLoadMutex?.Lock();
         var shouldExit = _exitThread;
         _toLoadMutex?.Unlock();

         if (shouldExit) 
            break;
         
         _toLoadMutex?.Lock();
         var toLoadCounter = _toLoad.Count;
         _toLoadMutex?.Unlock();
         
         if (toLoadCounter > 0) {
            _toLoadMutex?.Lock();
            var path = _toLoad.Dequeue();
            _toLoadMutex?.Unlock();
            var res = ResourceLoader.Load(path);
            //EmitSignal(nameof(ResourceLoaded), res);
            CallDeferred("emit_signal", nameof(ResourceLoaded), res);
         } else {
            // check if exit was requested during load (i.e. app quit)
            _toLoadMutex?.Lock();
            var forceExit = _exitThread;
            _toLoadMutex?.Unlock();

            if (!forceExit) 
               CallDeferred("emit_signal", nameof(AllLoaded));
         }
      }
   }

   void OnResourceLoaded(Resource? resource) {
      // check queue for next resource
      _toLoadSemaphore?.Post();
   }

   public override void _ExitTree() {
      if (_thread  != null && _thread.IsActive()) {
         OnQueueFinished();
      }
   }

   void OnQueueFinished() {
      // exit thread
      _toLoadMutex?.Lock();
      _exitThread = true;
      _toLoadMutex?.Unlock();
      _toLoadSemaphore?.Post();
      _thread?.WaitToFinish();
      Logger.Info("Loading queue finished.");
   }
}