using System;
using UnityEngine;

namespace Scripts.Gameplay
{
    public class RoomCamera: MonoBehaviour, IHaveACameraEnum
    {
        [SerializeField]
        private CameraEnum _myRoom;

        public CameraEnum CamEnum => _myRoom;

        [SerializeField] private Camera _myCamera;

        public Camera Cam => _myCamera;
        
        private void Awake()
        {
            _myCamera.enabled = false;
            CameraManager.Instance.OnActiveCameraChanged += OnActiveCameraChanged;
            CameraManager.Instance.OnCameraActiveStateChanged += OnCameraActiveStateChanged;
            
        }

        private void OnDestroy()
        {
            if (CameraManager.TryGetInstance(out var theCamManager))
            {
                theCamManager.OnActiveCameraChanged -= OnActiveCameraChanged;
                theCamManager.OnCameraActiveStateChanged -= OnCameraActiveStateChanged;
            }
        }

        private void Start()
        {
            
        }

        public void SetMyTexture(RenderTexture renderTarget)
        {

            _myCamera.targetTexture = renderTarget;
        }


        private void OnActiveCameraChanged(CameraEnum currentActiveCamera)
        {
            _myCamera.enabled = (this.IsThisMyCam(currentActiveCamera));
        }

        private void OnCameraActiveStateChanged(CameraState camState)
        {
            if (camState == CameraState.INACTIVE)
            {
                _myCamera.enabled = false;
            }
        }

        public void Activated()
        {
            _myCamera.enabled = true;
        }

        public void Deactivated()
        {
            _myCamera.enabled = false;
        }
        
        
    }
}