

namespace Scripts.Utils.Extensions
{

    namespace Mathf
    {
        public static class MathfExtensions
        {

            /// <summary>
            /// ReLU activation function. Returns max(f, 0)
            /// </summary>
            /// <param name="f">float to apply ReLU to</param>
            /// <returns>max(0, f)</returns>
            // ReSharper disable once InconsistentNaming
            public static float ReLU(in float f)
            {
                return UnityEngine.Mathf.Max(f, 0.0f);
            }

            public static float SafeDiv(this float numerator, float denominator)
            {
                return (denominator.CompareTo(0d) == 0) ? 0 : numerator / denominator;
            }

            /// <summary>
            /// ReLU activation function. Returns max(i, 0)
            /// </summary>
            /// <param name="f">float to apply ReLU to</param>
            /// <returns>max(f, 0)</returns>
            // ReSharper disable once InconsistentNaming
            public static float ReLU(this float f)
            {
                return UnityEngine.Mathf.Max(f, 0.0f);
            }

            /// <summary>
            /// ReLU activation function. Returns max(i, 0)
            /// </summary>
            /// <param name="i">int to apply ReLU to</param>
            /// <returns>max(i, 0)</returns>
            // ReSharper disable once InconsistentNaming
            public static int ReLU(this int i)
            {
                return UnityEngine.Mathf.Max(i, 0);
            }

            /// <summary>
            /// ReLU activation function. Returns max(d, 0)
            /// </summary>
            /// <param name="d">double to apply ReLU to</param>
            /// <returns>max(d, 0)</returns>
            // ReSharper disable once InconsistentNaming
            public static double ReLU(in double d)
            {
                return d > 0d ? d : 0d;
            }

            /// <summary>
            /// returns 0 if f ~= 0, otherwise returns sign(f)
            /// </summary>
            /// <param name="f"></param>
            /// <returns></returns>
            public static int PosNegZero(this float f)
            {
                return UnityEngine.Mathf.Approximately(f, 0) ? 0 : (int)UnityEngine.Mathf.Sign(f);
            }
            

        }

    }
}