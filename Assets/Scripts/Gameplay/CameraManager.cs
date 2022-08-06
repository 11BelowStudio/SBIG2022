using System;
using Scripts.Utils.Annotations;
using Scripts.Utils.Types;
using UnityEngine;

namespace Scripts.Gameplay
{
    public class CameraManager: Singleton<CameraManager>
    {
        
        public CameraEnum CurrentCamera => _currentCam;
        
        [SerializeField] private CameraEnum _currentCam = CameraEnum.A;
        
        [SerializeField] private CameraState _camState = CameraState.INACTIVE;

        public CameraState CamState => _camState;

        public bool AreCamsActive => _camState == CameraState.ACTIVE;

        [SerializeField] [ReadOnly] private RoomStruct _currentRoom;

        public RoomStruct CurrentCamRoom => _currentRoom;

        public RenderTexture theRenderTexture;

        private void Awake()
        {
            
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
            var oldCam = _currentCam;
            _currentCam = newCam;

            OnActiveCameraChanged?.Invoke(newCam);
        }
        
    }
}