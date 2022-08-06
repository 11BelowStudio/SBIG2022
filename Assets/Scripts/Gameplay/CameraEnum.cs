using System;
using System.Collections.Generic;

namespace Scripts.Gameplay
{
    [Serializable]
    public enum CameraEnum
    {
        OFFICE,
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        BRI_AIN
    }

    public static class CameraEnumExt
    {

        public static readonly IReadOnlyList<CameraEnum> camRooms = new[]{
            CameraEnum.A,
            CameraEnum.B,
            CameraEnum.C,
            CameraEnum.D,
            CameraEnum.E,
            CameraEnum.F,
            CameraEnum.G,
            CameraEnum.BRI_AIN
        };

    }
}