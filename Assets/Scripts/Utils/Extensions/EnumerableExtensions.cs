#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Scripts.Utils.Extensions
{
    namespace EnumerableExt
    {
        
        public static class EnumerableExtensions
        {
            public static bool ContainsObjectOfType<T>(this IEnumerable<T> theList, Type findMe)
            {
                foreach (var item in theList)
                {
                    if (item == null) continue;
                    
                    if (item is Type) return true;
                }
                return false;
            }

            public static bool TryGetComponentOfTypeInList<T, TR>(this IEnumerable<T> theComponentList, out TR findMe)
                where T : Component
                where TR : Component
            {
                foreach (var item in theComponentList)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    if (item.TryGetComponent(out TR comp))
                    {
                        findMe = comp;
                        return true;
                    }
                }

                findMe = null;
                return false;
            }
            
            public static void GetComponentsOfTypeInList<T, TR>(this IEnumerable<T> theComponentList, out List<TR> found)
                where T : Component
                where TR : Component
            {
                found = new List<TR>();
                foreach (var item in theComponentList)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    if (item.TryGetComponent(out TR comp))
                    {
                        found.Add(comp);
                    }
                }
            }
            
            /// <summary>
            /// Simple 'any' wrapper thing for IEnumerables (seeing if anything in the list satisfies a condition)
            /// but doesn't require using Linq
            /// </summary>
            /// <param name="theEnumerable">The IEnumerable that we're applying doThis to</param>
            /// <param name="checkThis">Condition to check for every item in the list</param>
            /// <typeparam name="T"></typeparam>
            /// <returns>True if anything in the list satisfies this condition</returns>
            public static bool Any<T>(this IEnumerable<T> theEnumerable, Predicate<T> checkThis)
            {
                foreach (var item in theEnumerable)
                {
                    if (checkThis(item))
                    {
                        return true;
                    }
                }
                return false;
            }

        }
        
    }
}