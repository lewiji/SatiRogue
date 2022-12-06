using System;
using System.Collections.Generic;
using Godot;
using SatiRogue.Debug;

namespace SatiRogue.Tools;

public partial class ResourceQueue : Node {
   [Signal] public delegate void ResourceLoadedEventHandler(Resource resource);
   [Signal] public delegate void AllLoadedEventHandler();
   
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

      Connect(nameof(ResourceLoaded),new Callable(this,nameof(OnResourceLoaded)));
      Connect(nameof(AllLoaded),new Callable(this,nameof(OnQueueFinished)));
   }

   public void QueueResource(string path) {
      _toLoadMutex?.Lock();
      _toLoad.Enqueue(path);
      _toLoadMutex?.Unlock();
   }

   public void LoadQueuedResources() {
      if (_thread == null) 
         throw new Exception("Loading thread not initialised.");

      _thread.Start(Callable.From(LoaderThread), Thread.Priority.Low);
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
      if (_thread  != null && _thread.IsAlive()) {
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