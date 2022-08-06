#region

using UnityEngine;

#endregion

// ReSharper disable LoopCanBeConvertedToQuery

// ReSharper disable once CheckNamespace
namespace Scripts.Utils.Extensions
{
    namespace GameObjectExt
    {
        
        public static class GameObjectExtensions
        {
            /// <summary>
            /// obtains the component in children from the gameobject with the specified tag.
            /// </summary>
            /// <param name="g"></param>
            /// <param name="tag"></param>
            /// <typeparam name="T"></typeparam>
            /// <returns>specified component from a child that has the given tag</returns>
            public static T GetComponentInChildrenWithTag<T>(this GameObject g, string tag) where T : class
            {
                foreach (Transform t in g.transform)
                {
                    if (t.CompareTag(tag)) return t.GetComponent<T>();
                }

                return null;
            }

            public static bool TryGetComponentInHierarchy<T>(this GameObject g, out T comp,
                bool includeInactives = false) where T : class
            {
                if (g.TryGetComponent(out T comp1))
                {
                    comp = comp1;
                    return true;
                }

                comp = g.GetComponentInChildren<T>(includeInactives);
                if (comp != null)
                {
                    return true;
                }

                comp = g.GetComponentInParent<T>(includeInactives);
                return comp != null;
            }

        }

    }
}