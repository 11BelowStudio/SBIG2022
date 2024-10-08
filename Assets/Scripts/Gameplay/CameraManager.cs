﻿using System;
using Scripts.Utils.Annotations;
using Scripts.Utils.Types;
using UnityEngine;

namespace Scripts.Gameplay
{
    public class CameraManager: Singleton<CameraManager>
    {
        
        public CameraEnum CurrentCamera => _currentCam;
        
        [SerializeField] private CameraEnum _currentCam = CameraEnum.F;
        
        [SerializeField] private CameraState _camState = CameraState.INACTIVE;

        public CameraState CamState => _camState;

        public bool AreCamsActive => _camState == CameraState.ACTIVE;

        [SerializeField] [ReadOnly] private RoomStruct _currentRoom;

        public RoomStruct CurrentCamRoom => _currentRoom;

        public RenderTexture theRenderTexture;

        public AudioClip cameraChangedClip;

        public AudioSource cameraChangeAudioSource;

        private ControllerInputListener controlInputs;

        private void Awake()
        {
            controlInputs = ControllerInputListener.Instance;
        }

        public event Action<CameraState> OnCameraActiveStateChanged;

        public event Action<CameraEnum> OnActiveCameraChanged;

        private void Start()
        {
            if (!_AttemptToRegisterInstance)
            {
                Destroy(this);
                return;
            }
            _currentRoom = GameManager.Instance.Rooms[_currentCam];
            foreach (var roomCam in FindObjectsOfType<RoomCamera>())
            {
                roomCam.SetMyTexture(theRenderTexture);
            }
        }

        public void ShowCams()
        {
            _camState = CameraState.ACTIVE;
            OnCameraActiveStateChanged?.Invoke(_camState);
            OnActiveCameraChanged?.Invoke(_currentCam);
        }

        public void HideCams()
        {
            _camState = CameraState.INACTIVE;
            OnCameraActiveStateChanged?.Invoke(_camState);
        }

        public void CameraButtonPressed(CameraEnum newCam)
        {
            if (newCam == CameraEnum.OFFICE)
            {
                Debug.LogError("ERROR! Somehow, CameraButtonPressed got called with 'Office'!!! Setting to 'A' instead.");
                newCam = CameraEnum.F;
            }
            
            var oldCam = _currentCam;
            _currentCam = newCam;
            if (oldCam != newCam)
            {
                cameraChangeAudioSource.PlayOneShot(cameraChangedClip);
            }
            OnActiveCameraChanged?.Invoke(newCam);
        }

        private void Update()
        {
            if (_camState == CameraState.ACTIVE)
            {
                if (Input.GetButtonDown("1"))
                {
                    CameraButtonPressed(CameraEnum.A);
                } 
                else if (Input.GetButtonDown("2"))
                {
                    CameraButtonPressed(CameraEnum.B);
                } 
                else if (Input.GetButtonDown("3"))
                {
                    CameraButtonPressed(CameraEnum.C);
                }
                else if (Input.GetButtonDown("4"))
                {
                    CameraButtonPressed(CameraEnum.D);
                }
                else if (Input.GetButtonDown("5"))
                {
                    CameraButtonPressed(CameraEnum.E);
                }
                else if (Input.GetButtonDown("6"))
                {
                    CameraButtonPressed(CameraEnum.F);
                }
                else if (Input.GetButtonDown("7"))
                {
                    CameraButtonPressed(CameraEnum.G);
                }
                else if (Input.GetButtonDown("8"))
                {
                    CameraButtonPressed(CameraEnum.BRI_AIN);
                }
                else if (Input.GetButtonDown("9"))
                {
                    CameraButtonPressed(CameraEnum.NUMBER_NINE);
                }
                else if (Input.GetButtonDown("left") || controlInputs.GetButtonDown(ControlDirection.LEFT))
                {
                    CameraButtonPressed(CurrentCamera.MoveLeft());
                }
                else if (Input.GetButtonDown("right") || controlInputs.GetButtonDown(ControlDirection.RIGHT))
                {
                    CameraButtonPressed(CurrentCamera.MoveRight());
                }
            }
        }
        
        
    }
}