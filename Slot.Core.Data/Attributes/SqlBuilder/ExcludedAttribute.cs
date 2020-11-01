using Slot.Core.Data.Attributes.SqlBuilder;
using System;

namespace Slot.Core.Data.Attributes.SqlBuilder
{
    /// <summary>
    /// When used this dictates that the property should not be included in parameter inclusion when querying through the <see cref="QueryableExtensions"/> methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ExcludedAttribute : SqlBuilderAttribute
    {
        /// <summary>
        /// Instantiate a new instance of <see cref="ExcludedAttribute"/> class.
        /// </summary>
        public ExcludedAttribute() : base(null)
        {
        }
    }
}
