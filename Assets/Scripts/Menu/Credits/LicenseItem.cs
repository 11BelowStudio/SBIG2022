#region

using TMPro;
using UnityEngine;

#endregion

namespace  Scripts.MainMenuStuff.Credits {
    public class LicenseItem : MonoBehaviour
    {

        ///<summary>
        /// Text to hold the name of this asset.
        ///</summary>
        [Tooltip("Text to hold the name of this asset")]
        public TextMeshProUGUI licenseTextThing;


        ///<summary>
        /// Is this waiting for the necessary info to be given to it?
        ///</summary>
        bool _awaiting_info = true;


        ///<summary>
        /// Is this waiting for the necessary info to be given to it?
        ///</summary>
        /// <returns>true if still awaiting info to display</returns>
        public bool AwaitingInfo{ get => _awaiting_info; }


        internal bool ShowThisInfo(LicenseItemData licenseInfoData){
            return ShowThisInfo(licenseInfoData.licensetext);
        }

        internal bool ShowThisInfo(IHaveASingleTextString infoData)
        {
            return ShowThisInfo(infoData.MyText);
        }

        private bool ShowThisInfo(string licenseInfo){
            if (_awaiting_info){
                _awaiting_info = false;
                licenseTextThing.text = licenseInfo;
                //Vector2 rectDelta = licenseTextThing.rectTransform.sizeDelta;
                
                float newY = CreditsLoadingUtils.ResizeTMPRectTransformPreferredY(licenseTextThing);
                
                //Debug.Log($"new for licenseinfo: {newY} {licenseInfo}");
                
                // float newY = licenseTextThing.preferredHeight;
                //licenseTextThing.rectTransform.sizeDelta = new Vector2(rectDelta.x, newY);

                RectTransform rt = GetComponent<RectTransform>();

                Vector2 rectDelta = rt.sizeDelta;
                rt.sizeDelta = new Vector2(rectDelta.x, newY);
                

                return true;
            }
            return false;
        }

        // Start is called before the first frame update
        void Awake()
        {
            licenseTextThing.text = "";
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }


}