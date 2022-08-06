namespace Scripts.Gameplay
{
    public interface IHaveACameraEnum
    {

        public CameraEnum CamEnum { get; }
        
    }

    public static class ExtIHaveACameraEnum
    {
        public static bool IsThisMyCam(this IHaveACameraEnum camOwner, CameraEnum theCam)
        {
            return camOwner.CamEnum == theCam;
        }
        
    }
}