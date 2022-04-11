using System;
using MersenneTwister;

namespace RoguelikeMono.Tools;

public class Rng
{
    private static Random _mtRandom;
    
    public Rng(int? seed = null)
    {
        _mtRandom = MTRandom.Create(seed.GetValueOrDefault());
    }

    public static int IntRange(int from, int to)
    {
        return (_mtRandom.Next() % to) + from;
    }
}