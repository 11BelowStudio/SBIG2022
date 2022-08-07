using System;
using UnityEngine;

namespace Scripts.Gameplay
{
    [RequireComponent(typeof(Transform))]
    public class ThingyRotator : MonoBehaviour
    {
        [SerializeField] private Quaternion fromQuaternion;

        [SerializeField] private Quaternion toQuaternion;

        [SerializeField] private float progress = 1f;

        [SerializeField] private float rotationDuration;

        [SerializeField] private float rotationPerSecond;

        private void Update()
        {
            if (progress >= 1f)
            {
                return;
            }

            progress = Mathf.Clamp01(progress + (rotationPerSecond * Time.deltaTime));
            
            transform.localRotation = Quaternion.Slerp(fromQuaternion, toQuaternion, progress);

        }

        public void DoTheRotation(Quaternion from, Quaternion to, float duration = 0f)
        {
            fromQuaternion = from;
            toQuaternion = to;
            if (duration <= 0f)
            {
                transform.localRotation = to;
                progress = 1f;
                return;
            }

            rotationDuration = duration;
            rotationPerSecond = 1 / duration;
            progress = 0f;

        }
    }
}