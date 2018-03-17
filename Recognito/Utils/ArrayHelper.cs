using System;

namespace Recognito
{
    public static class ArrayHelper
    {
        public static T[] Copy<T>(T[] sourceArray, int length)
        {
            var destinationArray = new T[length];

            Array.Copy(sourceArray, destinationArray, length);

            return destinationArray;
        }

        public static T[] Copy<T>(T[] sourceArray)
        {
            return Copy(sourceArray, sourceArray.Length);
        }

        public static void Fill<T>(T[] sourceArray, T value)
        {
            for (int i = 0; i < sourceArray.Length; i++)
            {
                sourceArray[i] = value;
            }
        }

        public static void Fill<T>(T[] array, int start, int end, T value)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (start < 0 || start > end)
            {
                throw new ArgumentOutOfRangeException("fromIndex");
            }
            if (end >= array.Length)
            {
                throw new ArgumentOutOfRangeException("toIndex");
            }
            for (int i = start; i < end; i++)
            {
                array[i] = value;
            }
        }

        public static string ToString<T>(T[] list)
        {
            return "[" + string.Join(",", list) + "]";
        }
    }
}
