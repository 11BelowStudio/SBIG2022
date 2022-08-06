#region

using System;

#endregion

namespace Scripts.Utils.Extensions
{
    namespace IComparable
    {
        public static class IComparableExtensions
        {

            public static bool IsBetween<T>(this IComparable<T> me, T lower, T upper)
            {
                return (me.CompareTo(lower) > 0) && (me.CompareTo(upper) < 0);
            }

            public static bool IsBetweenOrEqual<T>(this IComparable<T> me, T lower, T upper)
            {
                return (me.CompareTo(lower) >= 0) && (me.CompareTo(upper) <= 0);
            }

        }
    }
}