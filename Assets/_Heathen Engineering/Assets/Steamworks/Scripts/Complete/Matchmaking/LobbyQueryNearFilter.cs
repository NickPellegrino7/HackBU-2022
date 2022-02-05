#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using System;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public struct LobbyQueryNearFilter
    {
        public string key;
        public int value;
    }
}
#endif