using System;
using System.Collections.Generic;
using Scripts.Utils.Annotations;
using Scripts.Utils.Extensions.ListExt;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Gameplay
{
    
    /// <summary>
    /// Usable to swap the appearance of Tortelvis randomly
    /// </summary>
    [RequireComponent(typeof(Enemy))]
    public class TortelvisAppearanceSwapper: MonoBehaviour
    {
        [SerializeField] private List<Material> tortelvises = new List<Material>();

        [SerializeField] [ReadOnly] private Material currentTortelvis;

        [SerializeField] private Renderer tortelvisRenderer;
        
        [SerializeField] private Enemy tortelvisHimself;

        private void Awake()
        {
            Initialize();

            tortelvisHimself.OnBeingWatchedChanged += SwapVisibleTortelvis;
        }

        private void Initialize()
        {
            
            currentTortelvis = tortelvises[0];
            ShowTortelvis();
        }

        private void OnValidate()
        {
            Initialize();
            tortelvisHimself = GetComponent<Enemy>();
        }

        private void OnDestroy()
        {
            if (tortelvisHimself != null)
            {
                tortelvisHimself.OnBeingWatchedChanged -= SwapVisibleTortelvis;
            }
        }

        public void SwapVisibleTortelvis(bool shouldISwap = true)
        {
            if (!shouldISwap)
            {
                return;
            }
            //currentTortelvis.SetActive(false);

            currentTortelvis = tortelvises.SwapTheseTwoAndGet(
                Random.Range(1, tortelvises.Count)
                );
            ShowTortelvis();
        }

        private void ShowTortelvis()
        {
            tortelvisRenderer.material = currentTortelvis;
        }
    }
}