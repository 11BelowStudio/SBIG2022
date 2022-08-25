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

        [SerializeField] private Button endlessModeButton;

        private bool _isShowingCredits = false;

        private bool _isShowingInstructions = false;
        private bool _waitOneFrame = false;

        public bool IsShowingCredits => _isShowingCredits;

        private void Awake()
        {
            _isShowingInstructions = false;
            _isShowingCredits = false;
            _waitOneFrame = false;
            startGameButton.onClick.AddListener(ShowInstructionsPopup);
            instructionsPopupButton.onClick.AddListener(GamerTime);
            showCreditsButton.onClick.AddListener(ShowCredits);
            endlessModeButton.onClick.AddListener(EndlessGamerTime);
            
            instructionsPopupButton.gameObject.SetActive(false);
            
            theCredits.HideMe();
            entireCanvasGroupForTheWholeMenu.alpha = 1f;
            entireCanvasGroupForTheWholeMenu.interactable = true;
        }

        private void ShowInstructionsPopup()
        {
            _waitOneFrame = true;
            _isShowingInstructions = true;
            _isShowingCredits = false;
            mainMenuCanvasGroup.interactable = false;
            mainMenuCanvasGroup.alpha = 0f;
            instructionsPopupButton.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (_isShowingInstructions)
            {
                if (_waitOneFrame)
                {
                    _waitOneFrame = false;
                }
                else if (Input.GetButtonDown("Submit"))
                {
                    GamerTime();
                }
            }
        }

        private void GamerTime()
        {
            _isShowingInstructions = false;
            entireCanvasGroupForTheWholeMenu.alpha = 0f;
            entireCanvasGroupForTheWholeMenu.interactable = false;
            entireCanvasGroupForTheWholeMenu.gameObject.SetActive(false);
            GameManager.Instance.ItsGamerTime();
        }

        private void EndlessGamerTime()
        {
            GameManager.Instance.EnableEndlessMode();
            ShowInstructionsPopup();
        }

        private void ShowCredits()
        {
            _isShowingCredits = true;
            mainMenuCanvasGroup.alpha = 0f;
            mainMenuCanvasGroup.interactable = false;
            theCredits.ShowMe();
        }

        public void BackToMainMenu()
        {
            _isShowingCredits = false;
            mainMenuCanvasGroup.alpha = 1f;
            mainMenuCanvasGroup.interactable = true;
        }
    }
}