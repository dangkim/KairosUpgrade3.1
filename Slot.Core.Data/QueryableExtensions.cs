using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Slot.Core.Data.Attributes.SqlBuilder;
using Slot.Core.Data.Attributes;

namespace Slot.Core.Data
{
    public static class QueryableExtensions
    {
        private readonly static Dictionary<Type, DbType> typeMap;

        static QueryableExtensions()
        {
            typeMap = new Dictionary<Type, DbType>
            {
                [typeof(byte)] = DbType.Byte,
                [typeof(sbyte)] = DbType.SByte,
                [typeof(short)] = DbType.Int16,
                [typeof(ushort)] = DbType.UInt16,
                [typeof(int)] = DbType.Int32,
                [typeof(uint)] = DbType.UInt32,
                [typeof(long)] = DbType.Int64,
                [typeof(ulong)] = DbType.UInt64,
                [typeof(float)] = DbType.Single,
                [typeof(double)] = DbType.Double,
                [typeof(decimal)] = DbType.Decimal,
                [typeof(bool)] = DbType.Boolean,
                [typeof(string)] = DbType.String,
                [typeof(char)] = DbType.StringFixedLength,
                [typeof(Guid)] = DbType.Guid,
                [typeof(DateTime)] = DbType.DateTime,
                [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
                [typeof(TimeSpan)] = DbType.Time,
                [typeof(byte[])] = DbType.Binary,
                [typeof(byte?)] = DbType.Byte,
                [typeof(sbyte?)] = DbType.SByte,
                [typeof(short?)] = DbType.Int16,
                [typeof(ushort?)] = DbType.UInt16,
                [typeof(int?)] = DbType.Int32,
                [typeof(uint?)] = DbType.UInt32,
                [typeof(long?)] = DbType.Int64,
                [typeof(ulong?)] = DbType.UInt64,
                [typeof(float?)] = DbType.Single,
                [typeof(double?)] = DbType.Double,
                [typeof(decimal?)] = DbType.Decimal,
                [typeof(bool?)] = DbType.Boolean,
                [typeof(char?)] = DbType.StringFixedLength,
                [typeof(Guid?)] = DbType.Guid,
                [typeof(DateTime?)] = DbType.DateTime,
                [typeof(DateTimeOffset?)] = DbType.DateTimeOffset,
                [typeof(TimeSpan?)] = DbType.Time,
                [typeof(object)] = DbType.Object
            };
        }

        private static SqlParameter[] BuildParameters<T>(this T parameter)
        {
            var t = parameter.GetType();
            return t.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Select(prop => prop.GetSqlParameter(parameter))
                    .Where(x => x != null)
                    .ToArray();
        }

        private static SqlParameter GetSqlParameter<T>(this PropertyInfo property, T parameter)
        {
            SqlParameter sqlParameter = null;

            if (typeMap.TryGetValue(property.PropertyType, out DbType dbType))
            {
                var excludedAttribute = property.GetCustomAttribute<ExcludedAttribute>();

                if (excludedAttribute == null)
                {
                    var value = property.GetValue(parameter);
                    var sqlBuilderAttribute = property.GetCustomAttribute<SqlBuilderAttribute>();
                    var optionalAttribute = property.GetCustomAttribute<OptionalAttribute>();

                    var propertyName = sqlBuilderAttribute?.ParameterName ?? property.Name;

                    if (optionalAttribute != null && Equals(value, optionalAttribute.DefaultValue))
                    {
                        sqlParameter = new SqlParameter($"@{propertyName}", dbType)
                        {
                            Value = DBNull.Value
                        };
                    }
                    else
                    {
                        sqlParameter = new SqlParameter($"@{propertyName}", dbType)
                        {
                            Value = value ?? DBNull.Value
                        };
                    }
                }
            }

            return sqlParameter;
        }

        public static IQueryable<TEntity> Query<TEntity>(this DbQuery<TEntity> source, [NotParameterized] RawSqlString sql, object parameters) where TEntity : class
        {
            var param = parameters.BuildParameters();
            return source.FromSql(sql, param);
        }
    }
}
