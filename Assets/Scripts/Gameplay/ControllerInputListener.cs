using System;
using UnityEngine;
using Scripts.Utils.Annotations;
using Scripts.Utils.Types;
using Scripts.Utils.Extensions.Mathf;

namespace Scripts.Gameplay
{
    public class ControllerInputListener: Singleton<ControllerInputListener>
    {
        [SerializeField] [ReadOnly] private ControllerInputEnum up = ControllerInputEnum.RELEASED;

        [SerializeField] [ReadOnly] private ControllerInputEnum down = ControllerInputEnum.RELEASED;
        
        [SerializeField] [ReadOnly] private ControllerInputEnum left = ControllerInputEnum.RELEASED;
        
        [SerializeField] [ReadOnly] private ControllerInputEnum right = ControllerInputEnum.RELEASED;

        public ControllerInputEnum UP => up;
        public ControllerInputEnum DOWN => down;
        public ControllerInputEnum LEFT => left;
        public ControllerInputEnum RIGHT => right;

        [SerializeField] [ReadOnly] private int lastFrameUpDown = 0;

        [SerializeField] [ReadOnly] private int lastFrameLeftRight = 0;

       


        public bool GetButton(ControlDirection thisDirection)
        {
            return thisDirection switch
            {
                ControlDirection.UP => up.IsHeld(),
                ControlDirection.DOWN => down.IsHeld(),
                ControlDirection.LEFT => left.IsHeld(),
                ControlDirection.RIGHT => right.IsHeld(),
                _ => false
            };
        }
        
        public bool GetButtonDown(ControlDirection thisDirection)
        {
            return thisDirection switch
            {
                ControlDirection.UP => up.IsPressedThisFrame(),
                ControlDirection.DOWN => down.IsPressedThisFrame(),
                ControlDirection.LEFT => left.IsPressedThisFrame(),
                ControlDirection.RIGHT => right.IsPressedThisFrame(),
                _ => false
            };
        }

        private void Awake()
        {
            if (!_AttemptToRegisterInstance)
            {
                Destroy(this);
                return;
            }
            
        }



        private void Update()
        {
            up = up.FrameOverReset();
            down = down.FrameOverReset();
            left = left.FrameOverReset();
            right = right.FrameOverReset();
            
            if (!GameManager.Instance.gameIsRunning)
            {
                return;
            }
            

            var currentUpDown = Input.GetAxisRaw("updown").PosNegZero();
            

            var currentLeftRight = Input.GetAxisRaw("leftright").PosNegZero();

            if (lastFrameUpDown != currentUpDown)
            {

                if (lastFrameUpDown > 0)
                {
                    up = ControllerInputEnum.RELEASED_THIS_FRAME;
                } else if (lastFrameUpDown < 0)
                {
                    down = ControllerInputEnum.RELEASED_THIS_FRAME;
                }
                
                if (currentUpDown > 0)
                {
                    up = ControllerInputEnum.PRESSED_THIS_FRAME;
                }
                else if (currentUpDown < 0)
                {
                    down = ControllerInputEnum.PRESSED_THIS_FRAME;
                }
            }
            
            if (lastFrameLeftRight != currentLeftRight)
            {

                if (lastFrameLeftRight > 0)
                {
                    right = ControllerInputEnum.RELEASED_THIS_FRAME;
                } else if (lastFrameLeftRight < 0)
                {
                    left = ControllerInputEnum.RELEASED_THIS_FRAME;
                }
                
                if (currentLeftRight > 0)
                {
                    right = ControllerInputEnum.PRESSED_THIS_FRAME;
                }
                else if (currentLeftRight < 0)
                {
                    left = ControllerInputEnum.PRESSED_THIS_FRAME;
                }
            }

            lastFrameLeftRight = currentLeftRight;
            lastFrameUpDown = currentUpDown;
        }
    }


    [Serializable]
    public enum ControllerInputEnum
    {
        RELEASED,
        PRESSED_THIS_FRAME,
        HELD,
        RELEASED_THIS_FRAME
    }

    [Serializable]
    public enum ControlDirection
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    public struct UpDownLeftRightStruct
    {
        
    }
    
    public static class ControllerInputEnumExt {

        public static bool IsHeld(this ControllerInputEnum controlInput)
        {
            return (controlInput == ControllerInputEnum.HELD || controlInput == ControllerInputEnum.PRESSED_THIS_FRAME);
        }

        public static bool IsPressedThisFrame(this ControllerInputEnum checkThis)
        {
            return checkThis == ControllerInputEnum.PRESSED_THIS_FRAME;
        }

        public static ControllerInputEnum FrameOverReset(this ControllerInputEnum updateMe)
        {
            return updateMe switch
            {
                ControllerInputEnum.PRESSED_THIS_FRAME => ControllerInputEnum.HELD,
                ControllerInputEnum.RELEASED_THIS_FRAME => ControllerInputEnum.RELEASED,
                _ => updateMe
            };
        }
        
    }
}