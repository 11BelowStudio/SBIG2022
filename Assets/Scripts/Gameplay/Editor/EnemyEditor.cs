using UnityEditor;
using UnityEngine;

namespace Scripts.Gameplay.Editor
{
    [CustomEditor(typeof(Enemy))]
    public class EnemyEditor: UnityEditor.Editor
    {
        private Enemy _target;
        private void OnEnable()
        {
            _target = (Enemy)target;
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Forcibly validate stuff"))
            {
                _target.OnValidate();
            }
        }
    }
}