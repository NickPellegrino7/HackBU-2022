#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using Steamworks;

namespace HeathenEngineering.SteamAPI
{
    /// <summary>
    /// Used for internal purpses.
    /// </summary>
    public struct GenerateItemCount
    {
        public SteamItemDef_t ItemId;
        public uint Quantity;
    }
}
#endif