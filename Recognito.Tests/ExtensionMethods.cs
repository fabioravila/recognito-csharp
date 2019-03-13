using System;
using System.Collections.Generic;

namespace Recognito.Tests
{
    public static class ExtensionMethods
    {
        static Random rand = new Random();
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = list.Count - 1; i > 1; i--)
            {
                var rnd = rand.Next(i + 1);

                T val = list[rnd];
                list[rnd] = list[i];
                list[i] = val;
            }
        }
    }
}
