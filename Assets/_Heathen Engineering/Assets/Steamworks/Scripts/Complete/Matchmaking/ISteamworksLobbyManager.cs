#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using Steamworks;

namespace HeathenEngineering.SteamAPI
{
    public interface ISteamworksLobbyManager
    {
        /// <summary>
        /// Checks if a search is currently running
        /// </summary>
        bool IsSearching { get; }

        bool IsQuickSearching { get; }

        void CreateLobby(LobbyQueryParameters LobbyFilter, string LobbyName = "", ELobbyType lobbyType = ELobbyType.k_ELobbyTypePublic);

        void JoinLobby(CSteamID lobbyId);

        void LeaveLobby();

        void FindMatch(LobbyQueryParameters LobbyFilter);

        /// <summary>
        /// Starts a staged search for a matching lobby. Search will only start if no searches are currently running.
        /// </summary>
        /// <param name="LobbyFilter"></param>
        /// <param name="autoCreate"></param>
        /// <returns>True if the search was started, false otherwise.</returns>
        bool QuickMatch(LobbyQueryParameters LobbyFilter, string onCreateName, bool autoCreate = false, ELobbyDistanceFilter maxDistance = ELobbyDistanceFilter.k_ELobbyDistanceFilterDefault);

        void CancelQuickMatch();

        void CancelStandardSearch();

        void SendChatMessage(string message);

        void SetLobbyMetadata(string key, string value);

        void SetMemberMetadata(string key, string value);

        void SetLobbyGameServer();

        void SetLobbyGameServer(string address, ushort port, CSteamID steamID);
    }
}
#endif