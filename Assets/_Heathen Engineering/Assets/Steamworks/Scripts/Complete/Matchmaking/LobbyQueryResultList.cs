#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using Steamworks;
using System.Collections.Generic;
using System;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public class LobbyQueryResultList : List<LobbyQueryResultRecord>
    {
        public Dictionary<string, string> GetLobbyMetaData(CSteamID id)
        {
            if (this.Exists(p => p.lobbyId.m_SteamID == id.m_SteamID))
                return this.Find(p => p.lobbyId.m_SteamID == id.m_SteamID).metadata;
            else
                return new Dictionary<string, string>();
        }
    }
}
#endif
