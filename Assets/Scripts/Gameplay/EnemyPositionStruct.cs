using System;

namespace Scripts.Gameplay
{
    [Serializable]
    public struct EnemyPositionStruct: IHaveACameraEnum
    {
        public EnemyLocationNode theNode;
        
        public int nextPos;
        public bool isAttackPos;
        
        public CameraEnum CamEnum => theNode.CamEnum;
    }
}