#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using UnityEngine;

namespace HeathenEngineering.SteamAPI.Demo
{
    /// <summary>
    /// This is a simple example of how to create a custom <see cref="InventoryItemDefinition"/> object for your game.
    /// In most case you would add additional fields so your game client can render rich information about an item such as textures, models, names, descriptions, etc.
    /// The <see cref="InventoryItemDefinition"/> base class provides only the basics that the <see cref="SteamworksInventoryManager"/> needs to operate on the item.
    /// </summary>
    [CreateAssetMenu(menuName = "Steamworks/Examples/Inventory Item Definition")]
    public class ExampleInventoryItemDefinition : InventoryItemDefinition
    {
        //TODO: add game specific information such as icons, 3D models, names, descriptions and more
    }
}
#endif