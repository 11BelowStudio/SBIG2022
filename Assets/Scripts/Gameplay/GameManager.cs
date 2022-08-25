#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Menu;
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

        [SerializeField] private CameraManager camManager;
        [SerializeField] private ControllerInputListener controllerInput;

        [SerializeField] private MainMenu theMainMenu;

        private Dictionary<CameraEnum, RoomStruct> _rooms = new Dictionary<CameraEnum, RoomStruct>();

        [SerializeField] [ReadOnly] private List<RoomStruct> _roomsList = new List<RoomStruct>();
        
        public IReadOnlyDictionary<CameraEnum, RoomStruct> Rooms => _rooms;


        public event Action<EnemyEnum, EnemyPositionStruct> OnEnemyMoved;

        public event Action? DoorIsClosedGoAwayGrr;

        public bool gameIsRunning = false;

        public bool gameHasStarted = false;

        private bool allowedToQuit = true;

        [SerializeField] private float _noiseLimit = 10f;

        [SerializeField] private float _currentDisturbance = 0f;
        
        #if UNITY_EDITOR
        [SerializeField] [ReadOnly] private float timeSinceGameStarted = 0f;
        #endif

        public event Action? GameFinishedOneShot;

        [SerializeField] private OfficeTortoiseHUD _officeTortoiseHUD;

        public event Action NoiseLimitReachedGameOverOneShot;

        [SerializeField] private ControlStateEnum _controlState;

        public event Action<ControlStateEnum, ControlStateEnum> OnControlStateChangedOldNew;

        private float _cameraDrainRatePerSecond = 1 / 15f;

        [SerializeField] private float _cameraRechargeRatePerSecond = 0.25f;

        [SerializeField] [ReadOnly] private float _cameraPowerLevel = 1f;

        public event Action<float>? OnCameraPowerLevelChanged;

        public event Action? OnOutOfCameraPower;

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

        private const string K_VERSION_INDICATOR = "K_VERSION_INDICATOR";

        private float currentEndlessScore = 0;

        public void ScaleTheDisturbanceBarToPunishCampers(float scaleBy)
        {
            _noiseLimit *= scaleBy;
            DisturbanceLevel *= scaleBy;
        }

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

            if (_endlessMode)
            {
                if (currentEndlessScore > PlayerPrefs.GetFloat(K_ENDLESS_HIGH_SCORE, 0f) 
                    || PlayerPrefs.GetString(K_VERSION_INDICATOR, "") != Application.version)
                {
                    PlayerPrefs.SetFloat(K_ENDLESS_HIGH_SCORE, currentEndlessScore);
                    PlayerPrefs.SetString(K_VERSION_INDICATOR, Application.version);
                }
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
            Enemy.PleaseToStartThisEnemy(EnemyEnum.ESIOTROT);
            yield return new WaitForSeconds(10f);
            Enemy.PleaseToStartThisEnemy(EnemyEnum.TESTUDO);
            yield return new WaitForSeconds(10f);
            Enemy.PleaseToStartThisEnemy(EnemyEnum.KEIN);
            yield return new WaitForSeconds(10f);
            Enemy.PleaseToStartThisEnemy(EnemyEnum.INTRUDER);
            yield return new WaitForSeconds(20f);
            Enemy.PleaseToIncrementThisEnemyAI(EnemyEnum.TORTELVIS, 0.0125f);
            Enemy.PleaseToIncrementThisEnemyAI(EnemyEnum.TESTUDO, 0.0125f);
            Enemy.PleaseToIncrementThisEnemyAI(EnemyEnum.KEIN, 0.0125f);
            yield return new WaitForSeconds(60f);
            Enemy.PleaseToIncrementThisEnemyAI(EnemyEnum.TORTELVIS, 0.0125f);
            Enemy.PleaseToIncrementThisEnemyAI(EnemyEnum.TESTUDO, 0.0125f);
            Enemy.PleaseToIncrementThisEnemyAI(EnemyEnum.KEIN, 0.0125f);
            Enemy.PleaseToIncrementThisEnemyAI(EnemyEnum.INTRUDER, 0.0125f);
            while (gameIsRunning)
            {
                yield return new WaitForSeconds(75f);
                if (gameIsRunning)
                {
                    Enemy.PleaseToIncrementThisEnemyAI(EnemyEnum.TORTELVIS, 0.025f);
                    Enemy.PleaseToIncrementThisEnemyAI(EnemyEnum.TESTUDO, 0.025f);
                    Enemy.PleaseToIncrementThisEnemyAI(EnemyEnum.KEIN, 0.025f);
                    Enemy.PleaseToIncrementThisEnemyAI(EnemyEnum.INTRUDER, 0.0125f);
                }
            }
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
            controllerInput = ControllerInputListener.Instance;
            theMainMenu = FindObjectOfType<MainMenu>();
            percivalSource = FindObjectOfType<TheSourceOfPercival>();
            _officeTortoiseHUD = FindObjectOfType<OfficeTortoiseHUD>();

            if (PlayerPrefs.HasKey(K_ENDLESS_HIGH_SCORE))
            {
                endlessModeTextDisplay.gameObject.SetActive(true);

                string endlessModeVersionSuffix = "";
                
                if (PlayerPrefs.HasKey(K_VERSION_INDICATOR))
                {
                    var highScoreVersion = PlayerPrefs.GetString(K_VERSION_INDICATOR, "ERROR");
                    if (Application.version != highScoreVersion)
                    {
                        endlessModeVersionSuffix = $" (on v{K_VERSION_INDICATOR})";
                    }
                }
                else
                {
                    endlessModeVersionSuffix = " (on an old game version)";
                }
                
                endlessModeTextDisplay.text =
                    $"Endless mode high score: {PlayerPrefs.GetFloat(K_ENDLESS_HIGH_SCORE):F}{endlessModeVersionSuffix}";
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
        
        private bool WasQuitInput()
        {

            if (Input.GetButtonDown("quit"))
            {
                return true;
            }

            if (Input.GetButtonDown("joystickBack"))
            {
                if (gameIsRunning || theMainMenu.IsShowingCredits)
                {
                    return false;
                }
            }

            return false;
        }

        private void DoTheQuitting()
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

        void Update()
        {

            if (WasQuitInput())
            {
                DoTheQuitting();
            }

            
            if (!gameIsRunning)
            {
                return;
            }
            
            #if UNITY_EDITOR
            timeSinceGameStarted += Time.deltaTime;
            #endif

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
                    else if (Input.GetButtonDown("joystickBack")) // allows controller users to shut up percival
                    {
                        _officeTortoiseHUD.ShutUpButtonPressed();
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
        
        private bool LeftButtonDown =>
            Input.GetButtonDown("left") || controllerInput.GetButtonDown(ControlDirection.LEFT);

        private bool RightButtonDown =>
            Input.GetButtonDown("right") || controllerInput.GetButtonDown(ControlDirection.RIGHT);

        private bool DownButtonDown =>
            Input.GetButtonDown("down") || controllerInput.GetButtonDown(ControlDirection.DOWN);

        private bool ExitCamerasInput => DownButtonDown || Input.GetButtonDown("door");
        //Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Space);

        private bool ShowCamerasInput => Input.GetButton("up") || controllerInput.GetButton(ControlDirection.UP);
        //Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKey(KeyCode.UpArrow);

        private bool StartClosingDoorInput => Input.GetButtonDown("door");
        //Input.GetKeyDown(KeyCode.Space);
        private bool KeepDoorClosedInput => Input.GetButton("door");
        //Input.GetKey(KeyCode.Space);

        
        
        private bool TurnAroundInput => DownButtonDown
                                        || (_controlState != ControlStateEnum.USING_CAMS && 
                                            (LeftButtonDown || RightButtonDown)
                                        )
                                        || (_controlState == ControlStateEnum.LOOKING_BACK &&
                                            (Input.GetButtonDown("up") || controllerInput.GetButtonDown(ControlDirection.UP)));

        /*
        private bool TurnAroundInput => Input.GetKeyDown(KeyCode.DownArrow)
                                        || (_controlState != ControlStateEnum.USING_CAMS && 
                                            (Input.GetKeyDown(KeyCode.LeftArrow)
                                             || Input.GetKeyDown(KeyCode.RightArrow))
                                            )
                                        || (_controlState == ControlStateEnum.LOOKING_BACK &&
                                            Input.GetKeyDown(KeyCode.UpArrow));
                                            */


        public bool DoorIsClosed => _controlState == ControlStateEnum.DOOR_CLOSED;
    }


    
    
}