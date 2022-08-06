#region

using UnityEditor;
using UnityEngine;

#endregion

namespace Scripts.Utils.Annotations.Editor
{
    /// <summary>
    /// shows fields marked as [ReadOnly], makes them unmodifiable in inspector
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer {
     
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // Saving previous GUI enabled value
            var previousGUIState = GUI.enabled;
            // Disabling edit for property
            GUI.enabled = false;
            // Drawing Property

            EditorGUI.BeginProperty(position, label, property);
            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    EditorGUI.Toggle(position, label, property.boolValue);
                    break;
                case SerializedPropertyType.Color:
                    EditorGUI.ColorField(position, label, property.colorValue);
                    break;
                case SerializedPropertyType.Integer:
                    EditorGUI.IntField(position, label, property.intValue);
                    break;
                case SerializedPropertyType.Float:
                    EditorGUI.FloatField(position, label, property.floatValue);
                    break;
                case SerializedPropertyType.String:
                    EditorGUI.TextField(position, label, property.stringValue);
                    break;
                case SerializedPropertyType.ObjectReference:
                    EditorGUI.ObjectField(position, property, label);
                    break;
                case SerializedPropertyType.Vector2:
                    EditorGUI.Vector2Field(position, label, property.vector2Value);
                    break;
                case SerializedPropertyType.Vector3:
                    EditorGUI.Vector3Field(position, label, property.vector3Value);
                    break;
                case SerializedPropertyType.Vector4:
                    EditorGUI.Vector4Field(position, label, property.vector4Value);
                    break;
                case SerializedPropertyType.Quaternion:
                    EditorGUI.Vector4Field(position, label, ToVector4(property.quaternionValue));
                    break;
                case SerializedPropertyType.Rect:
                    EditorGUI.RectField(position, label, property.rectValue);
                    break;
                case SerializedPropertyType.Bounds:
                    EditorGUI.BoundsField(position, label, property.boundsValue);
                    break;
                case SerializedPropertyType.Vector2Int:
                    EditorGUI.Vector2IntField(position, label,property.vector2IntValue);
                    break;
                case SerializedPropertyType.Vector3Int:
                    EditorGUI.Vector3IntField(position, label,property.vector3IntValue);
                    break;
                case SerializedPropertyType.RectInt:
                    EditorGUI.RectIntField(position, label,property.rectIntValue);
                    break;
                case SerializedPropertyType.BoundsInt:
                    EditorGUI.BoundsIntField(position, label,property.boundsIntValue);
                    break;
                default:
                    EditorGUI.PropertyField(position, property, label, true);
                    break;
            }
            EditorGUI.EndProperty();
            // Setting old GUI enabled value
            GUI.enabled = previousGUIState;
        }

        

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * property.CountInProperty();
        }
        
        private static Vector4 ToVector4(Quaternion quat)
        {
            return new Vector4(quat.x, quat.y, quat.z, quat.w);
        }
    }

}