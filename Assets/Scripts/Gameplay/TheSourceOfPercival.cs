using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Utils.Annotations;
using Scripts.Utils.Extensions.ListExt;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Gameplay
{
    public class TheSourceOfPercival : MonoBehaviour
    {
        [SerializeField] private AudioSource percivalAudioSource;

        [SerializeField] private AudioClip percivalIntroAudioClip;

        [SerializeField] private List<AudioClip> percivalIntros = new List<AudioClip>();

        [SerializeField] private List<AudioClip> percivalMusic = new List<AudioClip>();

        private GameManager _gameManager;

        private OfficeTortoiseHUD _thingThatTellsPercivalToShutUp;

        private Coroutine _percivalAudioCoroutine;

        [SerializeField] private float _percivalDisturbance = 0.5f;


        [SerializeField] private float minPercivalDelay = 10f;

        [SerializeField] private float maxPercivalDelay = 20f;

        private float PercivalDelay => Random.Range(minPercivalDelay, maxPercivalDelay);

        [SerializeField] [ReadOnly] private float percivalDelayRemaining;

        [SerializeField] private bool isPercivalAllowedToAppear = false;

        [SerializeField] private bool isFirstShutUp = true;


        private void Awake()
        {
            _thingThatTellsPercivalToShutUp = FindObjectOfType<OfficeTortoiseHUD>();

            _gameManager = GameManager.Instance;

            _gameManager.GameFinishedOneShot += ShutUpForeverPercival;

            _thingThatTellsPercivalToShutUp.ShutTheFuckUpPercival += ShutUpPercival;

            percivalDelayRemaining = PercivalDelay;
        }

        public void PlayIntroMonologue()
        {
            percivalAudioSource.clip = percivalIntroAudioClip;
            percivalAudioSource.loop = false;
            percivalAudioSource.Play();
            _thingThatTellsPercivalToShutUp.PercivalStartedTalking();
            _percivalAudioCoroutine = StartCoroutine(WaitForPercivalToShutUpWithIntro());
        }

        private IEnumerator WaitForPercivalToShutUpWithIntro()
        {
            yield return new WaitWhile(() => percivalAudioSource.isPlaying);
            isPercivalAllowedToAppear = true;
            percivalDelayRemaining = PercivalDelay;
            isFirstShutUp = false;
            _percivalAudioCoroutine = null;
        }

        public bool IsPercivalStillTalking => percivalAudioSource.isPlaying;

        private void ShutUpForeverPercival()
        {
            isPercivalAllowedToAppear = false;
            ShutUpPercival();
        }

        private void Update()
        {
            if (isPercivalAllowedToAppear && _percivalAudioCoroutine == null)
            {
                percivalDelayRemaining -= Time.deltaTime;
                if (percivalDelayRemaining <= 0)
                {
                    ItsTimeForSomePercivalMusic();
                }
            }
        }

        private void ShutUpPercival()
        {
            percivalAudioSource.Stop();
            percivalDelayRemaining = PercivalDelay;
            if (_percivalAudioCoroutine != null)
            {
                StopCoroutine(_percivalAudioCoroutine);
                _percivalAudioCoroutine = null;
            }

            if (isFirstShutUp && _gameManager.DisturbanceLevel01 < 1)
            {
                isPercivalAllowedToAppear = true;
                isFirstShutUp = false;
            }
            
        }

        public void ItsTimeForSomePercivalMusic()
        {
            _percivalAudioCoroutine = StartCoroutine(PercivalAudioCoroutine());
            _thingThatTellsPercivalToShutUp.PercivalHasStartedSingingAgain();
        }

        public void TheIdendikitHasMoved()
        {
            if (minPercivalDelay > 1f)
            {
                minPercivalDelay -= 1f;
            } else if (maxPercivalDelay > 10f)
            {
                maxPercivalDelay -= 0.5f;
            }
        }

        private IEnumerator PercivalAudioCoroutine()
        {
            var introAudio = percivalIntros.SwapTheseTwoAndGet(Random.Range(1, percivalIntros.Count));
            percivalAudioSource.loop = false;
            percivalAudioSource.clip = introAudio;
            percivalAudioSource.Play();
            yield return new WaitForSeconds(introAudio.length);
            percivalAudioSource.clip = percivalMusic.SwapTheseTwoAndGet(Random.Range(1, percivalMusic.Count));
            percivalAudioSource.loop = true;
            percivalAudioSource.Play();
            while (percivalAudioSource.isPlaying)
            {
                _gameManager.DisturbanceLevel += (_percivalDisturbance * Time.deltaTime);
                yield return null;
            }
            
        }

    }
}