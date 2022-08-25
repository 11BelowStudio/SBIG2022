using System;
using UnityEngine;

namespace Scripts.Gameplay
{
    [Serializable]
    public enum EnemyEnum
    {
        TORTELVIS,
        ESIOTROT,
        KEIN,
        TESTUDO,
        INTRUDER
    }

    public static class EnemyEnumMethods
    {

        public static Color ToColour(this EnemyEnum whichOne)
        {
            return whichOne switch
            {
                EnemyEnum.TORTELVIS => Color.white,
                EnemyEnum.TESTUDO => Color.red,
                EnemyEnum.ESIOTROT => Color.green,
                EnemyEnum.KEIN => Color.blue,
                EnemyEnum.INTRUDER => Color.cyan,
                _ => Color.magenta
            };
        }
        
    }
}