#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Scripts.Utils.Types
{
    public class SingletonExtender: MonoBehaviour
    {
        [SerializeField]
        private Component makeThisASingleton;

        private static IDictionary<Type, Component> _singletons = new Dictionary<Type, Component>();

        private void OnValidate()
        {
            if (makeThisASingleton.gameObject != gameObject)
            {
                Debug.LogError("Please attach the SingletonExtender to the same GameObject as the thing you're trying to make into a singleton!");
                makeThisASingleton = null;
            }
        }

        private void Awake()
        {
            
            if (makeThisASingleton == null){ Destroy(this); return; }
            
            if (_singletons.TryGetValue(makeThisASingleton.GetType(), out var existing))
            {
                if (existing != makeThisASingleton){ Destroy(gameObject); }
                return;
            }

            _singletons[makeThisASingleton.GetType()] = makeThisASingleton;

            foreach (var kv in _singletons)
            {
                Debug.Log(kv);
            }
            
        }

        private void OnDestroy()
        {
            if (_singletons.TryGetValue(makeThisASingleton.GetType(), out var existing))
            {
                if (existing == makeThisASingleton) { _singletons[makeThisASingleton.GetType()] = null; }
            }
            
        }
    }
}