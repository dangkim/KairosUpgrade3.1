namespace Slot.Core.RandomNumberGenerators
{
    public class RandomNumberEngine
    {
        private static readonly IRandomNumberGenerator instance = new CryptoRandomNumberGenerator();

        public static int Next()
        {
            return instance.Next();
        }

        public static int Next(int maxValue)
        {
            return instance.Next(maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return instance.Next(minValue, maxValue);
        }

        public static double NextDouble()
        {
            return instance.NextDouble();
        }
    }
}
