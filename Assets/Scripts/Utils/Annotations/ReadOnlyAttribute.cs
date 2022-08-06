#region

using UnityEngine;

#endregion

namespace Scripts.Utils.Annotations
{
    /// <summary>
    /// used to mark fields as read-only (not modifiable in inspector)
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute { }

}