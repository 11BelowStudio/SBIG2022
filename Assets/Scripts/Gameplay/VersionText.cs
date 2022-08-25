using UnityEngine;
using TMPro;

namespace Scripts.Gameplay
{
    /// <summary>
    /// Class responsible for showing the current game version to players.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    public class VersionText: MonoBehaviour
    {
        /// <summary>
        /// TextMeshPro used for actually showing the version number text
        /// </summary>
        [SerializeField] private TextMeshProUGUI theText;
        /// <summary>
        /// Optional prefix text to put in front of the version number
        /// </summary>
        [SerializeField] private string prefix = "";

        [SerializeField] private string debugSuffix = " (debug)";

        void Awake()
        {
            theText.text = $"{prefix}{Application.version}{(Debug.isDebugBuild ? debugSuffix : "")}";
        }

        private void OnValidate()
        {
            if (theText != null)
            {
                theText = GetComponent<TextMeshProUGUI>();
            }
            theText.text = $"{prefix}{Application.version}{(Debug.isDebugBuild ? debugSuffix : "")}";
        }
    }
}