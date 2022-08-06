#region

using UnityEngine;

#endregion

namespace Scripts.Utils.Types
{
    /// <summary>
    /// A base class for singleton components
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {

        /// <summary>
        /// the singleton
        /// </summary>
        private static T _instance;

        /// <summary>
        /// Usable by subclasses to check if they're meant to be the instance.
        /// Returns true if _instance is currently null or equal to isThisTheInstance
        /// (setting _instance to isThisTheInstance)
        /// Returns false otherwise.
        /// </summary>
        /// <returns></returns>
        protected bool _AttemptToRegisterInstance
        {
            get {
                if (_instance == null || _instance == (T)this)
                {
                    _instance = (T)this;
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Use this to check if a thing actually is the instance.
        /// Returns true if it is the instance, false otherwise.
        /// </summary>
        /// <returns></returns>
        protected bool _AmITheInstance => (_instance == (T)this);

        /// <summary>
        /// obtains the singleton (creating it if needed)
        /// </summary>
        public static T Instance => GetInstanceInternal(true);
        
        
        

        /// <summary>
        /// does the internal 'finding the instance and creating it if needed' stuff
        /// </summary>
        /// <param name="createNewInstanceIfNeeded">if true, created a new instance if _instance is null.</param>
        /// <returns></returns>
        private static T GetInstanceInternal(bool createNewInstanceIfNeeded)
        {
            if (_instance == null)
            {
                var found = FindObjectsOfType(typeof(T), true);
                if (found.Length > 0) _instance = (T)found[0];
                if (found.Length > 1)
                {
                    Debug.LogError($"you have multiple instances of {typeof(T).Name} in the scene >:(");
                }

                if (_instance == null && createNewInstanceIfNeeded)
                {
                    Debug.Log($"There is no {typeof(T).Name}, so I'm making one myself.");
                    GameObject obj = new GameObject
                    {
                        name = $"_{typeof(T).Name}1 singleton"
                    };
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }

        public static bool TryGetInstance(out T theInstance)
        {
            theInstance = GetInstanceInternal(false);
            return (theInstance != null);
        }

        protected virtual void OnDestroy()
        {
            if (_AmITheInstance) { _instance = null; }
        }
        
        
        
    }
}