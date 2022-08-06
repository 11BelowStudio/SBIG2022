using System;
using Scripts.Utils.Extensions.Mathf;
using Unity.Collections;
using UnityEngine;

namespace Scripts.Gameplay
{
    public class MoveBetweenPositions : MonoBehaviour
    {
        [SerializeField]
        private ThingThatMovesBetweenPositions thingThatMovesBetweenPositions;

        [SerializeField] private PositionNode posA;

        [SerializeField] private PositionNode posB;

        [SerializeField] [Utils.Annotations.ReadOnly] private MoveStatesEnum theMoveState;

        [SerializeField] [Utils.Annotations.ReadOnly]
        private float moveTimeSeconds = 5;
        
        [SerializeField] [Utils.Annotations.ReadOnly]
        private float movePerSecond = 0.2f;

        [SerializeField] [Utils.Annotations.ReadOnly]
        private float moveDist = 1f;

        public void DoTheMove(MoveStatesEnum moveToDo, float duration = default)
        {
            duration = duration.ReLU();
            if (Mathf.Approximately(duration, 0f))
            {
                switch (moveToDo)
                {
                    case MoveStatesEnum.AT_A:
                    case MoveStatesEnum.B_TO_A:
                        theMoveState = MoveStatesEnum.AT_A;
                        thingThatMovesBetweenPositions.transform.position = posA.Position;
                        return;
                    case MoveStatesEnum.AT_B:
                    case MoveStatesEnum.A_TO_B:
                        theMoveState = MoveStatesEnum.AT_B;
                        thingThatMovesBetweenPositions.transform.position = posB.Position;
                        return;
                }
                return;
            }

            switch (moveToDo)
            {
                case MoveStatesEnum.AT_A:
                case MoveStatesEnum.A_TO_B:
                    theMoveState = moveToDo;
                    thingThatMovesBetweenPositions.transform.position = posA.Position;
                    if (moveToDo == MoveStatesEnum.AT_A) return;
                    break;
                case MoveStatesEnum.AT_B:
                case MoveStatesEnum.B_TO_A:
                    theMoveState = moveToDo;
                    thingThatMovesBetweenPositions.transform.position = posB.Position;
                    if (moveToDo == MoveStatesEnum.AT_B) return;
                    break;
            }

            moveTimeSeconds = duration;
            movePerSecond = 1f / duration;
            moveDist = 0f;
        }


        private void Update()
        {
            if (moveDist >= 1f){ return; }

            switch (theMoveState)
            {
                case MoveStatesEnum.AT_A:
                case MoveStatesEnum.AT_B:
                    return;
            }

            moveDist = Mathf.Clamp01(moveDist + (movePerSecond * Time.deltaTime));
            
            thingThatMovesBetweenPositions.transform.position = Vector3.Lerp(
                StartPos,
                EndPos,
                moveDist
            );

            if (moveDist >= 1f)
            {
                theMoveState = theMoveState switch
                {
                    MoveStatesEnum.AT_A => MoveStatesEnum.AT_A,
                    MoveStatesEnum.A_TO_B => MoveStatesEnum.AT_B,
                    MoveStatesEnum.AT_B => MoveStatesEnum.AT_B,
                    MoveStatesEnum.B_TO_A => MoveStatesEnum.AT_A,
                    _ => theMoveState
                };
            }



        }

        private Vector3 StartPos => theMoveState switch
        {
            MoveStatesEnum.AT_A => posA.Position,
            MoveStatesEnum.A_TO_B => posA.Position,
            MoveStatesEnum.AT_B => posB.Position,
            MoveStatesEnum.B_TO_A => posB.Position,
            _ => transform.position
        };
        
        private Vector3 EndPos => theMoveState switch
        {
            MoveStatesEnum.AT_A => posA.Position,
            MoveStatesEnum.A_TO_B => posB.Position,
            MoveStatesEnum.AT_B => posB.Position,
            MoveStatesEnum.B_TO_A => posA.Position,
            _ => transform.position
        };

        public bool CheckIfIAmWhatMovesThis(ThingThatMovesBetweenPositions doIMoveThis)
        {
            return doIMoveThis == thingThatMovesBetweenPositions;
        }


        internal void OnDrawGizmosSelected()
        {
            posA.OnDrawGizmos();
            posB.OnDrawGizmos();
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(
                posA.Position,
                posB.Position
            );
        }
    }

    [Serializable]
    public enum MoveStatesEnum
    {
        AT_A,
        A_TO_B,
        AT_B,
        B_TO_A
    }
}