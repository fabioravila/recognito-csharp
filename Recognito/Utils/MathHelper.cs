using System;

namespace Recognito
{
    public class MathHelper
    {
        public static double Ulp(double value)
        {
            var bits = BitConverter.DoubleToInt64Bits(value);
            var nextValue = BitConverter.Int64BitsToDouble(bits + 1);
            var result = nextValue - value;

            return result;
        }
    }
}
