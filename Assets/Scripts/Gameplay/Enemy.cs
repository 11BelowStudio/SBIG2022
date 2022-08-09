using System;
using System.Collections.Generic;
using Scripts.Utils.Annotations;
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

        [SerializeField] private int EsiotrotAttackPos = -1;

        [SerializeField][ReadOnly] private int _esiotrotNextStartPos = 1;

        [SerializeField] [ReadOnly] private EnemyPositionStruct myActualPosition;


        public static event Action<EnemyEnum> StartThisEnemy;

        public static event Action<EnemyEnum, float> UpdateEnemyAILevel;

        public event Action<bool> OnBeingWatchedChanged;


        [SerializeField][Unity.Collections.ReadOnly]
        private bool _isBeingWatched = false;

        private List<Enemy> allEnemies = new List<Enemy>();


        private void Awake()
        {

            allEnemies = new List<Enemy>(FindObjectsOfType<Enemy>());
            
            StartThisEnemy += ShouldIStart;
            UpdateEnemyAILevel += ShouldIUpdateMyAiLevel;
            MyCurrentPosition = 0;

            CameraManager.Instance.OnActiveCameraChanged += AmIBeingWatched;
            CameraManager.Instance.OnCameraActiveStateChanged += AmIBeingWatched;

            GameManager.Instance.DoorIsClosedGoAwayGrr += DoorHasJustBeenClosed;
            GameManager.Instance.GameFinishedOneShot += StopAI;

        }

        private void OnValidate()
        {
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

        public static void PleaseToStartThisEnemy(EnemyEnum e)
        {
            StartThisEnemy?.Invoke(e);
        }
        
        public static void PleaseToUpdateThisEnemyAi(EnemyEnum e, float newAi)
        {
            UpdateEnemyAILevel?.Invoke(e, newAi);
        }

        private void StartAI()
        {
            moveAttemptDelta = NextMoveEventTimer;
            isStarted = true;
        }

        private void AmIBeingWatched<T>(T _ = default)
        {
            if (myActualPosition.CamEnum == CameraEnum.OFFICE)
            {
                _isBeingWatched = (GameManager.Instance.ControlState == ControlStateEnum.AT_DOOR);
            }
            else
            {

                _isBeingWatched =
                    CameraManager.Instance.AreCamsActive &&
                    myActualPosition.theNode.IsThisMyCam(CameraManager.Instance.CurrentCamera);
                
            }
            OnBeingWatchedChanged?.Invoke(_isBeingWatched);
        }

        private void StopAI()
        {
            
            if (whoIs != EnemyEnum.IDENTIKIT)
            {
                isStarted = false;
                MyCurrentPosition = 0;
                gameObject.SetActive(false);
            }
        }
        


        private void Update()
        {
            if (!isStarted)
            {
                return;
            }

            if (myActualPosition.isAttackPos)
            {
                Debug.Log($"Enemy {name} is attacking!");
                GameManager.Instance.DisturbanceLevel += (DisturbancePerSecond * Time.deltaTime);
            }
            else if (!PauseCountdownForKeinOrIdendikit)
            {

                moveAttemptDelta -= Time.deltaTime;
                if (moveAttemptDelta <= 0f)
                {
                    moveAttemptDelta = NextMoveEventTimer;

                    if ((!_isBeingWatched) && Random.value <= AILevel)
                    {
                        MoveAttemptSucceeded();
                    }
                }
            }

        }

        private void MoveAttemptSucceeded(int overridePos = -1)
        {
            
            if (whoIs == EnemyEnum.IDENTIKIT)
            {
                Debug.Log("The identikit has moved, increasing enemy AI levels!!!!!");
                foreach (var en in allEnemies)
                {
                    if (en.whoIs != EnemyEnum.IDENTIKIT && en.AILevel < 0.9f)
                    {
                        en.AILevel += 0.1f;
                    }
                }
                FindObjectOfType<TheSourceOfPercival>().TheIdendikitHasMoved();
            }
            
            MyCurrentPosition = (overridePos == -1) ? myActualPosition.nextPos : overridePos;

            
            

            if (myActualPosition.isAttackPos)
            {
                if (GameManager.Instance.ControlState == ControlStateEnum.DOOR_CLOSED)
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

        private void DoorHasJustBeenClosed()
        {
            if (IsAttacking)
            {
                myAudioSource.Stop();
                // If door got closed on the enemy whilst attacking, get sent off


                switch (whoIs)
                {
                    case EnemyEnum.ESIO_TROT:
                        // The esiotrot doesn't get sent back as far on repeat attempts.
                        MoveAttemptSucceeded(_esiotrotNextStartPos);

                        if (_esiotrotNextStartPos < Positions.Count - 2)
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
            EnemyEnum.KEIN => CameraManager.Instance.AreCamsActive,
            EnemyEnum.IDENTIKIT => myActualPosition.CamEnum == CameraEnum.OFFICE &&
                                   GameManager.Instance.ControlState == ControlStateEnum.AT_DOOR,
            _ => false
        };


        private void OnDestroy()
        {
            StartThisEnemy -= ShouldIStart;
            if (CameraManager.TryGetInstance(out CameraManager theCamManager))
            {
                theCamManager.OnActiveCameraChanged -= AmIBeingWatched;
                theCamManager.OnCameraActiveStateChanged -= AmIBeingWatched;
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