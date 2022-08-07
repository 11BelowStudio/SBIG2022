using System;
using System.Collections.Generic;
using Scripts.Utils.Extensions.ListExt;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Gameplay
{
    public class TheSourceOfPercival : MonoBehaviour
    {
        [SerializeField] private AudioSource percivalAudioSource;

        [SerializeField] private AudioClip percivalIntroAudioClip;

        [SerializeField] private List<AudioClip> percivalMusic = new List<AudioClip>();

        private GameManager _gameManager;

        private OfficeTortoiseHUD _thingThatTellsPercivalToShutUp;


        private void Awake()
        {
            _thingThatTellsPercivalToShutUp = FindObjectOfType<OfficeTortoiseHUD>();

            _gameManager = GameManager.Instance;

            _thingThatTellsPercivalToShutUp.ShutTheFuckUpPercival += ShutUpPercival;
        }

        public void PlayIntroMonologue()
        {
            percivalAudioSource.clip = percivalIntroAudioClip;
            percivalAudioSource.loop = false;
            percivalAudioSource.Play();
            _thingThatTellsPercivalToShutUp.PercivalStartedTalking();
        }

        public bool IsPercivalStillTalking => percivalAudioSource.isPlaying;

        private void ShutUpPercival()
        {
            percivalAudioSource.Stop();
        }

        public void ItsTimeForSomePercivalMusic()
        {
            percivalAudioSource.clip = percivalMusic.SwapTheseTwoAndGet(Random.Range(1, percivalMusic.Count));
            percivalAudioSource.loop = true;
            percivalAudioSource.Play();
            _thingThatTellsPercivalToShutUp.PercivalHasStartedSingingAgain();
        }

    }
}