using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;

namespace BrettArnett
{
    public class SteamInventoryListItem : MonoBehaviour
    {
        public string itemName;
        public string itemSpecialization;
        public int itemIdNumber; // This is here to reference if you want it. The player won't have any use for knowing the game's ID numbers of Steam items. You can use it to debug.

        [SerializeField] private Text ItemNameText;

        public void SetSteamInventoryListItemValues()
        {
            ItemNameText.text = itemName;
        }

        public void SelectSteamInventoryItem()
        {
            if (itemSpecialization == "Hat")
            {
                Debug.Log("Hat selected");
            }
            else if (itemSpecialization == "Other")
            {
                Debug.Log("Other selected");
            }
            else
            {
                Debug.Log("No specialization selected");
            }
        }
    }
}