#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using Steamworks;
using System.Collections.Generic;
using System;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public struct LobbyQueryResultRecord
    {
        public string name;
        public CSteamID lobbyId;
        public int maxSlots;
        public CSteamID hostId;
        public Dictionary<string, string> metadata;
    }
}
#endif