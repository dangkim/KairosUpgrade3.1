using System;
using System.Security.Cryptography;

namespace Slot.Core.RandomNumberGenerators
{
    public class RNGCryptoService
    {
        private static readonly Lazy<RandomNumberGenerator> Rng =
        new Lazy<RandomNumberGenerator>(() => new RNGCryptoServiceProvider());
        //Returns: An int [0..2^32)
        private uint GenRandInt32()
        {
            var data = new byte[sizeof(uint)];
            Rng.Value.GetBytes(data);
            var randUint = BitConverter.ToUInt32(data, 0);
            return randUint;
        }

        //Returns: An int [0..2^31)
        private int GenRandInt31()
        {
            return (int)(GenRandInt32() >> 1);
        }

        /// <summary> Returns a random integer greater than or equal to zero and less than <c>MaxRandomInt</c> [0..Int32.MaxValue). </summary>
        /// <returns>The next random integer.</returns>
        public int Next()
        {
            return Next(int.MaxValue);
        }


        /// <summary>Returns a random integer within the specified range [minValue..maxValue).</summary>
        /// <param name="minValue">The lower bound.</param>
        /// <param name="maxValue">The upper bound.</param>
        /// <returns>A random integer greater than or equal to <c>minValue</c>, and less than <c>maxValue</c>.</returns>
        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                var tmp = maxValue;
                maxValue = minValue;
                minValue = tmp;
            }

            var range = maxValue - minValue;
            return Next(range) + minValue;
        }


        /// <summary>Returns a positive random integer less than the specified maximum [0..maxValue).</summary>
        /// <param name="maxValue">The maximum value. Must be greater than or equal to zero.</param>
        /// <returns>A positive random integer less than <c>maxValue</c>.</returns>
        public int Next(int maxValue)
        {
            if (maxValue == 0 || maxValue == 1) return 0;
            int randInt;
            do
            {
                randInt = GenRandInt31();
            } while (!IsFairInt(randInt, maxValue));
            return randInt % maxValue;
        }

        private static bool IsFairInt(int randInt, int maxValue)
        {
            var fullSetsOfValues = int.MaxValue / maxValue;
            return randInt < maxValue * fullSetsOfValues;
        }

        /// <summary> Returns a random number between 0.0 and 1.0 [0..1). </summary>
        /// <returns>A double-precision floating point number greater than or equal to 0.0,  and less than 1.0.</returns>
        public double NextDouble()
        {
            return GenRandInt32() / (uint.MaxValue + 1.0);
        }
    }
}
