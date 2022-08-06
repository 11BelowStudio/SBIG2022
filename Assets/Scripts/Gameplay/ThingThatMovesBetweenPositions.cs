using System;
using UnityEngine;

namespace Scripts.Gameplay
{
    public class ThingThatMovesBetweenPositions: MonoBehaviour
    {
        [SerializeField] private MoveBetweenPositions _thisMovesMe;
        
        private void OnValidate()
        {
            var theMovers = FindObjectsOfType<MoveBetweenPositions>();
            foreach (var mov in theMovers)
            {
                if (mov.CheckIfIAmWhatMovesThis(this))
                {
                    _thisMovesMe = mov;
                    break;
                }
            }
        }


        private void OnDrawGizmosSelected()
        {
            if (_thisMovesMe != null)
            {
                _thisMovesMe.OnDrawGizmosSelected();
            }
        }
    }
}