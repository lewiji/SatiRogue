using System;
using System.Collections.Generic;
using System.Linq;

namespace SatiRogue.Tools; 

public static class NumberExtensions {
    public static bool Divides(this int potentialFactor, int i)
    {
        return i % potentialFactor == 0;
    }

    public static IEnumerable<int> Factors(this int i)
    {
        return from potentialFactor in Enumerable.Range(1, i)
            where potentialFactor.Divides(i)
            select potentialFactor;
    }
    
    public static int GetMedian(this IEnumerable<int> source)
    {
        // Create a copy of the input, and sort the copy
        int[] temp = source.ToArray();    
        Array.Sort(temp);

        int count = temp.Length;
        if (count == 0)
        {
            throw new InvalidOperationException("Empty collection");
        }
        else
        {
            // count is odd, return the middle element
            return temp[count / 2];
        }
    }
}