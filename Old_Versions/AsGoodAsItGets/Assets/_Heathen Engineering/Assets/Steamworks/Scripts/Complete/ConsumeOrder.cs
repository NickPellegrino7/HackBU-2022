#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE

namespace HeathenEngineering.SteamworksIntegration
{
    public struct ConsumeOrder
    {
        public Steamworks.SteamItemDetails_t detail;
        public uint quantity;
    }
}
#endif