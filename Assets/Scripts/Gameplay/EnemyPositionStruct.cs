using System;

namespace Scripts.Gameplay
{
    [Serializable]
    public struct EnemyPositionStruct: IHaveACameraEnum
    {
        public EnemyLocationNode theNode;
        
        public int nextPos;
        public bool isAttackPos;
        
        /// <summary>
        /// Set this to true for positions that are the positions just before the attack position.
        /// </summary>
        public bool nextPosIsAttackPos;


        public bool IsAttackPosOrNextPosIsAttackPos => isAttackPos || nextPosIsAttackPos;
        
        public CameraEnum CamEnum => theNode.CamEnum;
    }
}