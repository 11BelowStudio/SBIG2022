#region

using System.Collections;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#endregion

namespace Scripts.MainMenuStuff.Credits
{

    ///<summary>
    /// The class responsible for showing the credits stuff
    ///</summary>
    public class CreditsInfo : MonoBehaviour//, IAmASubMenu
    {

        ///<summary>
        /// The main menu itself
        ///</summary>
        //[Tooltip("The main menu itself")]
        //public MainMenuRunner theMainMenu;


        ///<summary>
        /// The button that sends the player back to the main menu
        ///</summary>
        [Tooltip("Button that sends the player back to the main menu")]
        public Button backButton;

        ///<summary>
        /// The scrollable content place that the credits entries will be put into
        ///</summary>
        [Tooltip("Scrollable content area for the credits thing (where all of the credits info will be put)")]
        public GameObject creditsContentArea;

        private bool creditsInstantiated = false;

        private bool notStartedInstantiatingCredits = true;


        ///<summary>
        /// The title text thing for the credits menu
        ///</summary>
        [Tooltip("Title text for credits menu")]
        public TextMeshProUGUI credits_title;

        ///<summary>
        ///the PREFAB for the header for asset attributions
        /// </summary>
        [Tooltip("PREFAB for asset attributions.")]
        public GameObject ASSET_HEADER_PREFAB;

        ///<summary>
        ///the PREFAB for asset attribution items
        /// </summary>
        [Tooltip("PREFAB for credits items.")]
        public AssetItem ASSET_ITEM_PREFAB;


        ///<summary>
        ///the PREFAB for the header for the license text stuff
        /// </summary>
        [Tooltip("PREFAB for license text stuff.")]
        public GameObject LICENSE_HEADER_PREFAB;


        ///<summary>
        ///the PREFAB for license text items
        /// </summary>
        [Tooltip("PREFAB for license text items.")]
        public LicenseItem LICENSE_TEXT_ITEM_PREFAB;
        
        
        ///<summary>
        ///the PREFAB for the header for the 'other' info
        /// </summary>
        [Tooltip("PREFAB for the 'other' info")]
        public GameObject OTHER_HEADER_PREFAB;
        

        ///<summary>
        /// Text to show when loading the credits stuff
        ///</summary>
        [Tooltip("object to show when loading the credits stuff")]
        public GameObject loadingObject;

        private Coroutine _loadingCoroutine;

        // Start is called before the first frame update
        void Start()
        {
            //if (theMainMenu == null){
            //    theMainMenu = FindObjectOfType<MainMenuRunner>();
            //}
            
            backButton.onClick.AddListener(
                BackToMainMenu
            );

            _loadingCoroutine = null;
        }

        public void HideMe(){
            backButton.gameObject.SetActive(false);
            loadingObject.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        public void ShowMe(){
            gameObject.SetActive(true);

            backButton.gameObject.SetActive(true);
            InstantiateCredits();

        }


        
        private void InstantiateCredits(){
            if (creditsInstantiated){
                return;
            }
            loadingObject.gameObject.SetActive(true);
            if (notStartedInstantiatingCredits)
            {
                backButton.interactable = false;
                _loadingCoroutine = StartCoroutine(CreditsLoader());
                notStartedInstantiatingCredits = false;
            }
        }


        
        IEnumerator CreditsLoader(){

            TextAsset creditsXML = Resources.Load("CREDITS_DATA") as TextAsset;
            # if UNITY_EDITOR
            Debug.Log(creditsXML);
            # endif
            XmlDocument creditsDoc = new XmlDocument();
            creditsDoc.LoadXml(creditsXML.text);
            
            Transform contentParent = creditsContentArea.gameObject.transform;

            XmlNodeList creditsList = creditsDoc.GetElementsByTagName("credit");
            
            UnityEngine.Debug.Log(creditsList.Count);
            
            if(creditsList.Count > 0)
            {
                yield return null;
                foreach(XmlNode otherItem in creditsList)
                {
                    LicenseItem newItem = Instantiate(LICENSE_TEXT_ITEM_PREFAB, contentParent);
                    newItem.ShowThisInfo(new OtherItemData(otherItem));

                    yield return null;
                }
            }

            creditsList = creditsDoc.GetElementsByTagName("asset");
            
            UnityEngine.Debug.Log(creditsList.Count);

            if (creditsList.Count > 0)
            {
                Instantiate(ASSET_HEADER_PREFAB, contentParent);
                yield return null;
                foreach (XmlNode credItem in creditsList)
                {
                    AssetItem newItem = Instantiate(ASSET_ITEM_PREFAB, contentParent);
                    newItem.ShowThisInfo(new AssetItemData(credItem));
                    yield return null;
                }
            }

            creditsList = creditsDoc.GetElementsByTagName("licensetext");

            if(creditsList.Count > 0)
            {
                Instantiate(LICENSE_HEADER_PREFAB, contentParent);
                yield return null;
                foreach(XmlNode licenseItem in creditsList)
                {
                    LicenseItem newItem = Instantiate(LICENSE_TEXT_ITEM_PREFAB, contentParent);
                    newItem.ShowThisInfo(new LicenseItemData(licenseItem));

                    yield return null;
                }
            }
            
            creditsList = creditsDoc.GetElementsByTagName("other");
            
            if(creditsList.Count > 0)
            {
                Instantiate(OTHER_HEADER_PREFAB, contentParent);
                yield return null;
                foreach(XmlNode otherItem in creditsList)
                {
                    LicenseItem newItem = Instantiate(LICENSE_TEXT_ITEM_PREFAB, contentParent);
                    newItem.ShowThisInfo(new OtherItemData(otherItem));

                    yield return null;
                }
            }

            creditsInstantiated = true;
            loadingObject.gameObject.SetActive(false);
            _loadingCoroutine = null;
            backButton.interactable = true;
            yield break;
        }





        public void BackToMainMenu(){
            if (_loadingCoroutine == null)
            {
                //theMainMenu.CreditsToMain();
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }


}