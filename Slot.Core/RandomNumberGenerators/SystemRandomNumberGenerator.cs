// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RandomGenerator.cs" company="Vista Technology Capital Pte Ltd">
//   Copyright (c) Vista Technology Capital Pte Ltd. All rights reserved.
// </copyright>
// <summary>
//   The random generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Slot.Core.RandomNumberGenerators
{
    using System;

    /// <summary>Represents a pseudo-random number generator, a device that produces a sequence of numbers that meet certain statistical requirements for randomness.</summary>
    public class SystemRandomNumberGenerator : IRandomNumberGenerator
    {
        #region Static Fields

        /// <summary>The random seed.</summary>
        private static readonly Random RandomSeed = new Random((int)DateTime.Now.Ticks);

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="SystemRandomNumberGenerator"/> class.</summary>
        public SystemRandomNumberGenerator()
        {
            this.Random = new Random(((int)DateTime.Now.Ticks) + RandomSeed.Next());
        }

        #endregion

        #region Properties

        /// <summary>Gets or sets the random.</summary>
        private Random Random { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Returns a nonnegative random integer.</summary>
        /// <returns>A 32-bit signed integer greater than or equal to zero and less than MaxValue.</returns>
        public int Next()
        {
            return this.Random.Next();
        }

        /// <summary>Returns a nonnegative random integer that is less than or equal to the specified maximum..</summary>
        /// <param name="maxValue">The inclusive upper bound of the random number to be generated. maxValue must be greater than or equal to zero.</param>
        /// <exception cref="ArgumentOutOfRangeException">maxValue is less than zero.</exception>
        /// <returns>A 32-bit signed integer greater than or equal to zero, and less than or equal to maxValue.</returns>
        public int Next(int maxValue)
        {
            if (maxValue < 0)
            {
                throw new ArgumentOutOfRangeException("maxValue", @"maxValue is less than zero.");
            }

            return this.Random.Next(maxValue + 1);
        }

        /// <summary>Returns a random integer that is within a specified range.</summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The inclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <exception cref="ArgumentOutOfRangeException">minValue is greater than maxValue.</exception>
        /// <returns>A 32-bit signed integer greater than or equal to minValue and less than or equal to maxValue. If minValue equals maxValue, minValue is returned.</returns>
        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException("maxValue", @"minValue is greater than maxValue");
            }

            return this.Random.Next(minValue, maxValue + 1);
        }

        /// <summary>Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.</summary>
        /// <returns>A double-precision floating point number that is greater than or equal to 0.0, and less than 1.0.</returns>
        public double NextDouble()
        {
            return this.Random.NextDouble();
        }

        #endregion
    }
}