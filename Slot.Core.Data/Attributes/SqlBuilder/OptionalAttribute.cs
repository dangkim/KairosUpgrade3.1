using System;

namespace Slot.Core.Data.Attributes.SqlBuilder
{
    /// <summary>
    /// When used this dictates that the property would be a <see cref="DBNull"/> value when the default value parameter is met.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class OptionalAttribute : SqlBuilderAttribute
    {
        public object DefaultValue { get; }

        /// <summary>
        /// Instantiate a new instance of <see cref="OptionalAttribute"/> class.
        /// </summary>
        /// <param name="parameterName">Parameter name used when building the sql statement.</param>
        public OptionalAttribute(string parameterName) : base(parameterName)
        {
        }

        /// <summary>
        /// Instantiate a new instance of <see cref="OptionalAttribute"/> class.
        /// </summary>
        /// <param name="defaultValue">If the property value is equivalent to the property value, the property value will be transformed to a <see cref="DBNull"/> value.</param>
        /// <param name="parameterName">Parameter name used when building the sql statement.</param>
        public OptionalAttribute(object defaultValue, string parameterName = null) : this(parameterName)
        {
            DefaultValue = defaultValue;
        }
    }
}
