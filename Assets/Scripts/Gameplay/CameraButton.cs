using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Gameplay
{
    public class CameraButton: MonoBehaviour, IHaveACameraEnum
    {
        [SerializeField]
        private Button myButton;

        [SerializeField] private CameraEnum whichCamera;

        private void Awake()
        {
            myButton.onClick.AddListener(OnButtonPressed);
        }

        private void OnValidate()
        {
            if (whichCamera == CameraEnum.OFFICE)
            {
                throw new Exception("NOT SUPPOSED TO HAVE A CAMERAENUM THAT'S FOR THE OFFICE!");
            }
        }

        private void OnButtonPressed()
        {
            CameraManager.Instance.CameraButtonPressed(whichCamera);
        }

        public CameraEnum CamEnum => whichCamera;
        
    }
}