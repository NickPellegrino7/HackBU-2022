#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES
using UnityEngine;
using UnityEngine.UI;

namespace HeathenEngineering.SteamAPI.UI
{
    /// <summary>
    /// The base class of a UI element to display a <see cref="RemoteStorageDataFile"/> object in a <see cref="SteamDataFileList"/>
    /// </summary>
    public class SteamDataFileRecord : Button
    {
        /// <summary>
        /// The text field used to display the <see cref="RemoteStorageDataFile"/> name.
        /// </summary>
        [Header("Display Data")]
        public Text FileName;
        /// <summary>
        /// The text field used to display the <see cref="RemoteStorageDataFile"/> time stamp.
        /// </summary>
        public Text Timestamp;
        /// <summary>
        /// A UI object that is enabled when this record is selected and disabled when it is not.
        /// </summary>
        public GameObject SelectedIndicator;
        /// <summary>
        /// The address of this specific file on the Steamworks Remote Storage system.
        /// </summary>
        public RemoteStorageSystem.FileAddress Address;
        /// <summary>
        /// A pointer back to the parent <see cref="SteamDataFileList"/>
        /// </summary>
        [HideInInspector]
        public SteamDataFileList parentList;

        protected override void Start()
        {
            onClick.AddListener(HandleClick);
        }

        private void HandleClick()
        {
            if (parentList != null)
                parentList.SelectedFile = Address;
        }

        private void Update()
        {
            if(parentList != null && parentList.SelectedFile.HasValue && parentList.SelectedFile.Value.fileName == Address.fileName)
            {
                if (!SelectedIndicator.activeSelf)
                    SelectedIndicator.SetActive(true);
            }
            else
            {
                if (SelectedIndicator.activeSelf)
                    SelectedIndicator.SetActive(false);
            }
        }
    }

    public class test : MonoBehaviour
    {

    }
}
#endif