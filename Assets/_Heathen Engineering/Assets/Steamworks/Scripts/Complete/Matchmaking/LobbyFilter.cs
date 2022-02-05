#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using Steamworks;
using System;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public struct LobbyFilter
    {
        public string key;
        public string value;
        public ELobbyComparison method;
    }
}
#endif
