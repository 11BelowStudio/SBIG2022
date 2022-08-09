using System;
using UnityEngine;

namespace Scripts.Gameplay
{
    public class Office: MonoBehaviour
    {

        public GameManager gameManager;

        public CameraManager camManager;

        public MoveBetweenPositions DoorMover;

        public MoveBetweenPositions PCMover;

        public Camera playerFaceCamera;

        public ThingyRotator playerFaceCameraRotator;

        public OfficeInGameHUD inGameHud;

        public OfficeTortoiseHUD tortoiseHUD;

        public OfficeDoorHUD doorHUD;
        
        [SerializeField] private Quaternion faceDoorQuaternion = Quaternion.Euler(0, 180, 0);
        [SerializeField] private  Quaternion faceBehindQuaternion = Quaternion.Euler(0, 0, 0);

        public AudioSource doorAudioSource;

        public AudioClip doorClosedAudioClip;

        public AudioSource screenAudioSource;
        
        public AudioClip camsBeHere;

        public AudioClip camsBeGone;

        private void Awake()
        {
            camManager = CameraManager.Instance;
            gameManager = GameManager.Instance;

            playerFaceCamera.transform.localRotation = faceDoorQuaternion;

            camManager.OnCameraActiveStateChanged += OnCameraStateChanged;
            gameManager.OnControlStateChangedOldNew += OnControlStateChanged;

            gameManager.GameFinishedOneShot += GameFinishedYeahhhh;

            tortoiseHUD = FindObjectOfType<OfficeTortoiseHUD>();
            inGameHud = FindObjectOfType<OfficeInGameHUD>();
            doorHUD = FindObjectOfType<OfficeDoorHUD>();
            
            tortoiseHUD.HideMe();
            doorHUD.HideMe();
        }

        private void GameFinishedYeahhhh()
        {
            DoorMover.ChangeMoveState(MoveStatesEnum.AT_A);
            PCMover.ChangeMoveState(MoveStatesEnum.AT_A);
        }

        private void OnCameraStateChanged(CameraState camState)
        {
            switch (camState)
            {
                case CameraState.ACTIVE:
                    PCMover.ChangeMoveState(MoveStatesEnum.A_TO_B, 0.5f);
                    screenAudioSource.PlayOneShot(camsBeHere);
                    break;
                case CameraState.INACTIVE:
                    PCMover.ChangeMoveState(MoveStatesEnum.B_TO_A, 0.5f);
                    screenAudioSource.PlayOneShot(camsBeGone);
                    break;
            }
        }

        private void OnControlStateChanged(ControlStateEnum oldState, ControlStateEnum newState)
        {
            switch (oldState)
            {
                case ControlStateEnum.AT_DOOR:
                    switch (newState)
                    {
                        case ControlStateEnum.AT_DOOR:
                            doorHUD.ShowMe();
                            break;
                        case ControlStateEnum.USING_CAMS:
                            break; // redundant I think. unless I notify OfficeInGameHUD here
                        case ControlStateEnum.DOOR_CLOSED:
                            DoorMover.ChangeMoveState(MoveStatesEnum.A_TO_B, 0.25f);
                            doorAudioSource.PlayOneShot(doorClosedAudioClip);
                            break;
                        case ControlStateEnum.LOOKING_BACK:
                            doorHUD.HideMe();
                            tortoiseHUD.ShowMe();
                            playerFaceCameraRotator.DoTheRotation(faceDoorQuaternion, faceBehindQuaternion, 0.5f);
                            break;
                        case ControlStateEnum.DED:
                            doorHUD.HideMe();
                            break; // redundant
                    }
                    break;
                case ControlStateEnum.LOOKING_BACK:
                    switch (newState)
                    {
                        case ControlStateEnum.LOOKING_BACK:
                            break;
                        case ControlStateEnum.AT_DOOR:
                        case ControlStateEnum.USING_CAMS:
                        case ControlStateEnum.DOOR_CLOSED:
                            doorHUD.ShowMe();
                            tortoiseHUD.HideMe();
                            // might need to do the notifying about the new HUD?
                            playerFaceCameraRotator.DoTheRotation(faceBehindQuaternion, faceDoorQuaternion, 0.5f);
                            break;
                        case ControlStateEnum.DED:
                            tortoiseHUD.HideMe();
                            // might need to do the notifying about the new HUD?
                            playerFaceCameraRotator.DoTheRotation(faceBehindQuaternion, faceDoorQuaternion, 0.5f);
                            break;
                    }
                    break;
                case ControlStateEnum.USING_CAMS:
                    switch (newState)
                    {
                        case ControlStateEnum.LOOKING_BACK:
                            // how???
                            doorHUD.HideMe();
                            tortoiseHUD.ShowMe();
                            playerFaceCameraRotator.DoTheRotation(faceDoorQuaternion, faceBehindQuaternion, 0.5f);
                            break;
                        case ControlStateEnum.USING_CAMS:
                            break;
                        case ControlStateEnum.DED:
                            doorHUD.HideMe();
                            break;
                        case ControlStateEnum.DOOR_CLOSED:
                            DoorMover.ChangeMoveState(MoveStatesEnum.A_TO_B, 0.25f);
                            doorAudioSource.PlayOneShot(doorClosedAudioClip);
                            break;
                        default:
                            // redundant?
                            break;
                    }
                    break;
                case ControlStateEnum.DOOR_CLOSED:
                    switch (newState)
                    {
                        case ControlStateEnum.DOOR_CLOSED:
                            break;
                        case ControlStateEnum.DED:
                            DoorMover.ChangeMoveState(MoveStatesEnum.B_TO_A, 0.25f);
                            doorHUD.HideMe();
                            break;
                        default:
                            DoorMover.ChangeMoveState(MoveStatesEnum.B_TO_A, 0.25f);
                            break;
                    }
                    break;
                case ControlStateEnum.DED:
                    Debug.LogError("No. You are dead, you are not getting back alive that easily.");
                    break;
            }
        }
    }
}