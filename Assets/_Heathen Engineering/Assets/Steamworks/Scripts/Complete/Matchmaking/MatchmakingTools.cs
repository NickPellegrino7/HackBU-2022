#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE && !UNITY_SERVER
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace HeathenEngineering.SteamAPI
{
    /// <summary>
    /// Handles configuration and tracking for Steamworks Lobby
    /// </summary>
    /// <remarks>
    /// <para>
    /// Steamworks allows users to be in 1 'regular' lobby and up to 2 'invisible' lobbies.
    /// This system simply manages a list of lobbies and doesn't try to enforce Valve's rule for 1 regular and 2 invisible.
    /// It is up to the indavidual game developer to handle lobby types and enforce rules regarding how many lobbies a user can be in.
    /// </para>
    /// </remarks>
    public static class MatchmakingTools
    {
        /// <summary>
        /// Enables access to the <see cref="MatchmakingTools"/> static object through instanced pointers such as SteamSettings.Client.matchmaking
        /// </summary>
        public class InstanceWrapper
        {
            /// <summary>
            /// Any non-invisible lobby is considered a "normal" lobby by Valve.
            /// </summary>
            /// <remarks>
            /// It is preferable to access <see cref="lobbies"/> directly.
            /// <para>
            /// The system will set any lobby that is not invisible as the normalLobby.
            /// Note that a user may only be a member of 1 normal lobby at a time so its not possible to have mutliple normal lobbies
            /// </para>
            /// </remarks>
            public List<Lobby> Lobbies => MatchmakingTools.lobbies;
            /// <summary>
            /// Any non-invisible lobby is considered a "normal" lobby by Valve.
            /// </summary>
            /// <remarks>
            /// It is preferable to access <see cref="normalLobby"/> directly.
            /// <para>
            /// The system will set any lobby that is not invisible as the normalLobby.
            /// Note that a user may only be a member of 1 normal lobby at a time so its not possible to have mutliple normal lobbies
            /// </para>
            /// </remarks>
            public Lobby NormalLobby => MatchmakingTools.normalLobby;
            /// <summary>
            /// This is the lobby currently used as the "party" or player group lobby and is always of type Invisible.
            /// </summary>
            /// <remarks>
            /// It is preferable to access <see cref="groupLobby"/> directly.
            /// <para>
            /// Group is a Heathen concept and applies only to invisible type lobbies.
            /// Note that Valve permits a user to be a member of 1 normal lobby and 2 additional invisible lobbies.
            /// You can create a "group" lobby by calling <see cref="CreateGroup(int, Action{Lobby, bool})"/> or by setting <see cref="Lobby.IsGroup"/> to true.
            /// Note it is possible to have 2 lobbies set to "IsParty" but only 1 is tracked on the MatchmakingTools.party property as "the" party. 
            /// When you set <see cref="Lobby.IsGroup"/> to true the lobby type will be changed to invisible and if the <see cref="groupLobby"/> property is not yet set it will be assigned that lobby reference.
            /// </para>
            /// </remarks>
            public Lobby GroupLobby => MatchmakingTools.groupLobby;

            /// <summary>
            /// Is the user in a lobby
            /// </summary>
            /// <remarks>
            /// It is prefereable to access <see cref="MatchmakingTools.InLobby"/> directly.
            /// <para>
            /// This will test all lobbies listed in the <see cref="lobbies"/> field to determin if the user is a member.
            /// If the user is a member of any of the lobbies the result will be true.
            /// </para>
            /// </remarks>
            public bool InLobby => MatchmakingTools.InLobby;
            /// <summary>
            /// True if the system is tracking a lobby
            /// </summary>
            /// <remarks>
            /// It is prefereable to access <see cref="MatchmakingTools.HasLobby"/> directly.
            /// </remarks>
            public bool HasLobby => MatchmakingTools.HasLobby;

            /// <summary>
            /// Creates a new empty lobby. The user will be added to the lobby on creation
            /// </summary>
            /// <param name="lobbyType">The type of lobby to be created ... see Valve's documentation regarding ELobbyType for more informaiton</param>
            /// <param name="memberCountLimit">The limit on the number of users that can join this lobby</param>
            /// <param name="callback">An optional callback to be invoked when the process is complete. { <see cref="Lobby"/> lobbyCreated, <see cref="bool"/> bIOFailure }</param>
            public void CreateLobby(ELobbyType lobbyType, int memberCountLimit, Action<Lobby, bool> callback = null) => MatchmakingTools.CreateLobby(lobbyType, memberCountLimit, callback);

            /// <summary>
            /// Creates an invisible type lobby and stores the result to the <see cref="groupLobby"/> property.
            /// </summary>
            /// <remarks>
            /// This will set 2 metadata fields on the lobby indicating the lobby type and mode
            /// </remarks>
            /// <param name="memberCountLimit">The number of users permited in this lobby.
            /// This can be changed by the host</param>
            /// <param name="callback">An optional callback to be invoked when the process is complete. { <see cref="Lobby"/> lobbyCreated, <see cref="bool"/> bIOFailure }</param>
            public static void CreateGroup(int memberCountLimit, Action<Lobby, bool> callback = null) => MatchmakingTools.CreateGroup(memberCountLimit, callback);

            /// <summary>
            /// Creates a public lobby and stores the result to the <see cref="normalLobby"/> property
            /// </summary>
            /// <remarks>
            /// This will set 2 metadata fields on the lobby indicating the lobby type and mode
            /// </remarks>
            /// <param name="memberCountLimit">The number of users permited in this lobby.
            /// This can be changed by the host</param>
            /// <param name="callback">An optional callback to be invoked when the process is complete. { <see cref="Lobby"/> lobbyCreated, <see cref="bool"/> bIOFailure }</param>
            public static void CreatePublicLobby(int memberCountLimit, Action<Lobby, bool> callback = null) => MatchmakingTools.CreatePublicLobby(memberCountLimit, callback);

            /// <summary>
            /// Joins a steam lobby
            /// </summary>
            /// <param name="lobbyId">The ID of the lobby to join</param>
            /// <remarks>
            /// See <see href="https://partner.steamgames.com/doc/api/ISteamMatchmaking#JoinLobby">JoinLobby</see> in Valve's documentation for more details.
            /// </remarks>
            /// <param name="callback">An optional callback to be invoked when the process is complete. { <see cref="Lobby"/> lobbyCreated, <see cref="bool"/> bIOFailure }</param>
            public static void JoinLobby(CSteamID lobbyId, Action<Lobby, bool> callback = null) => MatchmakingTools.JoinLobby(lobbyId, callback);

            /// <summary>
            /// Joins a steam lobby
            /// </summary>
            /// <param name="lobbyId">The ID of the lobby to join</param>
            /// <param name="callback">An optional callback to be invoked when the process is complete. { <see cref="Lobby"/> lobbyCreated, <see cref="bool"/> bIOFailure }</param>
            /// <remarks>
            /// See <see href="https://partner.steamgames.com/doc/api/ISteamMatchmaking#JoinLobby">JoinLobby</see> in Valve's documentation for more details.
            /// </remarks>
            public static void JoinLobby(ulong lobbyId, Action<Lobby, bool> callback = null) => MatchmakingTools.JoinLobby(new CSteamID(lobbyId), callback);

            /// <summary>
            /// Leaves the current lobby if any
            /// </summary>
            public void LeaveAllLobbies() => MatchmakingTools.LeaveAllLobbies();

            /// <summary>
            /// Searches for a matching lobby according to the provided filter data.
            /// Note that a search will only start if no search is currently running.
            /// </summary>
            /// <param name="filter">Describes the metadata to search for in a lobby</param>
            /// <param name="callback">An optional callback to be invoked when the process is complete. { <see cref="LobbyQueryResultList"/> lobbyResults, <see cref="bool"/> bIOFailure }</param>
            public void FindLobbies(LobbyQueryParameters filter, Action<LobbyQueryResultList, bool> callback = null) => MatchmakingTools.FindLobbies(filter, callback);

            /// <summary>
            /// Sends a chat message via Valve's Lobby Chat system to the first lobby in the <see cref="lobbies"/> list
            /// </summary>
            /// <param name="message">The message to send</param>
            /// <remarks>
            /// <para>
            /// This method exists here for support of older single lobby systems. It is recomended that you use the SendChatMessage on the specific lobby you want to send a message on or that you use the overload that takes the lobby ID.
            /// </para>
            /// </remarks>
            public static void SendChatMessage(string message) => MatchmakingTools.SendChatMessage(message);

            /// <summary>
            /// Send a chat message to the indicated lobby
            /// </summary>
            /// <param name="lobbyId">The lobby to chat on</param>
            /// <param name="message">The message to be sent</param>
            public static void SendChatMessage(CSteamID lobbyId, string message) => MatchmakingTools.SendChatMessage(lobbyId, message);

            /// <summary>
            /// Sets metadata on the first lobby, this can only be called by the host of the lobby
            /// </summary>
            /// <param name="key">The key of the metadata to set</param>
            /// <param name="value">The value of the metadata to set</param>
            /// <remarks>
            /// <para>
            /// This is here to support older single lobby code, it is recomended that you set data directly on the <see cref="Lobby"/> object or use the overload to specify the lobby you want to target.
            /// </para>
            /// </remarks>
            public static void SetLobbyMetadata(string key, string value) => MatchmakingTools.SetLobbyMetadata(key, value);

            /// <summary>
            /// Sets metadata on the first lobby, this can only be called by the host of the lobby
            /// </summary>
            /// <param name="key">The key of the metadata to set</param>
            /// <param name="value">The value of the metadata to set</param>
            public static void SetLobbyMetadata(CSteamID lobbyId, string key, string value) => MatchmakingTools.SetLobbyMetadata(lobbyId, key, value);

            /// <summary>
            /// Sets metadata for the player on the first lobby
            /// </summary>
            /// <param name="key">The key of the metadata to set</param>
            /// <param name="value">The value of the metadata to set</param>
            public static void SetMemberMetadata(string key, string value) => MatchmakingTools.SetMemberMetadata(key, value);

            /// <summary>
            /// Sets metadata for the player on the first lobby
            /// </summary>
            /// <param name="key">The key of the metadata to set</param>
            /// <param name="value">The value of the metadata to set</param>
            public void SetMemberMetadata(CSteamID lobbyId, string key, string value) => MatchmakingTools.SetMemberMetadata(lobbyId, key, value);

            /// <summary>
            /// Sets the lobby game server e.g. game start using the lobby Host as the server ID
            /// </summary>
            /// <remarks>
            /// <para>
            /// This assumes you want to set the game server on the first lobby. It exists to support older code that used a single lobby system.
            /// It is recomended that you call <see cref="Lobby.SetGameServer"/> directly on the lobby you want or use the overload to indicate the lobby.
            /// </para>
            /// <para>
            /// This will trigger GameServerSet on all members of the lobby
            /// This should be called after the server is started
            /// </para>
            /// </remarks>
            public void SetLobbyGameServer() => MatchmakingTools.SetLobbyGameServer();

            /// <summary>
            /// Sets the lobby game server e.g. game start using the lobby Host as the server ID
            /// </summary>
            /// <remarks>
            /// <para>
            /// This will trigger GameServerSet on all members of the lobby
            /// This should be called after the server is started
            /// </para>
            /// </remarks>
            public void SetLobbyGameServer(CSteamID lobbyId) => MatchmakingTools.SetLobbyGameServer(lobbyId);

            /// <summary>
            /// Sets the lobby game server e.g. game start
            /// </summary>
            /// <param name="ipAddress"></param>
            /// <param name="port"></param>
            /// <param name="serverId"></param>
            /// <remarks>
            /// <para>
            /// This assumes you want to set the game server on the first lobby. It exists to support older code that used a single lobby system.
            /// It is recomended that you call <see cref="Lobby.SetGameServer"/> directly on the lobby you want or use the overload to indicate the lobby.
            /// </para>
            /// <para>
            /// This will trigger GameServerSet on all members of the lobby
            /// This should be called after the server is started
            /// </para>
            /// </remarks>
            public void SetLobbyGameServer(string ipAddress, ushort port, CSteamID serverId) => MatchmakingTools.SetLobbyGameServer(ipAddress, port, serverId);

            /// <summary>
            /// Sets the lobby game server e.g. game start
            /// </summary>
            /// <param name="ipAddress"></param>
            /// <param name="port"></param>
            /// <param name="serverId"></param>
            /// <remarks>
            /// <para>
            /// This will trigger GameServerSet on all members of the lobby
            /// This should be called after the server is started
            /// </para>
            /// </remarks>
            public void SetLobbyGameServer(CSteamID lobbyId, string ipAddress, ushort port, CSteamID serverId) => MatchmakingTools.SetLobbyGameServer(lobbyId, ipAddress, port, serverId);

            /// <summary>
            /// Sets the lobby as joinable or not. The default is that a lobby is joinable.
            /// </summary>
            /// <param name="value"></param>
            /// <remarks>
            /// <para>
            /// This assumes you want to set the game server on the first lobby. It exists to support older code that used a single lobby system.
            /// It is recomended that you call <see cref="Lobby.Joinable"/> directly on the lobby you want or use the overload to indicate the lobby.
            /// </para>
            /// </remarks>
            public void SetLobbyJoinable(bool value) => MatchmakingTools.SetLobbyJoinable(value);

            /// <summary>
            /// Sets the lobby as joinable or not. The default is that a lobby is joinable.
            /// </summary>
            /// <param name="value"></param>
            /// <remarks>
            /// <para>
            /// This assumes you want to set the game server on the first lobby. It exists to support older code that used a single lobby system.
            /// It is recomended that you call <see cref="Lobby.Joinable"/> directly on the lobby you want or use the overload to indicate the lobby.
            /// </para>
            /// </remarks>
            public void SetLobbyJoinable(CSteamID lobbyId, bool value) => MatchmakingTools.SetLobbyJoinable(lobbyId, value);

            /// <summary>
            /// Returns information about the lobbies game server
            /// </summary>
            /// <returns></returns>
            /// <remarks>
            /// <para>
            /// This assumes you want to set the game server on the first lobby. It exists to support older code that used a single lobby system.
            /// It is recomended that you call <see cref="Lobby.GameServer"/> directly on the lobby you want or use the overload to indicate the lobby.
            /// </para>
            /// </remarks>
            public LobbyGameServerInformation GetGameServer() => MatchmakingTools.GetGameServer();

            /// <summary>
            /// Returns information about the lobbies game server
            /// </summary>
            /// <param name="lobbyId"></param>
            /// <returns></returns>
            public LobbyGameServerInformation GetGameServer(CSteamID lobbyId) => MatchmakingTools.GetGameServer(lobbyId);

            /// <summary>
            /// Marks the user to be removed
            /// </summary>
            /// <param name="memberId"></param>
            /// <remarks>
            /// <para>
            /// This assumes you want to set the game server on the first lobby. It exists to support older code that used a single lobby system.
            /// It is recomended that you call <see cref="Lobby.GameServer"/> directly on the lobby you want or use the overload to indicate the lobby.
            /// </para>
            /// This creates an entry in the metadata named z_heathenKick which contains a string array of Ids of users that should leave the lobby.
            /// When users detect their ID in the string they will automatically leave the lobby on leaving the lobby the users ID will be removed from the array.
            /// </remarks>
            public void KickMember(CSteamID memberId) => MatchmakingTools.KickMember(memberId);

            /// <summary>
            /// Marks the user to be removed
            /// </summary>
            /// <param name="memberId"></param>
            /// <remarks>
            /// This creates an entry in the metadata named z_heathenKick which contains a string array of Ids of users that should leave the lobby.
            /// When users detect their ID in the string they will automatically leave the lobby on leaving the lobby the users ID will be removed from the array.
            /// </remarks>
            public void KickMember(CSteamID lobbyId, CSteamID memberId) => MatchmakingTools.KickMember(lobbyId, memberId);

            /// <summary>
            /// Sets the indicated user as the new owner of the lobby
            /// </summary>
            /// <param name="newOwner"></param>
            /// <remarks>
            /// <para>
            /// This assumes you want to set the game server on the first lobby. It exists to support older code that used a single lobby system.
            /// It is recomended that you call <see cref="Lobby.OwnerId"/> directly on the lobby you want or use the overload to indicate the lobby.
            /// </para>
            /// <para>
            /// This does not effect the NetworkManager or other networking funcitonality it only changes the ownership of a lobby
            /// </para>
            /// </remarks>
            public void ChangeOwner(CSteamID newOwner) => MatchmakingTools.ChangeOwner(newOwner);

            /// <summary>
            /// Sets the indicated user as the new owner of the lobby
            /// </summary>
            /// <param name="newOwner"></param>
            /// <remarks>
            /// <para>
            /// This does not effect the NetworkManager or other networking funcitonality it only changes the ownership of a lobby
            /// </para>
            /// </remarks>
            public void ChangeOwner(CSteamID lobbyId, CSteamID newOwner) => MatchmakingTools.ChangeOwner(lobbyId, newOwner);
        }

        public static readonly List<Lobby> lobbies = new List<Lobby>();
        /// <summary>
        /// Any non-invisible lobby is considered a "normal" lobby by Valve.
        /// </summary>
        /// <remarks>
        /// The system will set any lobby that is not invisible as the normalLobby.
        /// Note that a user may only be a member of 1 normal lobby at a time so its not possible to have mutliple normal lobbies
        /// </remarks>
        public static Lobby normalLobby;
        /// <summary>
        /// This is the lobby currently used as the "party".
        /// </summary>
        /// <remarks>
        /// Group is a Heathen concept and applies only to invisible type lobbies.
        /// Note that Valve permits a user to be a member of 1 normal lobby and 2 additional invisible lobbies.
        /// You can create a "group" lobby by calling <see cref="CreateGroup(int, Action{Lobby, bool})"/> or by setting <see cref="Lobby.IsGroup"/> to true.
        /// Note it is possible to have 2 lobbies set to "IsParty" but only 1 is tracked on the MatchmakingTools.party property as "the" party. 
        /// When you set <see cref="Lobby.IsGroup"/> to true the lobby type will be changed to invisible and if the <see cref="groupLobby"/> property is not yet set it will be assigned that lobby reference.
        /// </remarks>
        public static Lobby groupLobby;

        /// <summary>
        /// Is the user in a lobby
        /// </summary>
        /// <remarks>
        /// <para>
        /// This will test all lobbies listed in the <see cref="lobbies"/> field to determin if the user is a member.
        /// If the user is a member of any of the lobbies the result will be true.
        /// </para>
        /// </remarks>
        public static bool InLobby
        {
            get
            {
                if (lobbies.Any(p => p != null && p.User != null))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// True if the system is tracking a lobby
        /// </summary>
        public static bool HasLobby
        {
            get
            {
                return lobbies.Any(p => p.id != CSteamID.Nil);
            }
        }

        [Obsolete("No longer used")]
        public static bool IsSearching
        {
            get { return false; }// standardSearch; }
        }

        [Obsolete("No longer used")]
        public static bool IsQuickSearching
        {
            get { return false; }// quickMatchSearch; }
        }

        public static bool Initalized => callbacksRegistered;

        #region Internal Data
        //private static bool standardSearch = false;
        //private static bool quickMatchSearch = false;
        private static bool callbacksRegistered = false;
        //private static LobbyQueryParameters quickMatchFilter;
        #endregion

        #region Callbacks
#pragma warning disable IDE0052 // Remove unread private members
        private static CallResult<LobbyCreated_t> m_LobbyCreated;
        private static CallResult<LobbyEnter_t> m_LobbyEntered;
        private static Callback<GameLobbyJoinRequested_t> m_GameLobbyJoinRequested;
        private static Callback<LobbyChatUpdate_t> m_LobbyChatUpdate;
        private static CallResult<LobbyMatchList_t> m_LobbyMatchList;
        private static Callback<LobbyGameCreated_t> m_LobbyGameCreated;
        private static Callback<LobbyDataUpdate_t> m_LobbyDataUpdated;
        private static Callback<LobbyChatMsg_t> m_LobbyChatMsg;
#pragma warning restore IDE0052 // Remove unread private members
        #endregion

        #region Events
        /// <summary>
        /// Occures when a request to join the lobby has been recieved such as through Steamworks's invite friend dialog in the Steamworks Overlay
        /// </summary>
        [HideInInspector]
        public static UnityGameLobbyJoinRequestedEvent evtGameLobbyJoinRequest = new UnityGameLobbyJoinRequestedEvent();
        /// <summary>
        /// Occures when list of Lobbies is retured from a search
        /// </summary>
        [HideInInspector]
        public static UnityLobbyQueryResultListEvent evtLobbyMatchList = new UnityLobbyQueryResultListEvent();
        /// <summary>
        /// Occures when a lobby is created by the player
        /// </summary>
        /// <remarks>
        /// <para>
        /// The data from this event can be used to fetch the newly created lobby. A demonstration of this is availabel in the example below.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// public SteamworksLobbySettings lobbySettings;
        /// ...
        /// void Start()
        /// {
        ///    lobbySettings.OnLobbyCreate.AddListener(HandleOnLobbyCreated);
        /// }
        /// ...
        /// private void HandleOnLobbyCreated(LobbyCreated_t param)
        /// {
        ///    var myNewLobby = lobbySettings[param.m_ulSteamIDLobby];
        /// }
        /// </code>
        /// </example>
        [HideInInspector]
        public static UnityLobbyCreatedEvent evtLobbyCreated = new UnityLobbyCreatedEvent();

        [HideInInspector]
        public static UnitySteamIdEvent evtLobbyJoinFailed = new UnitySteamIdEvent();
        /// <summary>
        /// Occures when the player joins a lobby
        /// </summary>
        [HideInInspector]
        public static UnityLobbyEvent evtLobbyEnter = new UnityLobbyEvent();
        /// <summary>
        /// Occures when the player leaves a lobby
        /// </summary>
        [HideInInspector]
        public static UnityLobbyEvent evtLobbyExit = new UnityLobbyEvent();

        /// <summary>
        /// Occures when the host of the lobby starts the game e.g. sets game server data on the lobby
        /// </summary>
        [HideInInspector]
        public static UnityLobbyGameCreatedEvent evtGameServerSet = new UnityLobbyGameCreatedEvent();
        /// <summary>
        /// Occures when lobby chat metadata has been updated such as a kick or ban.
        /// </summary>
        [HideInInspector]
        public static UnityLobbyChatUpdateEvent evtLobbyChatUpdate = new UnityLobbyChatUpdateEvent();
        /// <summary>
        /// Occures when a quick match search fails to return a lobby match
        /// </summary>
        [HideInInspector]
        public static UnityEvent evtQuickMatchFailed = new UnityEvent();
        /// <summary>
        /// Occures when a search for a lobby has started
        /// </summary>
        [HideInInspector]
        public static UnityEvent evtSearchStarted = new UnityEvent();
        /// <summary>
        /// Occures when a lobby chat message is recieved
        /// </summary>
        [HideInInspector]
        public static LobbyChatMessageEvent evtChatMessageReceived = new LobbyChatMessageEvent();
        #endregion

        /// <summary>
        /// Typically called by the HeathenSteamManager.OnEnable()
        /// This registeres the Valve callbacks and CallResult deligates
        /// </summary>
        public static void Initalize()
        {
            if (!callbacksRegistered)
            {
                callbacksRegistered = true;
                m_LobbyCreated = CallResult<LobbyCreated_t>.Create(HandleLobbyCreated);
                m_LobbyEntered = CallResult<LobbyEnter_t>.Create(HandleLobbyEntered);
                m_LobbyMatchList = CallResult<LobbyMatchList_t>.Create();
                m_GameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(HandleGameLobbyJoinRequested);
                m_LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(HandleLobbyChatUpdate);
                m_LobbyGameCreated = Callback<LobbyGameCreated_t>.Create(HandleLobbyGameCreated);
                m_LobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(HandleLobbyDataUpdate);
                m_LobbyChatMsg = Callback<LobbyChatMsg_t>.Create(HandleLobbyChatMessage);
            }
        }

        #region Callbacks
        private static void SetLobbyFilter(LobbyQueryParameters LobbyFilter)
        {
            if (LobbyFilter.useSlotsAvailable)
                SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(LobbyFilter.requiredOpenSlots);

            if (LobbyFilter.useDistanceFilter)
                SteamMatchmaking.AddRequestLobbyListDistanceFilter(LobbyFilter.distanceOption);

            if (LobbyFilter.maxResults > 0)
                SteamMatchmaking.AddRequestLobbyListResultCountFilter(LobbyFilter.maxResults);

            if (LobbyFilter.numberValues != null)
            {
                foreach (var f in LobbyFilter.numberValues)
                    SteamMatchmaking.AddRequestLobbyListNumericalFilter(f.key, f.value, f.method);
            }

            if (LobbyFilter.nearValues != null)
            {
                foreach (var f in LobbyFilter.nearValues)
                    SteamMatchmaking.AddRequestLobbyListNearValueFilter(f.key, f.value);
            }

            if (LobbyFilter.stringValues != null)
            {
                foreach (var f in LobbyFilter.stringValues)
                    SteamMatchmaking.AddRequestLobbyListStringFilter(f.key, f.value, f.method);
            }
        }

        private static void HandleLobbyGameCreated(LobbyGameCreated_t param)
        {
            var lobby = lobbies.FirstOrDefault(p => p.id.m_SteamID == param.m_ulSteamIDLobby);

            if (lobby != null)
            {
                lobby.HandleLobbyGameCreated(param);
                evtGameServerSet.Invoke(param);
            }
        }

        private static void HandleLobbyChatUpdate(LobbyChatUpdate_t param)
        {
            var lobby = lobbies.FirstOrDefault(p => p.id.m_SteamID == param.m_ulSteamIDLobby);
            lobby.HandleLobbyChatUpdate(param);

            evtLobbyChatUpdate.Invoke(param);
        }

        private static void HandleGameLobbyJoinRequested(GameLobbyJoinRequested_t param)
        {
            //JoinLobby(param.m_steamIDLobby);
            evtGameLobbyJoinRequest.Invoke(param);
        }

        private static void HandleLobbyEntered(LobbyEnter_t param, bool bIOFailure)
        {
            Debug.Log("Handling lobby ener for lobby: " + param.m_ulSteamIDLobby);

            var lobby = lobbies.FirstOrDefault(p => p.id.m_SteamID == param.m_ulSteamIDLobby);
            if (lobby == null)
            {
                lobby = new Lobby(new CSteamID(param.m_ulSteamIDLobby));
                lobby.evtExitLobby.AddListener(HandleExitLobby);
                lobbies.Add(lobby);
            }

            evtLobbyEnter.Invoke(lobby);
        }

        private static void HandleLobbyCreated(LobbyCreated_t param, bool bIOFailure)
        {
            Debug.LogWarning("Default lobby created was called on lobby " + param.m_ulSteamIDLobby);

            var lobby = lobbies.FirstOrDefault(p => p.id.m_SteamID == param.m_ulSteamIDLobby);
            if (lobby == null)
            {
                lobby = new Lobby(new CSteamID(param.m_ulSteamIDLobby));
                lobby.evtExitLobby.AddListener(HandleExitLobby);
                lobbies.Add(lobby);
            }

            evtLobbyCreated.Invoke(param);
        }

        private static void HandleLobbyDataUpdate(LobbyDataUpdate_t param)
        {
            var lobby = lobbies.FirstOrDefault(p => p.id.m_SteamID == param.m_ulSteamIDLobby);
            lobby?.HandleLobbyDataUpdate(param);
        }

        private static void HandleLobbyChatMessage(LobbyChatMsg_t param)
        {
            var lobby = lobbies.FirstOrDefault(p => p.id.m_SteamID == param.m_ulSteamIDLobby);
            if (lobby != null)
            {
                var message = lobby.HandleLobbyChatMessage(param);

                if (message != null)
                    evtChatMessageReceived.Invoke(message);
            }
        }

        private static void HandleExitLobby(Lobby lobby)
        {
            lobbies.RemoveAll(p => p.id == lobby.id);
            
            if (normalLobby == lobby)
                normalLobby = null;
            
            if (groupLobby == lobby)
                groupLobby = null;

            evtLobbyExit.Invoke(lobby);
        }
        #endregion

        /// <summary>
        /// Creates a new empty lobby. The user will be added to the lobby on creation
        /// </summary>
        /// <param name="lobbyType">The type of lobby to be created ... see Valve's documentation regarding ELobbyType for more informaiton</param>
        /// <param name="memberCountLimit">The limit on the number of users that can join this lobby</param>
        /// <param name="callback">An optional callback to be invoked when the process is complete. { <see cref="Lobby"/> lobbyCreated, <see cref="bool"/> bIOFailure }</param>
        public static void CreateLobby(ELobbyType lobbyType, int memberCountLimit, Action<Lobby, bool> callback = null)
        {
            Initalize();

            if (normalLobby != null && lobbyType != ELobbyType.k_ELobbyTypeInvisible)
            {
                Debug.LogWarning("Attempting to create a normal typed lobby while already a member of a normal lobby. Valve is likely to reject this.\nUsers may only be a member of 1 normal lobby and 2 invisible lobbies at a time.");
            }


            var call = SteamMatchmaking.CreateLobby(lobbyType, memberCountLimit);
            m_LobbyCreated.Set(call, (c, e) =>
            {
                Debug.Log("General Lobby Created");

                var lobby = lobbies.FirstOrDefault(p => p.id.m_SteamID == c.m_ulSteamIDLobby);
                if (!e)
                {
                    if (lobby == null)
                    {
                        lobby = new Lobby(new CSteamID(c.m_ulSteamIDLobby));
                        lobby.evtExitLobby.AddListener(HandleExitLobby);
                        lobbies.Add(lobby);
                    }

                    switch (lobbyType)
                    {
                        case ELobbyType.k_ELobbyTypePublic:
                            lobby[Lobby.DataType] = "Public";
                            normalLobby = lobby;
                            break;
                        case ELobbyType.k_ELobbyTypePrivate:
                            lobby[Lobby.DataType] = "Private";
                            normalLobby = lobby;
                            break;
                        case ELobbyType.k_ELobbyTypeFriendsOnly:
                            lobby[Lobby.DataType] = "Friend";
                            normalLobby = lobby;
                            break;
                        case ELobbyType.k_ELobbyTypeInvisible:
                            lobby[Lobby.DataType] = "Invisible";
                            break;
                    }

                    lobby[Lobby.DataMode] = "General";

                    evtLobbyCreated.Invoke(c);
                }

                callback?.Invoke(lobby, e);
            });
        }

        /// <summary>
        /// Creates an invisible type lobby and stores the result to the <see cref="groupLobby"/> property.
        /// </summary>
        /// <remarks>
        /// This will set 2 metadata fields on the lobby indicating the lobby type and mode
        /// </remarks>
        /// <param name="memberCountLimit">The number of users permited in this lobby.
        /// This can be changed by the host</param>
        /// <param name="callback">An optional callback to be invoked when the process is complete. { <see cref="Lobby"/> lobbyCreated, <see cref="bool"/> bIOFailure }</param>
        public static void CreateGroup(int memberCountLimit, Action<Lobby, bool> callback = null)
        {
            if(groupLobby != null)
            {
                Debug.LogWarning("Request to create a party lobby when one is already set. This will be ignored.");
                return;
            }

            var call = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeInvisible, memberCountLimit);
            m_LobbyCreated.Set(call, (c, e) =>
            {
                Debug.Log("Group Lobby Created");

                groupLobby = lobbies.FirstOrDefault(p => p.id.m_SteamID == c.m_ulSteamIDLobby);
                if (groupLobby == null)
                {
                    groupLobby = new Lobby(new CSteamID(c.m_ulSteamIDLobby));
                    groupLobby.evtExitLobby.AddListener(HandleExitLobby);
                    lobbies.Add(groupLobby);
                }

                groupLobby[Lobby.DataType] = "Invisible";
                groupLobby[Lobby.DataMode] = "Group";

                evtLobbyCreated.Invoke(c);

                callback?.Invoke(groupLobby, e);
            });
        }

        /// <summary>
        /// Creates a public lobby and stores the result to the <see cref="normalLobby"/> property
        /// </summary>
        /// <remarks>
        /// This will set 2 metadata fields on the lobby indicating the lobby type and mode
        /// </remarks>
        /// <param name="memberCountLimit">The number of users permited in this lobby.
        /// This can be changed by the host</param>
        /// <param name="callback">An optional callback to be invoked when the process is complete. { <see cref="Lobby"/> lobbyCreated, <see cref="bool"/> bIOFailure }</param>
        public static void CreatePublicLobby(int memberCountLimit, Action<Lobby, bool> callback = null)
        {
            if (normalLobby != null)
            {
                Debug.LogWarning("Attempting to create a normal typed lobby while already a member of a normal lobby. Valve is likely to reject this.\nUsers may only be a member of 1 normal lobby and 2 invisible lobbies at a time.");
            }

            var lobbyType = ELobbyType.k_ELobbyTypePublic;

            var call = SteamMatchmaking.CreateLobby(lobbyType, memberCountLimit);
            m_LobbyCreated.Set(call, (c, e) =>
            {
                var lobby = lobbies.FirstOrDefault(p => p.id.m_SteamID == c.m_ulSteamIDLobby);
                if (!e)
                {
                    if (lobby == null)
                    {
                        lobby = new Lobby(new CSteamID(c.m_ulSteamIDLobby));
                        lobby.evtExitLobby.AddListener(HandleExitLobby);
                        lobbies.Add(lobby);
                    }

                    lobby[Lobby.DataType] = "Public";
                    normalLobby = lobby;

                    lobby[Lobby.DataMode] = "General";

                    evtLobbyCreated.Invoke(c);
                }

                callback?.Invoke(lobby, e);
            });
        }

        /// <summary>
        /// Joins a steam lobby
        /// </summary>
        /// <param name="lobbyId">The ID of the lobby to join</param>
        /// <remarks>
        /// See <see href="https://partner.steamgames.com/doc/api/ISteamMatchmaking#JoinLobby">JoinLobby</see> in Valve's documentation for more details.
        /// </remarks>
        /// <param name="callback">An optional callback to be invoked when the process is complete. { <see cref="Lobby"/> lobbyCreated, <see cref="bool"/> bIOFailure }</param>
        public static void JoinLobby(CSteamID lobbyId, Action<Lobby, bool> callback = null)
        {
            var call = SteamMatchmaking.JoinLobby(lobbyId);
            m_LobbyEntered.Set(call, (c, e) =>
            {
                if (!e)
                {
                    if (c.m_EChatRoomEnterResponse == 1)
                    {
                        var lobby = lobbies.FirstOrDefault(p => p.id.m_SteamID == c.m_ulSteamIDLobby);
                        if (lobby == null)
                        {
                            lobby = new Lobby(new CSteamID(c.m_ulSteamIDLobby));
                            lobby.UpdateLobbyState();
                            lobby.evtExitLobby.AddListener(HandleExitLobby);
                            lobbies.Add(lobby);
                        }

                        evtLobbyEnter.Invoke(lobby);
                        callback?.Invoke(lobby, false);
                    }
                    else
                    {
                        evtLobbyJoinFailed.Invoke(lobbyId);
                        callback?.Invoke(null, true);
                    }
                }
                else
                    callback?.Invoke(null, true);
            });
        }

        /// <summary>
        /// Joins a steam lobby
        /// </summary>
        /// <param name="lobbyId">The ID of the lobby to join</param>
        /// <param name="callback">An optional callback to be invoked when the process is complete. { <see cref="Lobby"/> lobbyCreated, <see cref="bool"/> bIOFailure }</param>
        /// <remarks>
        /// See <see href="https://partner.steamgames.com/doc/api/ISteamMatchmaking#JoinLobby">JoinLobby</see> in Valve's documentation for more details.
        /// </remarks>
        public static void JoinLobby(ulong lobbyId, Action<Lobby, bool> callback = null)
        {
            JoinLobby(new CSteamID(lobbyId), callback);
        }

        /// <summary>
        /// Leaves the current lobby if any
        /// </summary>
        public static void LeaveAllLobbies()
        {
            var tempList = lobbies.ToArray();

            foreach (var lobby in tempList)
                lobby.Leave();

            lobbies.Clear();
            tempList = null;
        }

        /// <summary>
        /// Searches for a matching lobby according to the provided filter data.
        /// Note that a search will only start if no search is currently running.
        /// </summary>
        /// <param name="filter">Describes the metadata to search for in a lobby</param>
        /// <param name="callback">An optional callback to be invoked when the process is complete. { <see cref="LobbyQueryResultList"/> lobbyResults, <see cref="bool"/> bIOFailure }</param>
        public static void FindLobbies(LobbyQueryParameters filter, Action<LobbyQueryResultList, bool> callback = null)
        {
            if (!callbacksRegistered)
                Initalize();

            SetLobbyFilter(filter);

            var call = SteamMatchmaking.RequestLobbyList();
            m_LobbyMatchList.Set(call, (r,e) =>
            {
                uint numLobbies = r.m_nLobbiesMatching;
                var result = new LobbyQueryResultList();

                if (e)
                {
                    if (SteamSettings.current.isDebugging)
                        Debug.Log("Lobby match list failed, bIOFailure");

                    evtQuickMatchFailed.Invoke();
                }
                else
                {
                    if (SteamSettings.current.isDebugging)
                        Debug.Log("Lobby match list returned (" + numLobbies.ToString() + ")");

                    for (int i = 0; i < numLobbies; i++)
                    {
                        LobbyQueryResultRecord record = new LobbyQueryResultRecord();

                        record.metadata = new Dictionary<string, string>();
                        record.lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
                        record.maxSlots = SteamMatchmaking.GetLobbyMemberLimit(record.lobbyId);


                        int dataCount = SteamMatchmaking.GetLobbyDataCount(record.lobbyId);

                        var matchLobby = lobbies.FirstOrDefault(p => p.id == record.lobbyId);

                        if (matchLobby != null)
                        {
                            if (SteamSettings.current.isDebugging)
                                Debug.Log("Browsed our own lobby and found " + dataCount.ToString() + " metadata records.");
                        }

                        for (int ii = 0; ii < dataCount; ii++)
                        {
                            bool isUs = matchLobby != null;
                            string key;
                            string value;
                            if (SteamMatchmaking.GetLobbyDataByIndex(record.lobbyId, ii, out key, Constants.k_nMaxLobbyKeyLength, out value, Constants.k_cubChatMetadataMax))
                            {
                                record.metadata.Add(key, value);
                                if (key == "name")
                                    record.name = value;
                                if (key == "OwnerID")
                                {
                                    ulong val;
                                    if (ulong.TryParse(value, out val))
                                    {
                                        record.hostId = new CSteamID(val);
                                    }
                                }
                            }
                        }

                        result.Add(record);
                    }

                    evtLobbyMatchList.Invoke(result);
                }

                callback?.Invoke(result, e);
            });

            evtSearchStarted.Invoke();
        }

        [Obsolete("Please use " + nameof(FindLobbies) + " instead.")]
        public static void FindMatch(LobbyQueryParameters lobbyFilter, Action<LobbyQueryResultList, bool> callback = null) => FindLobbies(lobbyFilter, callback);

        [Obsolete("Please use " + nameof(FindLobbies) + " instead.")]
        public static void QuickMatch(LobbyQueryParameters lobbyFilter, Action<LobbyQueryResultList, bool> callback = null) => FindLobbies(lobbyFilter, callback);

        [Obsolete("No longer used")]
        public static void CancelQuickMatch()
        { }

        [Obsolete("No longer used")]
        public static void CancelStandardSearch()
        { }

        /// <summary>
        /// Sends a chat message via Valve's Lobby Chat system to the first lobby in the <see cref="lobbies"/> list
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <remarks>
        /// <para>
        /// This method exists here for support of older single lobby systems. It is recomended that you use the SendChatMessage on the specific lobby you want to send a message on or that you use the overload that takes the lobby ID.
        /// </para>
        /// </remarks>
        public static void SendChatMessage(string message)
        {
            if (lobbies.Count == 0)
                return;

            if (!callbacksRegistered)
                Initalize();

            byte[] MsgBody = System.Text.Encoding.UTF8.GetBytes(message);
            SteamMatchmaking.SendLobbyChatMsg(lobbies[0].id, MsgBody, MsgBody.Length);
        }

        /// <summary>
        /// Send a chat message to the indicated lobby
        /// </summary>
        /// <param name="lobbyId">The lobby to chat on</param>
        /// <param name="message">The message to be sent</param>
        public static void SendChatMessage(CSteamID lobbyId, string message)
        {
            if (lobbies.Count == 0)
                return;

            if (!callbacksRegistered)
                Initalize();

            byte[] MsgBody = System.Text.Encoding.UTF8.GetBytes(message);
            SteamMatchmaking.SendLobbyChatMsg(lobbyId, MsgBody, MsgBody.Length);
        }

        /// <summary>
        /// Sets metadata on the first lobby, this can only be called by the host of the lobby
        /// </summary>
        /// <param name="key">The key of the metadata to set</param>
        /// <param name="value">The value of the metadata to set</param>
        /// <remarks>
        /// <para>
        /// This is here to support older single lobby code, it is recomended that you set data directly on the <see cref="Lobby"/> object or use the overload to specify the lobby you want to target.
        /// </para>
        /// </remarks>
        public static void SetLobbyMetadata(string key, string value)
        {
            if (lobbies.Count == 0)
                return;

            if (!callbacksRegistered)
                Initalize();

            lobbies[0][key] = value;
        }

        /// <summary>
        /// Sets metadata on the first lobby, this can only be called by the host of the lobby
        /// </summary>
        /// <param name="key">The key of the metadata to set</param>
        /// <param name="value">The value of the metadata to set</param>
        public static void SetLobbyMetadata(CSteamID lobbyId, string key, string value)
        {
            if (!callbacksRegistered)
                Initalize();

            var lobby = lobbies.FirstOrDefault(p => p.id == lobbyId);
            if (lobby != null)
                lobby[key] = value;
        }

        /// <summary>
        /// Sets metadata for the player on the first lobby
        /// </summary>
        /// <param name="key">The key of the metadata to set</param>
        /// <param name="value">The value of the metadata to set</param>
        public static void SetMemberMetadata(string key, string value)
        {
            if (lobbies.Count == 0)
                return;

            if (!callbacksRegistered)
                Initalize();

            lobbies[0].User[key] = value;
        }

        /// <summary>
        /// Sets metadata for the player on the first lobby
        /// </summary>
        /// <param name="key">The key of the metadata to set</param>
        /// <param name="value">The value of the metadata to set</param>
        public static void SetMemberMetadata(CSteamID lobbyId, string key, string value)
        {
            if (!callbacksRegistered)
                Initalize();

            var lobby = lobbies.FirstOrDefault(p => p.id == lobbyId);
            if(lobby != null)
                lobby.User[key] = value;
        }

        /// <summary>
        /// Sets the lobby game server e.g. game start using the lobby Host as the server ID
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assumes you want to set the game server on the first lobby. It exists to support older code that used a single lobby system.
        /// It is recomended that you call <see cref="Lobby.SetGameServer"/> directly on the lobby you want or use the overload to indicate the lobby.
        /// </para>
        /// <para>
        /// This will trigger GameServerSet on all members of the lobby
        /// This should be called after the server is started
        /// </para>
        /// </remarks>
        public static void SetLobbyGameServer()
        {
            if (!callbacksRegistered)
                Initalize();

            lobbies[0].SetGameServer();
        }

        /// <summary>
        /// Sets the lobby game server e.g. game start using the lobby Host as the server ID
        /// </summary>
        /// <remarks>
        /// <para>
        /// This will trigger GameServerSet on all members of the lobby
        /// This should be called after the server is started
        /// </para>
        /// </remarks>
        public static void SetLobbyGameServer(CSteamID lobbyId)
        {
            if (!callbacksRegistered)
                Initalize();

            var lobby = lobbies.FirstOrDefault(p => p.id == lobbyId);
            if (lobby != null)
                lobby.SetGameServer();
        }

        /// <summary>
        /// Sets the lobby game server e.g. game start
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="serverId"></param>
        /// <remarks>
        /// <para>
        /// This assumes you want to set the game server on the first lobby. It exists to support older code that used a single lobby system.
        /// It is recomended that you call <see cref="Lobby.SetGameServer"/> directly on the lobby you want or use the overload to indicate the lobby.
        /// </para>
        /// <para>
        /// This will trigger GameServerSet on all members of the lobby
        /// This should be called after the server is started
        /// </para>
        /// </remarks>
        public static void SetLobbyGameServer(string ipAddress, ushort port, CSteamID serverId)
        {
            lobbies[0].SetGameServer(ipAddress, port, serverId);
        }

        /// <summary>
        /// Sets the lobby game server e.g. game start
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="serverId"></param>
        /// <remarks>
        /// <para>
        /// This will trigger GameServerSet on all members of the lobby
        /// This should be called after the server is started
        /// </para>
        /// </remarks>
        public static void SetLobbyGameServer(CSteamID lobbyId, string ipAddress, ushort port, CSteamID serverId)
        {
            var lobby = lobbies.FirstOrDefault(p => p.id == lobbyId);
            if (lobby != null)
                lobby.SetGameServer(ipAddress, port, serverId);
        }

        /// <summary>
        /// Sets the lobby as joinable or not. The default is that a lobby is joinable.
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// <para>
        /// This assumes you want to set the game server on the first lobby. It exists to support older code that used a single lobby system.
        /// It is recomended that you call <see cref="Lobby.Joinable"/> directly on the lobby you want or use the overload to indicate the lobby.
        /// </para>
        /// </remarks>
        public static void SetLobbyJoinable(bool value)
        {
            lobbies[0].Joinable = value;
        }

        /// <summary>
        /// Sets the lobby as joinable or not. The default is that a lobby is joinable.
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// <para>
        /// This assumes you want to set the game server on the first lobby. It exists to support older code that used a single lobby system.
        /// It is recomended that you call <see cref="Lobby.Joinable"/> directly on the lobby you want or use the overload to indicate the lobby.
        /// </para>
        /// </remarks>
        public static void SetLobbyJoinable(CSteamID lobbyId, bool value)
        {
            var lobby = lobbies.FirstOrDefault(p => p.id == lobbyId);
            if (lobby != null)
                lobby.Joinable = value;
        }

        /// <summary>
        /// Returns information about the lobbies game server
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// <para>
        /// This assumes you want to set the game server on the first lobby. It exists to support older code that used a single lobby system.
        /// It is recomended that you call <see cref="Lobby.GameServer"/> directly on the lobby you want or use the overload to indicate the lobby.
        /// </para>
        /// </remarks>
        public static LobbyGameServerInformation GetGameServer()
        {
            return lobbies[0].GameServer;
        }

        /// <summary>
        /// Returns information about the lobbies game server
        /// </summary>
        /// <param name="lobbyId"></param>
        /// <returns></returns>
        public static LobbyGameServerInformation GetGameServer(CSteamID lobbyId)
        {
            var lobby = lobbies.FirstOrDefault(p => p.id == lobbyId);
            if (lobby != null)
                return lobby.GameServer;
            else
                return default;
        }

        /// <summary>
        /// Marks the user to be removed
        /// </summary>
        /// <param name="memberId"></param>
        /// <remarks>
        /// <para>
        /// This assumes you want to set the game server on the first lobby. It exists to support older code that used a single lobby system.
        /// It is recomended that you call <see cref="Lobby.GameServer"/> directly on the lobby you want or use the overload to indicate the lobby.
        /// </para>
        /// This creates an entry in the metadata named z_heathenKick which contains a string array of Ids of users that should leave the lobby.
        /// When users detect their ID in the string they will automatically leave the lobby on leaving the lobby the users ID will be removed from the array.
        /// </remarks>
        public static void KickMember(CSteamID memberId)
        {
            lobbies[0].KickMember(memberId);
        }

        /// <summary>
        /// Marks the user to be removed
        /// </summary>
        /// <param name="memberId"></param>
        /// <remarks>
        /// This creates an entry in the metadata named z_heathenKick which contains a string array of Ids of users that should leave the lobby.
        /// When users detect their ID in the string they will automatically leave the lobby on leaving the lobby the users ID will be removed from the array.
        /// </remarks>
        public static void KickMember(CSteamID lobbyId, CSteamID memberId)
        {
            var lobby = lobbies.FirstOrDefault(p => p.id == lobbyId);
            if (lobby != null)
                lobby.KickMember(memberId);
        }

        /// <summary>
        /// Sets the indicated user as the new owner of the lobby
        /// </summary>
        /// <param name="newOwner"></param>
        /// <remarks>
        /// <para>
        /// This assumes you want to set the game server on the first lobby. It exists to support older code that used a single lobby system.
        /// It is recomended that you call <see cref="Lobby.OwnerId"/> directly on the lobby you want or use the overload to indicate the lobby.
        /// </para>
        /// <para>
        /// This does not effect the NetworkManager or other networking funcitonality it only changes the ownership of a lobby
        /// </para>
        /// </remarks>
        public static void ChangeOwner(CSteamID newOwner)
        {
            lobbies[0].OwnerId = newOwner;
        }

        /// <summary>
        /// Sets the indicated user as the new owner of the lobby
        /// </summary>
        /// <param name="newOwner"></param>
        /// <remarks>
        /// <para>
        /// This does not effect the NetworkManager or other networking funcitonality it only changes the ownership of a lobby
        /// </para>
        /// </remarks>
        public static void ChangeOwner(CSteamID lobbyId, CSteamID newOwner)
        {
            var lobby = lobbies.FirstOrDefault(p => p.id == lobbyId);
            if (lobby != null)
                lobby.OwnerId = newOwner;
        }
    }
}
#endif