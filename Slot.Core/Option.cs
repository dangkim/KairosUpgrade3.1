namespace Slot.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an optional value.
    /// </summary>
    /// <typeparam name="T">The type of the value to be wrapped.</typeparam>
    public struct Option<T> : IEquatable<Option<T>>, IComparable<Option<T>>
    {
        private readonly bool hasValue;
        private readonly T value;

        /// <summary>
        /// Checks if a value is present.
        /// </summary>
        public bool HasValue => hasValue;

        public bool None => hasValue == false;

        internal T Value => value;

        internal Option(T value, bool hasValue)
        {
            this.value = value;
            this.hasValue = hasValue;
        }

        /// <summary>
        /// Determines whether two optionals are equal.
        /// </summary>
        /// <param name="other">The optional to compare with the current one.</param>
        /// <returns>A boolean indicating whether or not the optionals are equal.</returns>
        public bool Equals(Option<T> other)
        {
            if (!hasValue && !other.hasValue)
            {
                return true;
            }
            else if (hasValue && other.hasValue)
            {
                return EqualityComparer<T>.Default.Equals(value, other.value);
            }

            return false;
        }

        /// <summary>
        /// Determines whether two optionals are equal.
        /// </summary>
        /// <param name="obj">The optional to compare with the current one.</param>
        /// <returns>A boolean indicating whether or not the optionals are equal.</returns>
        public override bool Equals(object obj) => obj is Option<T> ? Equals((Option<T>)obj) : false;

        /// <summary>
        /// Generates a hash code for the current optional.
        /// </summary>
        /// <returns>A hash code for the current optional.</returns>
        public override int GetHashCode()
        {
            if (hasValue)
            {
                if (value == null)
                {
                    return 1;
                }

                return value.GetHashCode();
            }

            return 0;
        }

        /// <summary>
        /// Compares the relative order of two optionals. An empty optional is ordered before a
        /// non-empty one.
        /// </summary>
        /// <param name="other">The optional to compare with the current one.</param>
        /// <returns>An integer indicating the relative order of the optionals being compared.</returns>
        public int CompareTo(Option<T> other)
        {
            if (hasValue && !other.hasValue) return 1;
            if (!hasValue && other.hasValue) return -1;
            return Comparer<T>.Default.Compare(value, other.value);
        }

        /// <summary>
        /// Determines whether two optionals are equal.
        /// </summary>
        /// <param name="left">The first optional to compare.</param>
        /// <param name="right">The second optional to compare.</param>
        /// <returns>A boolean indicating whether or not the optionals are equal.</returns>
        public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);

        /// <summary>
        /// Determines whether two optionals are unequal.
        /// </summary>
        /// <param name="left">The first optional to compare.</param>
        /// <param name="right">The second optional to compare.</param>
        /// <returns>A boolean indicating whether or not the optionals are unequal.</returns>
        public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);
    }

    /// <summary>
    /// Provides a set of functions for creating optional values.
    /// </summary>
    public static class Option
    {
        /// <summary>
        /// Wraps an existing value in an Option&lt;T&gt; instance.
        /// </summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <returns>An optional containing the specified value.</returns>
        public static Option<T> Some<T>(T value) => new Option<T>(value, true);

        /// <summary>
        /// Creates an empty Option&lt;T&gt; instance.
        /// </summary>
        /// <returns>An empty optional.</returns>
        public static Option<T> None<T>() => new Option<T>(default(T), false);

        /// <summary>
        /// Converts an optional to a Nullable&lt;T&gt;.
        /// </summary>
        /// <param name="option">The specified optional.</param>
        /// <returns>The Nullable&lt;T&gt; instance.</returns>
        public static T? ToNullable<T>(this Option<T> option) where T : struct
        {
            if (option.HasValue)
            {
                return option.Value;
            }

            return default(T?);
        }

        /// <summary>
        /// Returns the existing value if present, otherwise default(T).
        /// </summary>
        /// <param name="option">The specified optional.</param>
        /// <returns>The existing value or a default value.</returns>
        public static T ValueOrDefault<T>(this Option<T> option)
        {
            if (option.HasValue)
            {
                return option.Value;
            }

            return default(T);
        }
    }
}