#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE && !UNITY_SERVER
using Steamworks;
using System;

namespace HeathenEngineering.SteamAPI
{
    public class LobbyChatMessageData
    {
        public EChatEntryType chatEntryType;
        public Lobby lobby;
        public LobbyMember sender;
        public DateTime recievedTime;
        public string message;
    }
}
#endif
