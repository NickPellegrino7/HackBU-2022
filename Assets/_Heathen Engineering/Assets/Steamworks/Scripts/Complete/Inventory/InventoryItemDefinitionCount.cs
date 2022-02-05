#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace HeathenEngineering.SteamAPI
{
    /// <summary>
    /// Used in <see cref="CraftingRecipe"/> to represent a specific item and quantity to be used.
    /// </summary>
    [Serializable]
    public class InventoryItemDefinitionCount
    {
        [Obsolete("Please use item instead")]
        public InventoryItemDefinition Item => item;
        [FormerlySerializedAs("Item")]
        public InventoryItemDefinition item;
        [Obsolete("Please use count instead")]
        public uint Count => count;
        [FormerlySerializedAs("Count")]
        public uint count;

#if !UNITY_SERVER
        /// <summary>
        /// Gets the ExchangeItemCount record for this item optionally decreasing the detail quantities to match that which was consumed
        /// </summary>
        /// <param name="decriment">Should the detail quantities be decrimented to show the use of the items fetched</param>
        /// <returns>Null if insufficent quantity available</returns>
        public List<ExchangeItemCount> FetchFromItem(bool decriment)
        {
            return item.FetchItemCount(count, decriment);
        }

        public override string ToString()
        {
            return item.definitionID.m_SteamItemDef + "x" + count;
        }
#endif
    }
}
#endif