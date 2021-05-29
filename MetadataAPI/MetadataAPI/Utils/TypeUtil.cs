using System;
using System.Collections.Generic;
using System.Text;

namespace MetadataAPI.Utils
{
    internal static class TypeUtil
    {
        /// <summary>
        /// Determines whether a specifed value can be assigned to a specified type.
        /// </summary>
        internal static bool IsAssignableTo<T>(object? value)
        {
            if (typeof(T).IsValueType)
            {
                return value is T;
            }
            else
            {
                return value is null || value is T;
            }
        }

    }
}
