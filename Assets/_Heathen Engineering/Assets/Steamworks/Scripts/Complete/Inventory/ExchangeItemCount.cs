#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using Steamworks;
using System;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    /// <summary>
    /// Used for internal processes. This represents the item and quantity to be used in various operations such as item exchanges.
    /// </summary>
    public struct ExchangeItemCount
    {
        public SteamItemInstanceID_t instanceId;
        public uint quantity;
    }
}
#endif