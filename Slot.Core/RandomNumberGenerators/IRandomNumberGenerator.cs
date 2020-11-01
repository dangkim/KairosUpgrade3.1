namespace Slot.Core.RandomNumberGenerators
{
    /// <summary>Represents a pseudo-random number generator, a device that produces a sequence of numbers that meet certain statistical requirements for randomness.</summary>
    public interface IRandomNumberGenerator
    {
        #region Public Methods and Operators

        /// <summary>Returns a nonnegative random integer.</summary>
        /// <returns>A 32-bit signed integer greater than or equal to zero and less than MaxValue.</returns>
        int Next();

        /// <summary>Returns a nonnegative random integer that is less than or equal to the specified maximum..</summary>
        /// <param name="maxValue">The inclusive upper bound of the random number to be generated. maxValue must be greater than or equal to zero.</param>
        /// <exception cref="ArgumentOutOfRangeException">maxValue is less than zero.</exception>
        /// <returns>A 32-bit signed integer greater than or equal to zero, and less than or equal to maxValue.</returns>
        int Next(int maxValue);

        /// <summary>Returns a random integer that is within a specified range.</summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The inclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <exception cref="ArgumentOutOfRangeException">minValue is greater than maxValue.</exception>
        /// <returns>A 32-bit signed integer greater than or equal to minValue and less than or equal to maxValue. If minValue equals maxValue, minValue is returned.</returns>
        int Next(int minValue, int maxValue);

        /// <summary>Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.</summary>
        /// <returns>A double-precision floating point number that is greater than or equal to 0.0, and less than 1.0.</returns>
        double NextDouble();

        #endregion
    }
}