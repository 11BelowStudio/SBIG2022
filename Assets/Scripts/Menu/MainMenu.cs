using System;
using Scripts.Gameplay;
using Scripts.MainMenuStuff.Credits;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Menu
{
    public class MainMenu: MonoBehaviour
    {

        [SerializeField] private CanvasGroup entireCanvasGroupForTheWholeMenu;
        
        [SerializeField] private CanvasGroup mainMenuCanvasGroup;
        
        [SerializeField] private CreditsInfo theCredits;

        [SerializeField] private Button startGameButton;

        [SerializeField] private Button showCreditsButton;

        [SerializeField] private Button instructionsPopupButton;

        private void Awake()
        {
            startGameButton.onClick.AddListener(ShowInstructionsPopup);
            instructionsPopupButton.onClick.AddListener(GamerTime);
            showCreditsButton.onClick.AddListener(ShowCredits);
            
            instructionsPopupButton.gameObject.SetActive(false);
            
            theCredits.HideMe();
            entireCanvasGroupForTheWholeMenu.alpha = 1f;
            entireCanvasGroupForTheWholeMenu.interactable = true;
        }

        private void ShowInstructionsPopup()
        {
            mainMenuCanvasGroup.interactable = false;
            mainMenuCanvasGroup.alpha = 0f;
            instructionsPopupButton.gameObject.SetActive(true);
        }

        private void GamerTime()
        {
            entireCanvasGroupForTheWholeMenu.alpha = 0f;
            entireCanvasGroupForTheWholeMenu.interactable = false;
            entireCanvasGroupForTheWholeMenu.gameObject.SetActive(false);
            GameManager.Instance.ItsGamerTime();
        }

        private void ShowCredits()
        {
            mainMenuCanvasGroup.alpha = 0f;
            mainMenuCanvasGroup.interactable = false;
            theCredits.ShowMe();
        }

        public void BackToMainMenu()
        {
            mainMenuCanvasGroup.alpha = 1f;
            mainMenuCanvasGroup.interactable = true;
        }
    }
}