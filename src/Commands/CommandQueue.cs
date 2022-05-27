using System.Collections.Generic;
using Godot;
using SatiRogue.Debug;

namespace SatiRogue.Commands;

public class CommandQueue {
   private readonly Queue<Command> _commands = new();

   public void Add(Command command) {
      _commands.Enqueue(command);
   }

   public Error ExecuteAll() {
      foreach (var command in _commands) {
         var err = command.Execute();

         if (err == Error.Ok) continue;
         Logger.Error($"CommandQueue: command {command} returned {err}");
         return err;
      }

      return Error.Ok;
   }
}