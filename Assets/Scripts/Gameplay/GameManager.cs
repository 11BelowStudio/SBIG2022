#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Scripts.Utils.Extensions.ListExt;
using UnityEngine;
using Scripts.Utils.Types;
using Scripts.Utils.Annotations;
namespace Scripts.Gameplay
{
    public class GameManager: Singleton<GameManager>
    {

        public CameraManager camManager;

        private Dictionary<CameraEnum, RoomStruct> _rooms = new Dictionary<CameraEnum, RoomStruct>();

        [SerializeField] [ReadOnly] private List<RoomStruct> _roomsList = new List<RoomStruct>();
        
        public IReadOnlyDictionary<CameraEnum, RoomStruct> Rooms => _rooms;


        public event Action<EnemyEnum, EnemyPositionStruct> OnEnemyMoved;

        public event Action DoorIsClosedGoAwayGrr;

        public bool gameIsRunning = false;

        [SerializeField] private float _noiseLimit = 10f;

        [SerializeField] private float _currentDisturbance = 0f;

        public event Action OnNoiseLimitReachedGameOverOneShot;

        [SerializeField] private ControlStateEnum _controlState;

        public event Action<ControlStateEnum, ControlStateEnum> OnControlStateChangedOldNew;

        private float _cameraDrainRatePerSecond = 1 / 15f;

        [SerializeField] private float _cameraRechargeRatePerSecond = 0.25f;

        [SerializeField] [ReadOnly] private float _cameraPowerLevel = 1f;

        public event Action<float> OnCameraPowerLevelChanged;

        public event Action OnOutOfCameraPower;

        public TheSourceOfPercival percivalSource;
        

        public float CameraPowerLevel
        {
            get => _cameraPowerLevel;
            set
            {
                value = Mathf.Clamp01(value);
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value == _cameraPowerLevel)
                {
                    return;
                }
                _cameraPowerLevel = value;
                OnCameraPowerLevelChanged?.Invoke(value);
                if (value <= 0)
                {
                    OnOutOfCameraPower?.Invoke();
                }
            }
        }

        public ControlStateEnum ControlState
        {
            get => _controlState;
            set
            {
                if (value == _controlState)
                {
                    OnControlStateChangedOldNew?.Invoke(value, value);
                    return;
                }

                var oldVal = _controlState;
                _controlState = value;
                OnControlStateChangedOldNew?.Invoke(oldVal, value);

            }
        }

        public float DisturbanceLevel
        {
            get => _currentDisturbance;
            set
            {
                Debug.Log($"Disturbance level called (current: {_currentDisturbance} new: {value})");
                var current = _currentDisturbance;
                value = Mathf.Clamp(value, 0, _noiseLimit);
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != current || value == 0)
                {
                    _currentDisturbance = value;
                    OnDisturbanceLevelChanged01?.Invoke(value / _noiseLimit);
                }

                if (gameIsRunning && value >= _noiseLimit)
                {
                    OnNoiseLimitReachedGameOverOneShot?.Invoke();
                    OnNoiseLimitReachedGameOverOneShot = null;
                    gameIsRunning = false;
                    ControlState = ControlStateEnum.DED;
                }
            }
        }

        public float DisturbanceLevel01
        {
            get => _currentDisturbance / _noiseLimit;
            set => DisturbanceLevel = Mathf.Clamp01(value) * _noiseLimit;
        }
        
        public event Action<float> OnDisturbanceLevelChanged01;
        
        

        public void ItsGamerTime()
        {
            gameIsRunning = true;
            percivalSource.PlayIntroMonologue();

            StartCoroutine(EnemyStarterCoroutine());
        }

        private IEnumerator EnemyStarterCoroutine()
        {
            Enemy.PleaseToStartThisEnemy(EnemyEnum.TORTELVIS);
            yield return new WaitForSeconds(15f);
            Enemy.PleaseToStartThisEnemy(EnemyEnum.ESIO_TROT);
            Enemy.PleaseToStartThisEnemy(EnemyEnum.TESTUDO);
            yield return new WaitForSeconds(15f);
            Enemy.PleaseToStartThisEnemy(EnemyEnum.KEIN);
            yield return new WaitForSeconds(5f);
            Enemy.PleaseToStartThisEnemy(EnemyEnum.IDENTIKIT);
            Enemy.PleaseToUpdateThisEnemyAi(EnemyEnum.TORTELVIS, 0.2f);
        }

        private void Awake()
        {
            if (!_AttemptToRegisterInstance)
            {
                Destroy(this);
                return;
            }

            camManager = CameraManager.Instance;
            percivalSource = FindObjectOfType<TheSourceOfPercival>();
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            // bruh;
        }
        
        private void OnValidate(){
            ReloadRoomStructs();
        }

        private void ReloadRoomStructs()
        {
            List<EnemyLocationNode> allEnemyLocations = new List<EnemyLocationNode>(FindObjectsOfType<EnemyLocationNode>());
            List<RoomCamera> allRoomCameras = new List<RoomCamera>(FindObjectsOfType<RoomCamera>());
            _rooms.Clear();
            _roomsList.Clear();
            foreach (CameraEnum currentCam in Enum.GetValues(typeof(CameraEnum)))
            {
                var thisRoom = new RoomStruct
                {
                    theRoom = currentCam,
                    theRoomCamera = allRoomCameras.FindButDontThrowIfNull(rc => rc.IsThisMyCam(currentCam)),
                    nodesInThisRoom = allEnemyLocations.FindAll(en => en.IsThisMyCam(currentCam))
                };
                _rooms.Add(
                    currentCam,
                    thisRoom
                );
                _roomsList.Add(thisRoom);
                
            }
            
        }

        void Update()
        {
            if (!gameIsRunning)
            {
                return;
            }

            switch (_controlState)
            {
                case ControlStateEnum.DED:
                    return;
                case ControlStateEnum.USING_CAMS:
                    if (ExitCamerasInput)
                    {
                        ControlState = ControlStateEnum.AT_DOOR;
                        camManager.HideCams();
                        RechargeCamThisFrame();
                        break;
                    }
                    DrainCamThisFrame();
                    if (_cameraPowerLevel <= 0)
                    {
                        ControlState = ControlStateEnum.AT_DOOR;
                        camManager.HideCams();
                    }
                    break;
                case ControlStateEnum.AT_DOOR:
                    if (StartClosingDoorInput || KeepDoorClosedInput)
                    {
                        ControlState = ControlStateEnum.DOOR_CLOSED;
                        DoorIsClosedGoAwayGrr?.Invoke();
                        break;
                    }
                    if (ShowCamerasInput && CameraPowerLevel >= 0)
                    {
                        ControlState = ControlStateEnum.USING_CAMS;
                        camManager.ShowCams();
                        DrainCamThisFrame();
                        break;
                    }
                    if (TurnAroundInput)
                    {
                        ControlState = ControlStateEnum.LOOKING_BACK;
                    }
                    RechargeCamThisFrame();
                    break;
                case ControlStateEnum.LOOKING_BACK:
                    if (TurnAroundInput || ShowCamerasInput || StartClosingDoorInput)
                    {
                        ControlState = ControlStateEnum.AT_DOOR;
                    }
                    RechargeCamThisFrame();
                    break;
                case ControlStateEnum.DOOR_CLOSED:
                    if (!KeepDoorClosedInput)
                    {
                        ControlState = ControlStateEnum.AT_DOOR;
                        RechargeCamThisFrame();
                    }
                    break;
            }
        }

        private void RechargeCamThisFrame()
        {
            CameraPowerLevel += (_cameraRechargeRatePerSecond * Time.deltaTime);
        }

        private void DrainCamThisFrame()
        {
            CameraPowerLevel -= (_cameraDrainRatePerSecond * Time.deltaTime);
        }

        private bool ExitCamerasInput => Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Space);

        private bool ShowCamerasInput => Input.GetKeyDown(KeyCode.UpArrow);
        
        private bool StartClosingDoorInput => Input.GetKeyDown(KeyCode.Space);
        private bool KeepDoorClosedInput => Input.GetKey(KeyCode.Space);

        private bool TurnAroundInput => Input.GetKeyDown(KeyCode.DownArrow);
    }


    
    
}