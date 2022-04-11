using System;
using MersenneTwister;

namespace RoguelikeMono.Tools;

public class Rng
{
    private static Random? _mtRandom;
    
    public Rng(int? seed = null)
    {
        var theSeed = seed ?? (int)((DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks) / TimeSpan.TicksPerSecond);
        _mtRandom = MTRandom.Create(theSeed);
    }

    public static int IntRange(int from, int to)
    {
        if (_mtRandom == null) throw new Exception("Rng was not initialised or seeded.");
        return (_mtRandom.Next() % to) + from;
    }
}