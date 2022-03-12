#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE
using Steamworks;
using System;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public struct InventoryResult
    {
        public ItemDetail[] items;
        public EResult result;
        public DateTime timestamp;
    }
}
#endif