#region

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

// ReSharper disable once CheckNamespace
namespace Scripts.Utils.Extensions
{

    namespace ListExt
    {
        public static class ListExtensions
        {
            public static void InsertIfNotPresent<T>(this List<T> theList, T addMe)
            {
                if (theList.Contains(addMe)) return;
                theList.Add(addMe);
            }

            public static IList<T> AddAllIfNotPresent<T>(this List<T> theList, IEnumerable<T> addThese)
            {
                var uniques = new HashSet<T>(theList);
                uniques.UnionWith(addThese);
                theList.Clear();
                theList.InsertRange(0, uniques);
                return theList;
            }

            /// <summary>
            /// Swaps the items at index A and index B of the list. Also returns the item that was at index A.
            /// </summary>
            /// <param name="theList"></param>
            /// <param name="a">index of the first iem (item that was at this index will be returned)</param>
            /// <param name="b">index to swap that item with.</param>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public static T SwapTheseTwoAndGet<T>(this IList<T> theList, int a, int b = 0)
            {
                var getMe = theList[a];
                theList[a] = theList[b];
                theList[b] = getMe;
                return getMe;

            }

            public static T GetFirstMatchOrFirstElement<T>(this IList<T> theList, Predicate<T> condition)
            {
                foreach (var item in theList)
                {
                    if (condition(item))
                    {
                        return item;
                    }
                }

                return theList[0];

            }
            
            [CanBeNull]
            public static T FindButDontThrowIfNull<T>(this IList<T> theList, Predicate<T> condition) where T: class
            {
                foreach (var item in theList)
                {
                    if (condition(item))
                    {
                        return item;
                    }
                }
                return null;
            }

        }
    }
}