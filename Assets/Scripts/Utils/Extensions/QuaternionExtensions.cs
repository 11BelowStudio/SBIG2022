#region

using UnityEngine;

#endregion

// ReSharper disable once CheckNamespace
namespace Scripts.Utils.Extensions
{

    namespace QuaternionExt
    {

        public static class QuaternionExtensions
        {
            public static Vector4 ToVector4(this Quaternion quat)
            {
                return new Vector4(quat.x, quat.y, quat.z, quat.w);
            }

            public static float[] ToArray(this Quaternion quat)
            {
                return new[] { quat.x, quat.y, quat.z, quat.w };
            }

            public static void ToArrayNonAlloc(this Quaternion quat, ref float[] arr)
            {
                arr[0] = quat.x;
                arr[1] = quat.y;
                arr[2] = quat.z;
                arr[3] = quat.w;
            }
        }

    }
}