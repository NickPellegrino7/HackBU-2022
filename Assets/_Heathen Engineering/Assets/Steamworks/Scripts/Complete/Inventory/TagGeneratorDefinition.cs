#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using Steamworks;
using System.Collections.Generic;
using UnityEngine;

namespace HeathenEngineering.SteamAPI
{
    [CreateAssetMenu(menuName = "Steamworks/Inventory Tag Generator")]
    public class TagGeneratorDefinition : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif
        /// <summary>
        /// This must match the definition ID set in Steamworks for this item
        /// </summary>
        public SteamItemDef_t DefinitionID;

        public string TagName;
        public List<TagGeneratorValue> TagValues;
    }
}
#endif