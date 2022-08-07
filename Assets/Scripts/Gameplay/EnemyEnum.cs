using System;
using UnityEngine;

namespace Scripts.Gameplay
{
    [Serializable]
    public enum EnemyEnum
    {
        TORTELVIS,
        ESIO_TROT,
        KEIN,
        TESTUDO,
        IDENTIKIT
    }

    public static class EnemyEnumMethods
    {

        public static Color ToColour(this EnemyEnum whichOne)
        {
            return whichOne switch
            {
                EnemyEnum.TORTELVIS => Color.white,
                EnemyEnum.TESTUDO => Color.red,
                EnemyEnum.ESIO_TROT => Color.green,
                EnemyEnum.KEIN => Color.blue,
                EnemyEnum.IDENTIKIT => Color.cyan,
                _ => Color.magenta
            };
        }
        
    }
}