

using System;

namespace Scripts.Utils.Extensions
{
    namespace ArrayExt
    {
        /// <summary>
        /// A few utility/extension methods for arrays (y'know, the <code>Foo[]</code> sorts of arrays, those).
        /// Most of these methods involve the removal of nulls from these arrays.
        /// </summary>
        public static class ArrayExtensions
        {

            /// <summary>
            /// Create a copy of the list, but with the null values removed.
            /// </summary>
            /// <param name="theList">List to remove nulls from</param>
            /// <typeparam name="T"></typeparam>
            /// <returns>A copy of this list, with the null values removed from it (and trimmed down to only contain the non-nulls)</returns>
            public static T[] NonNulls<T>(this T[] theList)
            {

                int nonNullCursor = 0;
                T[] nonNulls = new T[theList.Length];

                for (int i = 0; i < theList.Length; i++)
                {
                    if (theList[i] == null)
                    {
                        continue;
                    }
                    nonNulls[nonNullCursor] = theList[i];
                    nonNullCursor += 1;
                }

                System.Array.Resize(ref nonNulls, nonNullCursor);
                return nonNulls;
            }

            /// <summary>
            /// Remove the nulls from this list. Modifies this list, AND WILL TRIM IT!
            /// </summary>
            /// <param name="theList"></param>
            /// <typeparam name="T"></typeparam>
            /// <returns>This list, without the nulls, trimmed down to size.</returns>
            public static T[] RemoveNulls<T>(this T[] theList)
            {
                int nonNullCursor = 0;
                for (int i = 0; i < theList.Length; i++)
                {
                    if (theList[i] == null)
                    {
                        continue;
                    }

                    theList[nonNullCursor] = theList[i];
                    nonNullCursor += 1;
                }
                System.Array.Resize(ref theList, nonNullCursor);
                return theList;
            }

            /// <summary>
            /// This will move the null values to the end of the array.
            /// (if the array is of a value type, it'll move 'default' to the end,
            /// although I have no idea why a value type array would have any nulls in it in the first place,
            /// but, y'know, full API provided for future usage I guess).
            /// Returns the count of non-nulls in the list.
            /// </summary>
            /// <param name="theList"></param>
            /// <typeparam name="T"></typeparam>
            /// <returns>Count of non-null items in this list.</returns>
            public static int MoveNullsToEnd<T>(this T[] theList)
            {
                int nonNullCursor = 0;
                for (int i = 0; i < theList.Length; i++)
                {
                    if (theList[i] == null)
                    {
                        continue;
                    }

                    if (nonNullCursor != i)
                    {
                        theList[nonNullCursor] = theList[i];
                        theList[i] = default;
                    }
                    nonNullCursor += 1;
                }
                return nonNullCursor;
            }

            /// <summary>
            /// Uses <see cref="RemoveNulls{T}"/> to remove nulls and trim the list, returns the new length of this list.
            /// </summary>
            /// <param name="theList"></param>
            /// <typeparam name="T"></typeparam>
            /// <returns>New length of this list.</returns>
            public static int RemoveNullsCount<T>(this T[] theList)
            {
                return theList.RemoveNulls().Length;
            }

            /// <summary>
            /// Removes the nulls from the list of params given to this method.
            /// </summary>
            /// <param name="theseMayBeNull">
            /// The things that may or may not be null, which we want to remove the nulls from.
            /// </param>
            /// <typeparam name="T"></typeparam>
            /// <returns>A new T[] array, containing the non-null params.</returns>
            /// <remarks>IF YOU PASS AN ARRAY DIRECTLY INSTEAD OF PARAMS, THAT ARRAY WILL BE MODIFIED.
            /// USE <see cref="NonNulls{T}"/> INSTEAD IF YOU WANT TO KEEP THE ORIGINAL ARRAY.</remarks>
            public static T[] GetNonNulls<T>(params T[] theseMayBeNull)
            {
                return theseMayBeNull.RemoveNulls();
            }

            /// <summary>
            /// Removes the nulls from the list of params given to this method.
            /// </summary>
            /// <param name="theseMayBeNull"></param>
            /// <returns>A new array of the non-null objects passed as parameters to this method.</returns>
            /// <remarks>IF YOU PASS AN ARRAY DIRECTLY INSTEAD OF PARAMS, THAT ARRAY WILL BE MODIFIED.
            /// USE <see cref="NonNulls{T}"/> INSTEAD IF YOU WANT TO KEEP THE ORIGINAL ARRAY.</remarks>
            public static object[] GetNonNulls(params object[] theseMayBeNull)
            {
                return theseMayBeNull.RemoveNulls();
            }

            /// <summary>
            /// Simple 'any' wrapper thing for arrays (seeing if anything in the list satisfies a condition)
            /// </summary>
            /// <param name="theList">The list that we're applying doThis to</param>
            /// <param name="checkThis">Condition to check for every item in the list</param>
            /// <typeparam name="T"></typeparam>
            /// <returns>True if anything in the list satisfies this condition</returns>
            public static bool Any<T>(this T[] theList, Predicate<T> checkThis)
            {
                for (int i = theList.Length - 1; i >= 0; i--)
                {
                    if (checkThis.Invoke(theList[i]))
                    {
                        return true;
                    }
                }
                return false;
            }


            /// <summary>
            /// Simple foreach wrapper thing for arrays
            /// </summary>
            /// <param name="theList">The list that we're applying doThis to</param>
            /// <param name="doThis">Action to perform for each item in theList</param>
            /// <typeparam name="T"></typeparam>
            /// <returns>theList, after performing doThis on every item in it.</returns>
            public static T[] ForEach<T>(this T[] theList, Action<T> doThis)
            {
                for (int i = theList.Length - 1; i >= 0; i--)
                {
                    doThis?.Invoke(theList[i]);
                }
                return theList;
            }

            /// <summary>
            /// Like <see cref="ForEach{T}(T[],System.Action{T})"/>, except for params[] instead.
            /// </summary>
            /// <param name="doThis"></param>
            /// <param name="applyToThese"></param>
            /// <typeparam name="T"></typeparam>
            /// <returns>The params[] after applying doThis to everything in it.</returns>
            public static T[] ForEach<T>(Action<T> doThis, params T[] applyToThese)
            {
                return applyToThese.ForEach(doThis);
            }
        }
    }
    
}