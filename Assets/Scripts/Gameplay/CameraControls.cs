using System;
using Scripts.Utils.Extensions.Vectors;
using UnityEngine;

namespace Scripts.Gameplay
{
    public class CameraControls : MonoBehaviour
    {

        [SerializeField] private Camera mainCamera;

        [SerializeField] private Camera screenUICamera;

        [SerializeField] private LayerMask screenUIRenderLayerMask;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            
        }
    }
}