using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scripts.Gameplay
{
    public class GameOverHUD: MonoBehaviour
    {

        public CanvasGroup goodEndingHud;

        public CanvasGroup badEndingHud;

        public CanvasGroup gameOverHudCanvasGroup;

        public Button playAgainButton;

        private void Awake()
        {
            badEndingHud.gameObject.SetActive(false);
            goodEndingHud.gameObject.SetActive(false);
            gameOverHudCanvasGroup.interactable = false;
            gameOverHudCanvasGroup.alpha = 0f;
            
            playAgainButton.onClick.AddListener(PlayAgain);
        }

        public void BadEnding()
        {
            badEndingHud.gameObject.SetActive(true);
            gameOverHudCanvasGroup.interactable = true;
            gameOverHudCanvasGroup.alpha = 1f;
        }

        public void GoodEnding()
        {
            goodEndingHud.gameObject.SetActive(true);
            gameOverHudCanvasGroup.interactable = true;
            gameOverHudCanvasGroup.alpha = 1f;
        }

        private void PlayAgain()
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }
}