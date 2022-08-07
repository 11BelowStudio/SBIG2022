
using UnityEditor;
using UnityEngine;

namespace Scripts.Gameplay.Editor
{
    [CustomEditor(typeof(TortelvisAppearanceSwapper))]
    public class TortelvisAppearanceSwapperEditor: UnityEditor.Editor
    {
        private TortelvisAppearanceSwapper _tortelvis;

        private void OnEnable()
        {
            _tortelvis = (TortelvisAppearanceSwapper)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Insert Dread Zeppelin Reference Here"))
            {
                _tortelvis.SwapVisibleTortelvis();
            }
            
        }
    }
}