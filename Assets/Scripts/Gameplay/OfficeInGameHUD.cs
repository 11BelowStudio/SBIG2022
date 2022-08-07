using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Gameplay
{
    public class OfficeInGameHUD: MonoBehaviour
    {

        public GameManager manager;

        public OfficeDoorHUD doorHUD;

        public OfficeTortoiseHUD tortoiseHUD;

        public Slider disturbanceLevelSlider;

        public Slider cameraPowerSlider;

        public Button TurnAroundButton;

        private void Awake()
        {
            manager = GameManager.Instance;

            manager.OnCameraPowerLevelChanged += OnCameraPowerLevelChanged;
            cameraPowerSlider.value = manager.CameraPowerLevel;
            manager.OnDisturbanceLevelChanged01 += OnDisturbanceLevelChanged01;
            disturbanceLevelSlider.value = manager.DisturbanceLevel01;
        }

        private void OnCameraPowerLevelChanged(float newPower)
        {
            cameraPowerSlider.value = newPower;
        }

        private void OnDisturbanceLevelChanged01(float newDisturbance01)
        {
            disturbanceLevelSlider.value = newDisturbance01;
        }
    }
}