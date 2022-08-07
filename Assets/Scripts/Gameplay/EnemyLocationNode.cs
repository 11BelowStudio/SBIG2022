using System;
using Scripts.Utils.Extensions.ListExt;
using UnityEngine;

namespace Scripts.Gameplay
{
    public class EnemyLocationNode: MonoBehaviour, IHaveACameraEnum, IHaveAPosition
    {

        public EnemyEnum WhoseNodeIsThis => _whoIs;

        public Vector3 Position => transform.position;

        public bool ShouldIFaceCamera = false;

        [SerializeField]
        private EnemyEnum _whoIs;

        public CameraEnum CamEnum => _whereIs;

        [SerializeField] private CameraEnum _whereIs;

        public void OnDrawGizmos()
        {
            var oldColor = Gizmos.color;

            Gizmos.color = WhoseNodeIsThis.ToColour();
            
            Gizmos.DrawWireSphere(transform.position, 1f);

            Gizmos.color = oldColor;
        }

        private void OnValidate()
        {
            if (ShouldIFaceCamera)
            {
                if (FindObjectsOfType<RoomCamera>().TryFindInList(rc => rc.IsThisMyCam(_whereIs), out RoomCamera lookAtThis))
                {
                    transform.LookAt(lookAtThis.transform.position);
                }
                
            }
        }
    }
}