using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Gameplay
{
    public class OfficeDoorHUD: MonoBehaviour
    {

        public Button ShowCamerasButton;

        public CanvasGroup myCanvasGroup;

        public Button DoorControlsButton;

        public Button TurnAroundButton;

        private void Awake()
        {
            
        }

        public void ShowMe()
        {
            myCanvasGroup.alpha = 1f;
            myCanvasGroup.interactable = true;
        }

        public void HideMe()
        {
            myCanvasGroup.alpha = 0f;
            myCanvasGroup.interactable = false;
        }
    }
}