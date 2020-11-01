using System;
using System.ComponentModel.DataAnnotations;


namespace Slot.Model.Entity
{
    /// <summary>The default value attribute.</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DefaultValueAttribute : Attribute
    {
        public DefaultValueAttribute(string sqlExpression)
        {
            this.SqlExpression = sqlExpression;
        }

        public string SqlExpression { get; private set; }
    }

    public sealed class NotNullAttribute : RequiredAttribute
    {
        public NotNullAttribute()
        {
            AllowEmptyStrings = true;
        }
    }
}