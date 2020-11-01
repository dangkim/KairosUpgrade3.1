using System;

namespace Slot.Core.RandomNumberGenerators
{
    public class CryptoRandomNumberGenerator : IRandomNumberGenerator
    {
        private static readonly Lazy<RNGCryptoService> Random = new Lazy<RNGCryptoService>(() => new RNGCryptoService());

        public int Next()
        {
            return Random.Value.Next();
        }

        public int Next(int maxValue)
        {
            ++maxValue;
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(@"maxValue", new Exception("maxValue must be positive"));

            return Random.Value.Next(maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            if (maxValue == int.MaxValue)
                throw new ArgumentOutOfRangeException(@"maxValue", new Exception("maxValue is not valid"));
            if (minValue == maxValue)
                return minValue;

            ++maxValue;
            return Random.Value.Next(minValue, maxValue);
        }

        public double NextDouble()
        {
            return Random.Value.NextDouble();
        }
    }
}