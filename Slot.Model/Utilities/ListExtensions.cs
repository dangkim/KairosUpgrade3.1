using System;
using System.Collections.Generic;

namespace Slot.Model.Utilities
{
    internal static class ListExtensions
    {
        public static void Append<T>(this List<T> combination, List<T> reel)
        {
            foreach (var symbol in reel)
            {
                if (combination.IndexOf(symbol) == -1)
                    combination.Add(symbol);
            }
        }
    }
}