#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeathenEngineering.SteamAPI
{
    /// <summary>
    /// Defines the list of items and quantities required to create some item from some other group of items.
    /// </summary>
    [CreateAssetMenu(menuName = "Steamworks/Crafting Recipe")]
    public class CraftingRecipe : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif
        [Obsolete("Please use items instead")]
        public List<InventoryItemDefinitionCount> Items => items;
        [FormerlySerializedAs("Items")]
        /// <summary>
        /// The list of items and quantities required to create the item this recipie is related to.
        /// </summary>
        public List<InventoryItemDefinitionCount> items;

        public override string ToString()
        {
            string value = "";
            foreach(var item in items)
            {
                value += item.item.definitionID.m_SteamItemDef.ToString() + "x" + item.count.ToString() + ",";
            }
            return value.Remove(value.Length - 1, 1);
        }
    }
}
#endif