using System;
using System.Collections.Generic;
using Scripts.Utils.Annotations;
using Scripts.Utils.Extensions.EnumerableExt;
using Scripts.Utils.Extensions.ListExt;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Gameplay
{
    public class Enemy : MonoBehaviour
    {

        public EnemyEnum whoIs;
        
        public float minTimeForMoveAttempt = 3f;

        public float maxTimeForMoveAttempt = 5f;
        
        public float minIntervalForIntruderAltMoveWhenDoorsClosed = 3f;

        public float maxIntervalForIntruderAltMoveWhenDoorsClosed = 6f;

        public float intruderAltMoveChanceWhenDoorsClosed = 0.5f;

        public float intruderAltMoveChanceAfterEnemyIsBanished = 0.1f;

        private static event Action enemyJustBeenBanishedFromOfficeEvent = null;
        
        private float NextIntruderAltMoveEventTimer => Random.Range(
            minIntervalForIntruderAltMoveWhenDoorsClosed,
            maxIntervalForIntruderAltMoveWhenDoorsClosed
        );

        [SerializeField] [ReadOnly] private float intruderAltMoveDelta = 10f;

        public float AILevel = 0.1f;

        public bool isStarted = false;

        public float patience;

        public float DisturbancePerSecond = 1;

        [SerializeField] [ReadOnly] private float moveAttemptDelta;

        [SerializeField]
        private List<EnemyPositionStruct> _positions = new List<EnemyPositionStruct>();

        public IReadOnlyList<EnemyPositionStruct> Positions => _positions;

        private float NextMoveEventTimer => Random.Range(minTimeForMoveAttempt, maxTimeForMoveAttempt);

        [SerializeField] private List<AudioClip> _moveClips = new List<AudioClip>();

        [SerializeField] private AudioClip _attackAudio;

        [SerializeField] private AudioSource myAudioSource;

        [SerializeField] private List<int> PossibleStartPositionsForTheTestudoAndKein = new List<int>(new int[]{0,0});

        [SerializeField][ReadOnly]
        private bool _hasJustBeenBanishedFromTheOffice = false;
        
        /// <summary>
        /// Time that a player is allowed to keep the door closed for
        /// (without alerting the intruder) after banishing an enemy from the office.
        /// </summary>
        [SerializeField]
        private float justBeenBanishedLeewayDuration = 1.5f;
        
        [SerializeField][ReadOnly]
        private float justBeenBanishedLeewayTimer = 0f;
        public bool HasJustBeenBanishedFromTheOffice => _hasJustBeenBanishedFromTheOffice;

        /// <summary>
        /// Should be true if current position is an attack position or is a position just in front of the door.
        /// (or has just been banished from the office)
        /// </summary>
        public bool IsInAPositionThatIsCloseToTheDoor => (_hasJustBeenBanishedFromTheOffice || myActualPosition.IsAttackPosOrNextPosIsAttackPos);

        private static bool IsThisEnemyInAPositionCloseToTheDoor(Enemy e)
        {
            return e.IsInAPositionThatIsCloseToTheDoor;
        }

        public int MyCurrentPosition
        {
            get => _myCurrentPosition;
            set
            {
                if (value > _positions.Count)
                {
                    throw new ArgumentOutOfRangeException(
                        $"{gameObject.name} {nameof(Enemy)} MyCurrentPosition: Value of {value} is out of range! (limit: {_positions.Count})"
                    );
                }

                _myCurrentPosition = value;
                myActualPosition = _positions[value];

                transform.position = myActualPosition.theNode.Position;

                // ReSharper disable once Unity.InefficientPropertyAccess
                transform.rotation = myActualPosition.theNode.transform.rotation;
            }
        }
        
        [SerializeField] private int _myCurrentPosition = 0;

        /// <summary>
        /// Keeps track of the index for the Esiotrot's 'attack' pos
        /// (so the Esiotrot won't be allowed to start at that position)
        /// </summary>
        [SerializeField] private int esiotrotAttackPos = -1;

        [SerializeField][ReadOnly] private int _esiotrotNextStartPos = 1;

        [SerializeField] [ReadOnly] private EnemyPositionStruct myActualPosition;


        public static event Action<EnemyEnum> StartThisEnemy;

        public static event Action<EnemyEnum, float> UpdateEnemyAILevel;
        
        public static event Action<EnemyEnum, float> IncrementEnemyAILevel;

        public event Action<bool> OnBeingWatchedChanged;


        [SerializeField][Unity.Collections.ReadOnly]
        private bool _isBeingWatched = false;

        /// <summary>
        /// List containing every enemy
        /// </summary>
        private List<Enemy> allEnemies = new List<Enemy>();

        /// <summary>
        /// List containing every enemy except this one.
        /// </summary>
        private List<Enemy> everyoneElse;

        private GameManager gm;

        // has this not yet failed a move attempt due to being watched since the last time its 'is watched' changed?
        private bool notYetFailedAttemptFromBeingWatched = true;

        void ScaleTheIntervalsToPunishCampers(float scaleBy)
        {
            minTimeForMoveAttempt *= scaleBy;
            maxTimeForMoveAttempt *= scaleBy;
            moveAttemptDelta *= scaleBy;
        }

        void ScaleAttackPowerToPunishCampers(float scaleBy)
        {
            DisturbancePerSecond *= scaleBy;
        }

        private void Awake()
        {
            _myCurrentPosition = 0;
            InitializeValidate();
            intruderAltMoveDelta = NextIntruderAltMoveEventTimer;
            allEnemies = new List<Enemy>(FindObjectsOfType<Enemy>());
            
            allEnemies.TrimExcess();

            ISet<Enemy> enemySet = new HashSet<Enemy>(allEnemies);
            enemySet.Remove(this);
            everyoneElse = new List<Enemy>(enemySet);
            
            StartThisEnemy += ShouldIStart;
            UpdateEnemyAILevel += ShouldIUpdateMyAiLevel;
            IncrementEnemyAILevel += ShouldIIncrementMyAiLevel;
            MyCurrentPosition = 0;

            gm = GameManager.Instance;

            CameraManager.Instance.OnActiveCameraChanged += _AmIBeingWatched;
            CameraManager.Instance.OnCameraActiveStateChanged += _AmIBeingWatched;

            gm.DoorIsClosedGoAwayGrr += DoorHasJustBeenClosed;
            gm.GameFinishedOneShot += StopAI;

            if (whoIs == EnemyEnum.INTRUDER)
            {
                enemyJustBeenBanishedFromOfficeEvent = null;
                enemyJustBeenBanishedFromOfficeEvent += IntruderOnEnemyJustBeenBanishedFromOfficeDoThis;
            }

        }

        #if UNITY_EDITOR
        public void OnValidate()
        {
            InitializeValidate();
        }
        #endif

        private void InitializeValidate()
        {
            for (int i = _positions.Count - 1; i >= 0; i--)
            {
                var thisPosition = _positions[i];
                if (thisPosition.isAttackPos || thisPosition.nextPos < 0)
                {
                    continue;
                }
                var theNextPos = _positions[thisPosition.nextPos];
                thisPosition.nextPosIsAttackPos = theNextPos.isAttackPos;
                _positions[i] = thisPosition;
            }
            MyCurrentPosition = _myCurrentPosition;
        }

        private void ShouldIStart(EnemyEnum enemyToStart)
        {
            if (enemyToStart == whoIs)
            {
                StartThisEnemy -= ShouldIStart;
                StartAI();
            }
        }

        private void ShouldIUpdateMyAiLevel(EnemyEnum enemyToUpdate, float newAilevel)
        {
            if (enemyToUpdate == whoIs)
            {
                AILevel = newAilevel;
            }
        }
        
        private void ShouldIIncrementMyAiLevel(EnemyEnum enemyToUpdate, float aiIncrement)
        {
            if (enemyToUpdate == whoIs)
            {
                AILevel += aiIncrement;
                AILevel = Mathf.Min(AILevel, 0.975f);
                Debug.Log($"{gameObject.name} has had AI level increased to {AILevel}!");
            }
        }

        public static void PleaseToStartThisEnemy(EnemyEnum e)
        {
            StartThisEnemy?.Invoke(e);
        }
        
        public static void PleaseToUpdateThisEnemyAI(EnemyEnum e, float newAI)
        {
            UpdateEnemyAILevel?.Invoke(e, newAI);
        }
        
        public static void PleaseToIncrementThisEnemyAI(EnemyEnum e, float incrementBy)
        {
            IncrementEnemyAILevel?.Invoke(e, incrementBy);
        }

        private void StartAI()
        {
            moveAttemptDelta = NextMoveEventTimer;
            isStarted = true;
        }

        private void _AmIBeingWatched<T>(T _ = default)
        {
            AmIBeingWatched();
        }

        private void AmIBeingWatched()
        {
            bool oldBeingWatched = _isBeingWatched;
            
            if (myActualPosition.CamEnum == CameraEnum.OFFICE)
            {
                _isBeingWatched = (gm.ControlState == ControlStateEnum.AT_DOOR 
                                   || gm.ControlState == ControlStateEnum.LOOKING_BACK
                                   || (gm.ControlState == ControlStateEnum.USING_CAMS && (CameraManager.Instance.CurrentCamera == CameraEnum.NUMBER_NINE))
                                   );
            }
            else
            {

                _isBeingWatched =
                    CameraManager.Instance.AreCamsActive &&
                    myActualPosition.theNode.IsThisMyCam(CameraManager.Instance.CurrentCamera);
                
            }
            
            if (oldBeingWatched != _isBeingWatched)
            {
                notYetFailedAttemptFromBeingWatched = (whoIs != EnemyEnum.INTRUDER && !IsInAPositionThatIsCloseToTheDoor);
                OnBeingWatchedChanged?.Invoke(_isBeingWatched);
            }
        }

        

        private void StopAI()
        {
            isStarted = false;
            if (whoIs != EnemyEnum.INTRUDER)
            {
                MyCurrentPosition = 0;
                gameObject.SetActive(false);
            }
        }

        private void IntruderOnEnemyJustBeenBanishedFromOfficeDoThis()
        {
            if (whoIs != EnemyEnum.INTRUDER)
            {
                return;
            }

            if (Random.value <= intruderAltMoveChanceAfterEnemyIsBanished)
            {
                Debug.Log("Enemy banishment caused the Intruder to move!");
                MoveAttemptSucceeded();
            }
        }

        private void Update()
        {
            if (!isStarted)
            {
                return;
            }

            if (_hasJustBeenBanishedFromTheOffice)
            {
                justBeenBanishedLeewayTimer -= Time.deltaTime;
                if (justBeenBanishedLeewayTimer <= 0f)
                {
                    _hasJustBeenBanishedFromTheOffice = false;
                }
            }

            if (myActualPosition.isAttackPos)
            {
                Debug.Log($"Enemy {name} is attacking!");
                gm.DisturbanceLevel += (DisturbancePerSecond * Time.deltaTime);
            }
            else if (!PauseCountdownForKeinOrIdendikit)
            {
                bool doorIsClosed = GameManager.Instance.DoorIsClosed;

                moveAttemptDelta -= Time.deltaTime
                                    * (doorIsClosed ? 1.5f : 1f)
                                    * (IsInAPositionThatIsCloseToTheDoor ? 1.5f : 1f)
                                    * ((_isBeingWatched && notYetFailedAttemptFromBeingWatched) ? 2f : 1f);
                
                if (moveAttemptDelta <= 0f)
                {
                    moveAttemptDelta = NextMoveEventTimer;

                    if (_isBeingWatched)
                    {
                        notYetFailedAttemptFromBeingWatched = false;
                        // Ke'in gets pushed back if his move attempt failed.
                        if (whoIs == EnemyEnum.KEIN && _myCurrentPosition > 0)
                        {
                            MoveAttemptSucceeded(_myCurrentPosition - 1, false);
                        }
                    }
                    else if ( Random.value <= Mathf.Min(0.9f, AILevel * (doorIsClosed ? 1.1f : 1)))
                    {
                        MoveAttemptSucceeded();
                    }
                }
            }

            // NERFING THE OPTIMAL STRATEGY!!!
            if (
                whoIs == EnemyEnum.INTRUDER && 
                gm.ControlState == ControlStateEnum.DOOR_CLOSED &&
                (!allEnemies.Any(IsThisEnemyInAPositionCloseToTheDoor))
                // The door IS allowed to be closed if an enemy is in front of the office/their next position is in front of the office.
                // The intruder's alt timer will count down if the door is closed at any other time.
            )
            {
                intruderAltMoveDelta -= Time.deltaTime;
                if (intruderAltMoveDelta <= 0f)
                {
                    intruderAltMoveDelta = NextIntruderAltMoveEventTimer;

                    if (Random.value <= intruderAltMoveChanceWhenDoorsClosed)
                    {
                        Debug.Log("Looks like the player is trying to be naughty and keep their door constantly closed!");
                        MoveAttemptSucceeded();
                        foreach (var enemy in everyoneElse)
                        {
                            enemy.ScaleTheIntervalsToPunishCampers(0.75f);
                            enemy.ScaleAttackPowerToPunishCampers(1.25f);
                        }
                        FindObjectOfType<TheSourceOfPercival>().ScaleThePercivalDelayToPunishCampers(0.75f);
                        FindObjectOfType<GameManager>().ScaleTheDisturbanceBarToPunishCampers(0.75f);
                    }
                }
            }

        }

        private void MoveAttemptSucceeded(int overridePos = -1, bool saySomething = true)
        {
            
            if (whoIs == EnemyEnum.INTRUDER)
            {
                Debug.Log("The intruder has moved, increasing enemy AI levels!!!!!");
                foreach (var en in everyoneElse)
                {
                    if (en.AILevel < 0.9f)
                    {
                        en.AILevel += 0.1f;
                    }
                }
                FindObjectOfType<TheSourceOfPercival>().TheIntruderHasMoved();
            }
            
            MyCurrentPosition = (overridePos == -1) ? myActualPosition.nextPos : overridePos;

            
            AmIBeingWatched();
            
            

            if (saySomething)
            {
                if (myActualPosition.isAttackPos)
                {
                    if (gm.ControlState == ControlStateEnum.DOOR_CLOSED)
                    {
                        DoorHasJustBeenClosed();
                    }
                    else
                    {
                        if (_attackAudio != null)
                        {
                            myAudioSource.clip = _attackAudio;
                            myAudioSource.loop = true;
                            myAudioSource.Play();
                        }
                    }
                }
                else
                {
                    if (_moveClips.Count > 0)
                    {
                        if (_moveClips.Count == 1)
                        {
                            myAudioSource.PlayOneShot(_moveClips[0]);
                        }
                        else
                        {
                            myAudioSource.PlayOneShot(
                                _moveClips.SwapTheseTwoAndGet(
                                    Random.Range(1, _moveClips.Count)
                                )
                            );
                        }
                    }
                }
            }
        }

        private void DoorHasJustBeenClosed()
        {
            if (isStarted && !allEnemies.Any(IsThisEnemyInAPositionCloseToTheDoor))
            {
                moveAttemptDelta *= Random.Range(0.5f, 1f);
            }
            
            if (IsAttacking)
            {
                justBeenBanishedLeewayTimer = justBeenBanishedLeewayDuration;
                _hasJustBeenBanishedFromTheOffice = true;
                
                myAudioSource.Stop();
                // If door got closed on the enemy whilst attacking, get sent off

                if (whoIs != EnemyEnum.INTRUDER)
                {
                    Debug.Log("An enemy has just been banished, informing the Intruder...");
                    enemyJustBeenBanishedFromOfficeEvent?.Invoke();
                }


                switch (whoIs)
                {
                    case EnemyEnum.ESIOTROT:
                        // The esiotrot doesn't get sent back as far on repeat attempts.
                        MoveAttemptSucceeded(_esiotrotNextStartPos);

                        if (_esiotrotNextStartPos < Positions.Count - 3)
                        {
                            _esiotrotNextStartPos++;
                        }
                        break;
                    case EnemyEnum.TESTUDO:
                    case EnemyEnum.KEIN:
                        // the testudo will attempt a different route (and ke'in might start a bit closer)
                        MoveAttemptSucceeded(
                            PossibleStartPositionsForTheTestudoAndKein.SwapTheseTwoAndGet(
                                Random.Range(0, PossibleStartPositionsForTheTestudoAndKein.Count)
                            )
                        );
                        break;
                    default:
                        // everyone else (tortelvis) just goes back to the start.
                        MoveAttemptSucceeded();
                        break;
                }
                
            }
        }

        public bool IsAttacking => myActualPosition.isAttackPos;


        private void KeinUpdate()
        {
            
        }
        
        


        private bool PauseCountdownForKeinOrIdendikit => whoIs switch
        {
            EnemyEnum.KEIN => CameraManager.Instance.AreCamsActive && 
                              // Timer now actually counts down for Ke'in whilst camera is looking at Bri'ain
                              // so his movement attempts can be forcibly failed.
                              CameraManager.Instance.CurrentCamera != CameraEnum.BRI_AIN,
            EnemyEnum.INTRUDER => myActualPosition.CamEnum == CameraEnum.OFFICE &&
                                  gm.ControlState == ControlStateEnum.AT_DOOR &&
                                  Random.value < 0.75f,
            _ => false
        };


        private void OnDestroy()
        {
            enemyJustBeenBanishedFromOfficeEvent = null;
            StartThisEnemy -= ShouldIStart;
            UpdateEnemyAILevel -= ShouldIUpdateMyAiLevel;
            IncrementEnemyAILevel -= ShouldIIncrementMyAiLevel;
            if (CameraManager.TryGetInstance(out CameraManager theCamManager))
            {
                theCamManager.OnActiveCameraChanged -= _AmIBeingWatched;
                theCamManager.OnCameraActiveStateChanged -= _AmIBeingWatched;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = whoIs.ToColour();

            foreach (var pos in _positions)
            {
                pos.theNode.OnDrawGizmos();
                
                Gizmos.DrawLine(pos.theNode.Position, _positions[pos.nextPos].theNode.Position);
                

                if (pos.isAttackPos)
                {
                    Gizmos.DrawWireCube(pos.theNode.Position, Vector3.one);
                }
            }
            
        }
    }
    
}