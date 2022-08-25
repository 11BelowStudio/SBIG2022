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
        BRI_AIN,
        NUMBER_NINE
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
            CameraEnum.BRI_AIN,
            CameraEnum.NUMBER_NINE
        };

        public static CameraEnum MoveLeft(this CameraEnum currentCam) => currentCam switch
        {
            CameraEnum.A => CameraEnum.NUMBER_NINE,
            CameraEnum.B => CameraEnum.A,
            CameraEnum.C => CameraEnum.B,
            CameraEnum.D => CameraEnum.C,
            CameraEnum.E => CameraEnum.D,
            CameraEnum.F => CameraEnum.E,
            CameraEnum.G => CameraEnum.F,
            CameraEnum.BRI_AIN => CameraEnum.G,
            CameraEnum.NUMBER_NINE => CameraEnum.BRI_AIN,
            _ => currentCam
        };
        
        public static CameraEnum MoveRight(this CameraEnum currentCam) => currentCam switch
        {
            CameraEnum.A => CameraEnum.B,
            CameraEnum.B => CameraEnum.C,
            CameraEnum.C => CameraEnum.D,
            CameraEnum.D => CameraEnum.E,
            CameraEnum.E => CameraEnum.F,
            CameraEnum.F => CameraEnum.G,
            CameraEnum.G => CameraEnum.BRI_AIN,
            CameraEnum.BRI_AIN => CameraEnum.NUMBER_NINE,
            CameraEnum.NUMBER_NINE => CameraEnum.A,
            _ => currentCam
        };

    }
}