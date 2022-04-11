using System;
using EcsRx.Components;
using SystemsRx.ReactiveData;

namespace RoguelikeMono.Tools.Components;

public class CanLogComponent : IComponent, IDisposable
{
    public ReactiveProperty<string>? Message { get; set; }

    public CanLogComponent()
    { Message = new ReactiveProperty<string>(); }

    public void Dispose()
    {
        Message = null;
    }
}