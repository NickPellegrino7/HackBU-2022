#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE
using Steamworks;

namespace HeathenEngineering.SteamworksIntegration
{
    public struct ExchangeEntry
    {
        public SteamItemInstanceID_t instance;
        public uint quantity;
    }

}
#endif