using System;
using UnityEditor;
using UnityEngine;

namespace Scripts.Gameplay.Editor
{
    
    [CustomEditor(typeof(OfficeTortoiseHUD))]
    public class OfficeTortoiseHUDEditor: UnityEditor.Editor
    {
        private OfficeTortoiseHUD _target;

        private void OnEnable()
        {
            _target = (OfficeTortoiseHUD)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Oh god oh fuck Percival started again"))
            {
                _target.PercivalHasStartedSingingAgain();
            }
        }
    }
}