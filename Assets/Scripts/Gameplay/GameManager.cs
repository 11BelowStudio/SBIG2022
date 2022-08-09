#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Scripts.Utils.Extensions.ListExt;
using UnityEngine;
using Scripts.Utils.Types;
using Scripts.Utils.Annotations;
using TMPro;
using UnityEngine.SceneManagement;

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

        public bool gameHasStarted = false;

        private bool allowedToQuit = true;

        [SerializeField] private float _noiseLimit = 10f;

        [SerializeField] private float _currentDisturbance = 0f;

        public event Action GameFinishedOneShot;

        public event Action NoiseLimitReachedGameOverOneShot;

        [SerializeField] private ControlStateEnum _controlState;

        public event Action<ControlStateEnum, ControlStateEnum> OnControlStateChangedOldNew;

        private float _cameraDrainRatePerSecond = 1 / 15f;

        [SerializeField] private float _cameraRechargeRatePerSecond = 0.25f;

        [SerializeField] [ReadOnly] private float _cameraPowerLevel = 1f;

        public event Action<float> OnCameraPowerLevelChanged;

        public event Action OnOutOfCameraPower;

        public TheSourceOfPercival percivalSource;

        public GameObject Kevin;
        
        public GameObject KevinDestination;

        public PositionNode BanishmentPosition;

        public AudioClip kevinWarpInNoise;

        public AudioSource myAudioSource;

        public AudioClip kevinWarpsYouNoise;

        public AudioClip badEndMusic;

        public AudioClip goodEndMusic;

        public float gameDurationSeconds = 600f;

        private bool _endlessMode = false;

        public bool EndlessMode => _endlessMode;

        public TextMeshProUGUI endlessModeTextDisplay;
        
        private const string K_ENDLESS_HIGH_SCORE = "K_ENDLESS_HIGH_SCORE";

        private float currentEndlessScore = 0;

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
                    GameIsOver(false);
                }
            }
        }


        private void GameIsOver(bool won)
        {
            allowedToQuit = false;
            GameFinishedOneShot?.Invoke();
            GameFinishedOneShot = null;
            gameIsRunning = false;
            ControlState = ControlStateEnum.DED;

            if (_endlessMode && currentEndlessScore > PlayerPrefs.GetFloat(K_ENDLESS_HIGH_SCORE, 0f))
            {
                PlayerPrefs.SetFloat(K_ENDLESS_HIGH_SCORE, currentEndlessScore);
            }
            
            
            if (won)
            {
                StartCoroutine(WonCoroutine());
            }
            else
            {
                StartCoroutine(LostCoroutine());
            }
        }

        private IEnumerator WonCoroutine()
        {
            yield return new WaitForSeconds(3f);
            Kevin.transform.position = KevinDestination.transform.position;
            
            myAudioSource.PlayOneShot(kevinWarpInNoise);
            yield return new WaitForSeconds(3f);
            myAudioSource.clip = goodEndMusic;
            myAudioSource.loop = true;
            myAudioSource.Play();
            var gameOverHud = FindObjectOfType<GameOverHUD>();
            gameOverHud.GoodEnding();
            yield return new WaitForSeconds(3f);
            gameOverHud.ShowPlayAgainButton();
            allowedToQuit = true;
        }
        
        private IEnumerator LostCoroutine()
        {
            yield return new WaitForSeconds(3f);
            Kevin.transform.position = KevinDestination.transform.position;
            myAudioSource.PlayOneShot(kevinWarpInNoise);

            yield return new WaitForSeconds(3f);
            myAudioSource.PlayOneShot(kevinWarpsYouNoise);
            yield return new WaitForSeconds(0.961f);
            // ReSharper disable once PossibleNullReferenceException
            Camera.main.transform.position = BanishmentPosition.Position;
            Camera.main.fieldOfView = 60f;
            yield return new WaitForSeconds(1f);
            myAudioSource.clip = badEndMusic;
            myAudioSource.loop = true;
            myAudioSource.Play();
            var gameOverHud = FindObjectOfType<GameOverHUD>();
            gameOverHud.BadEnding();
            yield return new WaitForSeconds(12.5f);
            gameOverHud.ShowPlayAgainButton();
            allowedToQuit = true;
        }

        private IEnumerator GameTimerCoroutine()
        {
            yield return new WaitForSeconds(gameDurationSeconds);
            if (gameIsRunning)
            {
                // congrats u win
                GameIsOver(true);
            }
        }
        

        public float DisturbanceLevel01
        {
            get => _currentDisturbance / _noiseLimit;
            set => DisturbanceLevel = Mathf.Clamp01(value) * _noiseLimit;
        }
        
        public event Action<float> OnDisturbanceLevelChanged01;

        public void EnableEndlessMode()
        {
            _endlessMode = true;
            currentEndlessScore = 0f;
            endlessModeTextDisplay.gameObject.SetActive(true);
            endlessModeTextDisplay.text = $"Score: {currentEndlessScore:f}";
        }

        public void ItsGamerTime()
        {
            gameIsRunning = true;
            gameHasStarted = true;
            percivalSource.PlayIntroMonologue();

            StartCoroutine(EnemyStarterCoroutine());
            if (!_endlessMode)
            {
                endlessModeTextDisplay.gameObject.SetActive(false);
                StartCoroutine(GameTimerCoroutine());
            }
        }

        private IEnumerator EnemyStarterCoroutine()
        {
            Enemy.PleaseToStartThisEnemy(EnemyEnum.TORTELVIS);
            yield return new WaitForSeconds(10f);
            Enemy.PleaseToStartThisEnemy(EnemyEnum.ESIO_TROT);
            yield return new WaitForSeconds(10f);
            Enemy.PleaseToStartThisEnemy(EnemyEnum.TESTUDO);
            yield return new WaitForSeconds(10f);
            Enemy.PleaseToStartThisEnemy(EnemyEnum.KEIN);
            yield return new WaitForSeconds(5f);
            Enemy.PleaseToStartThisEnemy(EnemyEnum.IDENTIKIT);
        }

        
        private void Awake()
        {
            if (!_AttemptToRegisterInstance)
            {
                Destroy(this);
                return;
            }

            gameHasStarted = false;
            camManager = CameraManager.Instance;
            percivalSource = FindObjectOfType<TheSourceOfPercival>();

            if (PlayerPrefs.HasKey(K_ENDLESS_HIGH_SCORE))
            {
                endlessModeTextDisplay.gameObject.SetActive(true);
                endlessModeTextDisplay.text =
                    $"Endless mode high score: {PlayerPrefs.GetFloat(K_ENDLESS_HIGH_SCORE):F}";
            }
            else
            {
                endlessModeTextDisplay.gameObject.SetActive(false);
            }
            
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

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (gameHasStarted)
                {
                    if (allowedToQuit)
                    {
                        SceneManager.LoadScene(0);
                        return;
                    }
                }
                else
                {
                    #if UNITY_WEBGL
                    SceneManager.LoadScene(0);
                    #else
                    Application.Quit();
                    #endif
                    return;
                }
            }
            
            
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

            if (EndlessMode)
            {
                currentEndlessScore += Time.deltaTime/60;
                endlessModeTextDisplay.text = $"Score: {currentEndlessScore:F}";
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