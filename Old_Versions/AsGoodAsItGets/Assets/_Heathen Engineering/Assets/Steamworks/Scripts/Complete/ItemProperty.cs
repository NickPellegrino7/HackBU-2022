#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE
using System;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public struct ItemProperty
    {
        public string key;
        public string value;
    }
}
#endif