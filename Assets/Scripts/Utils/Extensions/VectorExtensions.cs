#region

using UnityEngine;

#endregion

// ReSharper disable once CheckNamespace
namespace Scripts.Utils.Extensions
{

    namespace Vectors
    {
        /// <summary>
        /// Some extension methods for vectors.
        /// </summary>
        public static class VectorExtensions
        {

            /// <summary>
            /// Converts a 2D vector XY to a Vector3 of (xy.x, 0, xy.z)
            /// </summary>
            /// <param name="xy">2D vector of XY</param>
            /// <returns>Vector3 of (xy.x, 0, xy.y)</returns>
            public static Vector3 XYtoXZ(this Vector2 xy)
            {
                return new Vector3(xy.x, 0, xy.y);
            }

            /// <summary>
            /// Vector that goes from this (the source) to the destination.
            /// </summary>
            /// <param name="source"></param>
            /// <param name="destination"></param>
            /// <returns></returns>
            public static Vector3 VectorTo(this Vector3 source, Vector3 destination)
            {
                return destination - source;
            }

            /// <summary>
            /// Converts a 3D vector XZ to a Vector2 of (xz.x, xz.z).
            /// XZ.y will be ignored.
            /// </summary>
            /// <param name="xz">The 3D vector that we want to obtain the XZ components of</param>
            /// <returns>Vector2 of (xz.x, xz.z)</returns>
            public static Vector2 XZtoXY(this Vector3 xz)
            {
                return new Vector2(xz.x, xz.z);
            }

            /// <summary>
            /// multiplies vector3 componentwise by different scales
            /// </summary>
            /// <param name="vect"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z"></param>
            /// <returns></returns>
            public static Vector3 MultXYZ(this Vector3 vect, float x, float y, float z)
            {
                return new Vector3(vect.x * x, vect.y * y, vect.z * z);
            }

            /// <summary>
            /// multiplies vector3 componentwise by different scales
            /// </summary>
            /// <param name="vect"></param>
            /// <param name="xy">multiply x by xy.x and y by xy.y</param>
            /// <returns></returns>
            public static Vector3 MultXY(this Vector3 vect, Vector2 xy)
            {
                return new Vector3(vect.x * xy.x, vect.y * xy.y, vect.z);
            }

            public static Vector3 MultXYZ(this Vector3 vect, Vector3 other)
            {
                return new Vector3(vect.x * other.x, vect.y * other.y, vect.z * other.z);
            }

            public static void ScaleXYZ(this Vector3 vect, Vector3 other)
            {
                vect.x *= other.x;
                vect.y *= other.y;
                vect.z *= other.z;
            }

            public static Vector3 CopySetY(this Vector3 vect, float newY)
            {
                return new Vector3(vect.x, newY, vect.z);
            }

            /// <summary>
            /// sets y of this vector3 to 0.
            /// </summary>
            /// <param name="vect"></param>
            public static void Flatten(this Vector3 vect)
            {
                vect.y = 0;
            }


            /// <summary>
            /// scales vector3 componentwise by different scales
            /// </summary>
            /// <param name="vect"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z"></param>
            /// <returns></returns>
            public static void ScaleXYZ(this Vector3 vect, float x, float y, float z)
            {
                vect.x *= x;
                vect.y *= y;
                vect.z *= z;
            }



            /// <summary>
            /// returns the result of adding x y z componentwise to vect (does not modify vect).
            /// </summary>
            /// <param name="vect"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z"></param>
            /// <returns></returns>
            public static Vector3 AddReturnXYZ(this Vector3 vect, float x, float y, float z)
            {
                return new Vector3(vect.x + x, vect.y + y, vect.z + z);
            }


            /// <summary>
            /// Returns the result of multiplying vect.x by x and vect.y by y (as a new vector)
            /// </summary>
            /// <param name="vect"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static Vector2 MultXY(this Vector2 vect, float x, float y)
            {
                return new Vector2(vect.x * x, vect.y * y);
            }



            /// <summary>
            /// scales Vector2 componentwise by Xy. MODIFIES THIS VECTOR!
            /// </summary>
            /// <param name="vect"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static Vector2 ScaleXY(this Vector2 vect, float x, float y)
            {
                vect.x *= x;
                vect.y *= y;
                return vect;
            }

            /// <summary>
            /// Returns a new Vector2 of (sign(x), sign(y))
            /// </summary>
            /// <param name="vect"></param>
            /// <returns></returns>
            public static Vector2 Sign(this Vector2 vect)
            {
                return new Vector2(UnityEngine.Mathf.Sign(vect.x), UnityEngine.Mathf.Sign(vect.y));
            }

            public static Vector2 PosNegZero(this Vector2 vect)
            {
                return new Vector2(
                    UnityEngine.Mathf.Approximately(vect.x, 0f) ? 0f : UnityEngine.Mathf.Sign(vect.x),
                    UnityEngine.Mathf.Approximately(vect.y, 0f) ? 0f : UnityEngine.Mathf.Sign(vect.y)
                );
            }

            public static Vector2 Abs(this Vector2 vect)
            {
                return new Vector2(UnityEngine.Mathf.Abs(vect.x), UnityEngine.Mathf.Abs(vect.y));
            }
        }
    }
}