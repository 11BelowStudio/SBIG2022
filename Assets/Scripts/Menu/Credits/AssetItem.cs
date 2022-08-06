#region

using TMPro;
using UnityEngine;

#endregion

namespace  Scripts.MainMenuStuff.Credits
 {
    public class AssetItem : MonoBehaviour
    {

        ///<summary>
        /// Text to hold the name of this asset.
        ///</summary>
        [Tooltip("Text to hold the name of this asset")]
        public TextMeshProUGUI assetName;

        ///<summary>
        /// Text to hold the author of this asset.
        ///</summary>
        [Tooltip("Text to hold the info about the asset")]
        public TextMeshProUGUI assetInfo;


        ///<summary>
        /// Is this waiting for the necessary info to be given to it?
        ///</summary>
        bool _awaiting_info = true;


        ///<summary>
        /// Is this waiting for the necessary info to be given to it?
        ///</summary>
        /// <returns>true if still awaiting info to display</returns>
        public bool AwaitingInfo => _awaiting_info;


        void Awake(){
            assetName.text = "";
            assetInfo.text = "";

            
        }


        internal bool ShowThisInfo(AssetItemData data){
            return ShowThisInfo(data.assetname, data.author, data.from, data.license, data.extra);
        }

        private bool ShowThisInfo(string infoName, string author, string from, string license, string extra){
            if (_awaiting_info){
                _awaiting_info = false;
                assetName.text = infoName;

                var byLine = $"By: {author}";

                var fromLine = $"From: {from}";

                var licenseLine = $"License: {license}";

                var extraLine = extra;

                var parentRect = gameObject.transform.parent.GetComponent<RectTransform>();

                //var parentx = parentRect.rect.x-20f;

                
                
                assetInfo.text = $"By: {author}\n{from}\nLicense: {license}{extra}";
                
                //assetInfo.mesh.RecalculateBounds();
                //if (assetInfo.textBounds.size)
                
                float newY = CreditsLoadingUtils.ResizeTMPRectTransformPreferredY(assetName);
                newY += CreditsLoadingUtils.ResizeTMPRectTransformPreferredY(assetInfo);

                RectTransform rt = GetComponent<RectTransform>();

                rt.sizeDelta = new Vector2(rt.sizeDelta.x, newY); //new Vector2(parentx, newY); //new Vector2(rt.sizeDelta.x, newY);
                return true;
            }
            return false;
        }

        private static float ResizeTMPRectTransformPreferredY(TMP_Text tmpro){
            Vector2 rectDelta = tmpro.rectTransform.sizeDelta;
            tmpro.rectTransform.sizeDelta = new Vector2(tmpro.preferredWidth, tmpro.preferredHeight);
            return tmpro.preferredHeight;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}