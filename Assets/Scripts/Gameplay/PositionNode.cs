using System;
using UnityEngine;

namespace Scripts.Gameplay
{
    public class PositionNode: MonoBehaviour, IHaveAPosition
    {
        public Vector3 Position => transform.position;

        internal void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(Position, 1f);
        }
    }
}