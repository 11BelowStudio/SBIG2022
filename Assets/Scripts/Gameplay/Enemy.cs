using System;
using System.Collections.Generic;
using Scripts.Utils.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Gameplay
{
    public class Enemy : MonoBehaviour
    {

        public EnemyEnum whoIs;
        
        public float minTimeForMoveAttempt = 3f;

        public float maxTimeForMoveAttempt = 5f;

        public float AILevel;

        public bool isStarted = false;

        public float patience;

        [SerializeField] [ReadOnly] private float moveAttemptDelta;

        [SerializeField]
        private List<EnemyPositionStruct> _positions = new List<EnemyPositionStruct>();

        public IReadOnlyList<EnemyPositionStruct> Positions => _positions;

        private float NextMoveEventTimer => Random.Range(minTimeForMoveAttempt, maxTimeForMoveAttempt);

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
                myEnemyPosition = _positions[value];

                transform.position = myEnemyPosition.theNode.Position;

                // ReSharper disable once Unity.InefficientPropertyAccess
                transform.rotation = myEnemyPosition.theNode.transform.rotation;
            }
        }
        
        [SerializeField] private int _myCurrentPosition = 0;

        [SerializeField] private int EsiotrotAttackPos = -1;

        [SerializeField] [ReadOnly] private EnemyPositionStruct myEnemyPosition;


        public static event Action<EnemyEnum> StartThisEnemy;

        public event Action<bool> OnBeingWatchedChanged;


        [SerializeField][Unity.Collections.ReadOnly]
        private bool _isBeingWatched = false;


        private void Awake()
        {
            StartThisEnemy += ShouldIStart;
            MyCurrentPosition = 0;

            CameraManager.Instance.OnActiveCameraChanged += AmIBeingWatched;
            CameraManager.Instance.OnCameraActiveStateChanged += AmIBeingWatched;

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

        private void StartAI()
        {
            isStarted = true;
        }


        private void AmIBeingWatched<T>(T _ = default)
        {
            _isBeingWatched =
                CameraManager.Instance.AreCamsActive &&
                myEnemyPosition.theNode.IsThisMyCam(CameraManager.Instance.CurrentCamera);
            
            OnBeingWatchedChanged?.Invoke(_isBeingWatched);
        }
        


        private void Update()
        {
            if (!isStarted)
            {
                return;
            }
            
            
        }


        private void KeinUpdate()
        {
            
        }
        
        

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