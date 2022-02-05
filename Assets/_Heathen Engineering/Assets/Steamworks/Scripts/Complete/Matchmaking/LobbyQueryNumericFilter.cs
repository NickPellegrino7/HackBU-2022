#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using Steamworks;
using System;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public struct LobbyQueryNumericFilter
    {
        public string key;
        public int value;
        public ELobbyComparison method;
    }
}
#endif