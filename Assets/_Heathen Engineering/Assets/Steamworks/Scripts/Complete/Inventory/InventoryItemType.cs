#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE

namespace HeathenEngineering.SteamAPI
{
    /// <summary>
    /// Indicates the type of item a specific pointer relates to
    /// </summary>
    public enum InventoryItemType
    {
        /// <summary>
        /// Item Definitions are real items e.g. represent a specifc type of item.
        /// </summary>
        ItemDefinition,
        /// <summary>
        /// Item Generators are probabilities of a given type of item and are assessed by the Steamworks backend to resolve into a specific item definition.
        /// </summary>
        ItemGenerator,
        TagGenerator,
        ItemBundle,
    }
}
#endif