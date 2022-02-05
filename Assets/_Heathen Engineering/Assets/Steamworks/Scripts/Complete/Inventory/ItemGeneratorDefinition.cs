#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeathenEngineering.SteamAPI
{
    [CreateAssetMenu(menuName = "Steamworks/Inventory Item Generator")]
    public class ItemGeneratorDefinition : InventoryItemPointer
    {
        public override InventoryItemType ItemType { get { return InventoryItemType.ItemGenerator; } }

        public List<InventoryItemPointerCount> Content;

        public void TriggerDrop(Action<bool, SteamItemDetails_t[]> callback)
        {
            if(!SteamworksInventoryTools.TriggerItemDrop(definitionID, callback))
            {
                Debug.LogWarning("[ItemGeneratorDefinition.TriggerDrop] - Call failed.");
            }
        }

        public void TriggerDrop()
        {
            var result = SteamworksInventoryTools.TriggerItemDrop(definitionID, (status, results) =>
            {
                if (!status)
                {
                    Debug.LogWarning("[ItemGeneratorDefinition.TriggerDrop] - Call returned an error status.");
                }
            });

            if(!result)
            {
                Debug.LogWarning("[ItemGeneratorDefinition.TriggerDrop] - Call failed.");
            }
        }
    }
}
#endif