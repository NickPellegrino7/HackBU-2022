#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using Steamworks;
using System.Collections.Generic;
using System;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public struct LobbyQueryParameters
    {
        public int maxResults;
        public bool useDistanceFilter;
        public ELobbyDistanceFilter distanceOption;
        public bool useSlotsAvailable;
        public int requiredOpenSlots;
        public List<LobbyQueryNearFilter> nearValues;
        public List<LobbyQueryNumericFilter> numberValues;
        public List<LobbyQueryStringFilter> stringValues;
        [Obsolete("No longer used, please use the callback paramiter on the MatchmakingTools.FindLobbies method.", true)]
        public Action<LobbyQueryResultList> onQueryComplete;
    }
}
#endif