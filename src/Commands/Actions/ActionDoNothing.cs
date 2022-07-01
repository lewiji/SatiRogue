using System;
using Godot;
using SatiRogue.Components;
using SatiRogue.Components.Render;
using SatiRogue.Debug;
using SatiRogue.Entities;

namespace SatiRogue.Commands.Actions; 

public class ActionDoNothing : Action {
    public ActionDoNothing(Entity owner) : base(owner)
    { }

    public override Error Execute() {
        Logger.Info($"{Owner!.Name} doing nothing.");
        return Error.Ok;
    }
}