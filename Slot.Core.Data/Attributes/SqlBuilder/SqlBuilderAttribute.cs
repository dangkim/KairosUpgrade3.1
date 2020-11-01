using System;

namespace Slot.Core.Data.Attributes.SqlBuilder
{
    /// <summary>
    /// Base class for sql builder attributes. Used for mapping class properties with raw queries.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SqlBuilderAttribute : Attribute
    {
        public string ParameterName { get; protected set; }

        public SqlBuilderAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }
    }
}
