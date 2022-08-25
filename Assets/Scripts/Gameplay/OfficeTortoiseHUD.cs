using System;
using System.Collections.Generic;
using Scripts.Utils.Extensions.ListExt;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Scripts.Gameplay
{
    public class OfficeTortoiseHUD: MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup myCanvasGroup;

        [SerializeField] private Button ShutTheFuckUp;

        [SerializeField] private TextMeshProUGUI ShutUpButtonLabel;

        [SerializeField] private RectTransform buttonRandomArea;

        [SerializeField] private List<string> ShutUpWords = new List<string>(new []
        {
            "Oh god no",
            "not again",
            "Shut",
            "make it stop",
            "aaauuugghhhh",
            "whyyyyyy",
            "right that's it"
        });

        public event Action ShutTheFuckUpPercival;

        private void Awake()
        {
            ShutTheFuckUp.onClick.AddListener(ShutUpButtonPressed);

            ShutTheFuckUp.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            ShutUpButtonLabel.text = "ok cool shut up now Percival";
            ShutTheFuckUp.gameObject.SetActive(false);
        }

        public void PercivalStartedTalking()
        {
            ShutTheFuckUp.gameObject.SetActive(true);
        }

        public void ShutUpButtonPressed()
        {
            ShutTheFuckUp.gameObject.SetActive(false);
            ShutTheFuckUpPercival?.Invoke();
            
        }

        public void ShowMe()
        {
            myCanvasGroup.alpha = 1f;
            myCanvasGroup.interactable = true;
        }

        public void HideMe()
        {
            myCanvasGroup.alpha = 0f;
            myCanvasGroup.interactable = false;
        }

        public void PercivalHasStartedSingingAgain()
        {
            ShutTheFuckUp.gameObject.SetActive(true);
            ShutUpButtonLabel.text = ShutUpWords.SwapTheseTwoAndGet(
                Random.Range(1, ShutUpWords.Count));

            var areaRect = buttonRandomArea.rect;

            Vector2 randomRectPos = new Vector2(
                areaRect.width * Random.value,
                areaRect.height * Random.value
            ) + areaRect.position;

            ShutTheFuckUp.GetComponent<RectTransform>().anchoredPosition = randomRectPos;



        }
    }
}