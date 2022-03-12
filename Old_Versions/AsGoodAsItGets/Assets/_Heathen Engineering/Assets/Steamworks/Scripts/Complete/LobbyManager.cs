#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace HeathenEngineering.SteamworksIntegration
{
    /// <summary>
    /// Helps you find or create a lobby.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is meant to be attached to your lobby UI, party UI or similar and manages 1 single lobby.
    /// It can be used to search for a matching lobby and automatically join it, 
    /// to create a lobby or to browse for lobby that match its <see cref="searchArguments"/>.
    /// </para>
    /// <para>
    /// When you create or join a lobby using this tool it will link that joined or created lobby and provide access to that lobbies events. methods and members.
    /// You can then use this object as an interface between your UI and a specific lobby to create lobby windows, party windows, lobby chats and more.
    /// </para>
    /// </remarks>
    public class LobbyManager : MonoBehaviour
    {
        [Obsolete("Use evtEnter instead", true)]
        [NonSerialized]
        [HideInInspector]
        public LobbyDataEvent evtJoin;
        [Obsolete("Use evtEnterSuccess or evtEnterFailed instead")]
        public LobbyEnterEvent evtEnter => API.Matchmaking.Client.EventLobbyEnter;

        [Serializable]
        public class GameServerSetEvent : UnityEvent<LobbyGameServer>
        { }

        [HideInInspector]
        public ulong lobbyId = CSteamID.Nil.m_SteamID;
        public SearchArguments searchArguments = new SearchArguments();
        public CreateArguments createArguments = new CreateArguments();

        [Header("Events")]
        public LobbyDataListEvent evtFound;
        /// <summary>
        /// Occurs when the local user enters a lobby as a responce to a join
        /// </summary>
        public LobbyDataEvent evtEnterSuccess;
        public LobbyResponceEvent evtEnterFailed;
        public LobbyDataEvent evtCreated;
        public UnityEvent evtCreateFailed;
        public UnityEvent evtQuickMatchFailed;
        public LobbyDataUpdateEvent evtDataUpdated;
        /// <summary>
        /// Occurs when the local user leaves the managed lobby
        /// </summary>
        public UnityEvent evtLeave;
        public UnityEvent evtAskedToLeave;
        public GameServerSetEvent evtGameCreated;
        /// <summary>
        /// Occurs when the local user is a member of a lobby and a new member joins that lobby
        /// </summary>
        public UserDataEvent evtUserJoined;
        /// <summary>
        /// Occurts when the local user is a member of a lobby and another fellow member leveas the lobby
        /// </summary>
        public UserLeaveEvent evtUserLeft;

        public Lobby Lobby
        {
            get => lobbyId;
            set => lobbyId = value;
        }
        public bool HasLobby => lobbyId != CSteamID.Nil.m_SteamID && SteamMatchmaking.GetNumLobbyMembers(new CSteamID(lobbyId)) > 0;
        public bool IsPlayerOwner => Lobby.IsOwner;
        public bool AllPlayersReady => Lobby.AllPlayersReady;
        public bool IsPlayerReady
        {
            get => API.Matchmaking.Client.GetLobbyMemberData(Lobby, API.User.Client.Id, Lobby.DataReady) == "true";
            set => API.Matchmaking.Client.SetLobbyMemberData(Lobby, Lobby.DataReady, value.ToString().ToLower());
        }
        public bool Full => Lobby.Full;
        public bool IsTypeSet => Lobby.IsTypeSet;
        public ELobbyType Type
        {
            get => Lobby.Type;
            set
            {
                var l = Lobby;
                l.Type = value;
            }
        }
        public int MaxMembers
        {
            get => API.Matchmaking.Client.GetLobbyMemberLimit(new CSteamID(lobbyId));
            set => API.Matchmaking.Client.SetLobbyMemberLimit(new CSteamID(lobbyId), value);
        }
        public bool HasServer => SteamMatchmaking.GetLobbyGameServer(new CSteamID(lobbyId), out _, out _, out _);
        public LobbyGameServer GameServer => API.Matchmaking.Client.GetLobbyGameServer(new CSteamID(lobbyId));

        private void OnEnable()
        {
            API.Matchmaking.Client.EventLobbyAskedToLeave.AddListener(HandleAskedToLeave);
            API.Matchmaking.Client.EventLobbyDataUpdate.AddListener(HandleLobbyDataUpdate);
            API.Matchmaking.Client.EventLobbyLeave.AddListener(HandleLobbyLeave);
            API.Matchmaking.Client.EventLobbyGameCreated.AddListener(HandleGameServerSet);
            API.Matchmaking.Client.EventLobbyChatUpdate.AddListener(HandleChatUpdate);
        }

        private void OnDisable()
        {
            API.Matchmaking.Client.EventLobbyAskedToLeave.RemoveListener(HandleAskedToLeave);
            API.Matchmaking.Client.EventLobbyDataUpdate.RemoveListener(HandleLobbyDataUpdate);
            API.Matchmaking.Client.EventLobbyLeave.RemoveListener(HandleLobbyLeave);
            API.Matchmaking.Client.EventLobbyGameCreated.RemoveListener(HandleGameServerSet);
            API.Matchmaking.Client.EventLobbyChatUpdate.RemoveListener(HandleChatUpdate);
        }

        private void HandleChatUpdate(LobbyChatUpdate_t arg0)
        {
            if(arg0.m_ulSteamIDLobby == lobbyId)
            {
                var state = (EChatMemberStateChange)arg0.m_rgfChatMemberStateChange;
                if (state == EChatMemberStateChange.k_EChatMemberStateChangeEntered)
                    evtUserJoined?.Invoke(arg0.m_ulSteamIDUserChanged);
                else
                    evtUserLeft?.Invoke(new UserLobbyLeaveData { user = arg0.m_ulSteamIDUserChanged, state = state });
            }
        }

        private void HandleGameServerSet(LobbyGameCreated_t arg0)
        {
            if (arg0.m_ulSteamIDLobby == lobbyId)
                evtGameCreated.Invoke(GameServer);
        }

        private void HandleLobbyLeave(Lobby arg0)
        {
            if (arg0 == lobbyId)
                evtLeave.Invoke();
        }

        private void HandleAskedToLeave(Lobby arg0)
        {
            if (arg0 == lobbyId)
                evtAskedToLeave.Invoke();
        }

        private void HandleLobbyDataUpdate(LobbyDataUpdate_t arg0)
        {
            if (arg0.m_ulSteamIDLobby == lobbyId)
                evtDataUpdated.Invoke(arg0);
        }

        /// <summary>
        /// Changes the type of the current lobby if any
        /// </summary>
        /// <remarks>
        /// This will also update the type in the <see cref="createArguments"/> record
        /// </remarks>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool SetType(ELobbyType type)
        {
            createArguments.type = type;
            return API.Matchmaking.Client.SetLobbyType(new CSteamID(lobbyId), type);
        }
        /// <summary>
        /// Sets the lobby joinable or not
        /// </summary>
        /// <param name="makeJoinable"></param>
        /// <returns></returns>
        public bool SetJoinable(bool makeJoinable) => API.Matchmaking.Client.SetLobbyJoinable(new CSteamID(lobbyId), makeJoinable);
        /// <summary>
        /// Searches for a match based on <see cref="searchArguments"/>, if none is found it will create a lobby matching the <see cref="createArguments"/>
        /// </summary>
        public void QuickMatch(bool createOnFail = true)
        {
            API.Matchmaking.Client.AddRequestLobbyListDistanceFilter(searchArguments.distance);

            if (searchArguments.slots > 0)
                API.Matchmaking.Client.AddRequestLobbyListFilterSlotsAvailable(searchArguments.slots);

            foreach (var near in searchArguments.nearValues)
                API.Matchmaking.Client.AddRequestLobbyListNearValueFilter(near.key, near.value);

            foreach (var numeric in searchArguments.numericFilters)
                API.Matchmaking.Client.AddRequestLobbyListNumericalFilter(numeric.key, numeric.value, numeric.comparison);

            foreach (var text in searchArguments.stringFilters)
                API.Matchmaking.Client.AddRequestLobbyListStringFilter(text.key, text.value, text.comparison);

            API.Matchmaking.Client.AddRequestLobbyListResultCountFilter(1);

            API.Matchmaking.Client.RequestLobbyList((r, e) =>
            {
                if (!e && r.Length >= 1)
                {
                    API.Matchmaking.Client.JoinLobby(r[0], (r2, e2) =>
                    {
                        var responce = (EChatRoomEnterResponse)r2.m_EChatRoomEnterResponse;

                        if (!e2 && responce == EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
                        {
                            if (SteamSettings.current.isDebugging)
                                Debug.Log("Quick match found, joined lobby: " + r2.m_ulSteamIDLobby.ToString());

                            lobbyId = r2.m_ulSteamIDLobby;
                            evtFound?.Invoke(r);
                            evtEnterSuccess.Invoke(r[0]);
                        }
                        else
                        {
                            if (responce == EChatRoomEnterResponse.k_EChatRoomEnterResponseLimited)
                            {
                                Debug.LogError("This user is limited and cannot create or join lobbies or chats.");
                                evtEnterFailed.Invoke(responce);
                            }
                            else
                            {
                                if (createOnFail)
                                {
                                    if (SteamSettings.current.isDebugging)
                                        Debug.LogError("Quick match failed, lobbies found but failed to join ... creating lobby.");

                                    Create();
                                }
                                else
                                    evtQuickMatchFailed.Invoke();
                            }
                        }
                    });
                }
                else
                {
                    if (createOnFail)
                    {
                        if (SteamSettings.current.isDebugging)
                            Debug.Log("Quick match failed, no lobbies found ... creating lobby.");

                        Create();
                    }
                    else
                        evtQuickMatchFailed.Invoke();
                }
            });
        }
        /// <summary>
        /// Creates a new lobby with the data found in <see cref="createArguments"/>
        /// </summary>
        public void Create()
        {
            API.Matchmaking.Client.CreateLobby(createArguments.type, createArguments.slots, (r, e) =>
            {
                if (!e)
                {
                    if (SteamSettings.current.isDebugging)
                        Debug.Log("New lobby created.");

                    lobbyId = r.id.m_SteamID;

                    var lobby = Lobby;
                    lobby[Lobby.DataName] = createArguments.name;
                    foreach (var data in createArguments.metadata)
                        lobby[data.key] = data.value;

                    evtCreated?.Invoke(lobby);
                }
                else
                {
                    Debug.LogError("Lobby creation failed with message: IOFailure\nSteam API responded with a general IO Failure.");
                    evtCreateFailed?.Invoke();
                }
            });
        }
        /// <summary>
        /// Searches for lobbies that match the <see cref="searchArguments"/>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Remimber Lobbies are a matchmaking feature, the first lobby returned is generally he best, lobby search is not intended to return all possible results simply the best matching options.
        /// </para>
        /// </remarks>
        /// <param name="maxResults">The maximum number of lobbies to return. lower values are better.</param>
        public void Search(int maxResults)
        {
            if (maxResults <= 0)
                return;

            API.Matchmaking.Client.AddRequestLobbyListDistanceFilter(searchArguments.distance);

            if (searchArguments.slots > 0)
                API.Matchmaking.Client.AddRequestLobbyListFilterSlotsAvailable(searchArguments.slots);

            foreach (var near in searchArguments.nearValues)
                API.Matchmaking.Client.AddRequestLobbyListNearValueFilter(near.key, near.value);

            foreach (var numeric in searchArguments.numericFilters)
                API.Matchmaking.Client.AddRequestLobbyListNumericalFilter(numeric.key, numeric.value, numeric.comparison);

            foreach (var text in searchArguments.stringFilters)
                API.Matchmaking.Client.AddRequestLobbyListStringFilter(text.key, text.value, text.comparison);

            API.Matchmaking.Client.AddRequestLobbyListResultCountFilter(maxResults);

            API.Matchmaking.Client.RequestLobbyList((r, e) =>
            {
                if (!e)
                {
                    evtFound?.Invoke(r);
                }
                else
                {
                    evtFound?.Invoke(new Lobby[0]);
                }
            });
        }
        /// <summary>
        /// Joins the indicated steam lobby
        /// </summary>
        /// <param name="lobby"></param>
        public void Join(Lobby lobby)
        {
            API.Matchmaking.Client.JoinLobby(lobby, (r, e) =>
            {
                if(!e)
                {
                    if (r.m_EChatRoomEnterResponse == (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
                    {
                        if (SteamSettings.current.isDebugging)
                            Debug.Log("Joined lobby: " + lobby.ToString());

                        lobbyId = r.m_ulSteamIDLobby;
                        evtEnterSuccess.Invoke(lobby);
                    }
                    else
                        evtEnterFailed.Invoke((EChatRoomEnterResponse)r.m_EChatRoomEnterResponse);
                }
                else
                    evtEnterFailed.Invoke(EChatRoomEnterResponse.k_EChatRoomEnterResponseError);
            });
        }

        public void Join(ulong lobby)
        {
            API.Matchmaking.Client.JoinLobby(lobby, (r, e) =>
            {
                if (!e)
                {
                    if (r.m_EChatRoomEnterResponse == (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
                    {
                        if (SteamSettings.current.isDebugging)
                            Debug.Log("Joined lobby: " + lobby.ToString());

                        lobbyId = r.m_ulSteamIDLobby;
                        evtEnterSuccess.Invoke(lobby);
                    }
                    else
                        evtEnterFailed.Invoke((EChatRoomEnterResponse)r.m_EChatRoomEnterResponse);
                }
                else
                    evtEnterFailed.Invoke(EChatRoomEnterResponse.k_EChatRoomEnterResponseError);
            });
        }
        public void Join(string lobbyIdAsString)
        {
            if (ulong.TryParse(lobbyIdAsString, out ulong result))
                Join(result);
        }
        public bool SetLobbyData(string key, string value) => API.Matchmaking.Client.SetLobbyData(new CSteamID(lobbyId), key, value);
        public void SetLobbyMemberData(string key, string value) => API.Matchmaking.Client.SetLobbyMemberData(new CSteamID(lobbyId), key, value);
        public LobbyMember GetLobbyMember(CSteamID member) => new LobbyMember { lobby = Lobby, user = member };
        public string GetMemberData(CSteamID member, string key) => API.Matchmaking.Client.GetLobbyMemberData(Lobby, member, key);
        public bool IsMemberReady(CSteamID member) => API.Matchmaking.Client.GetLobbyMemberData(Lobby, member, Lobby.DataReady) == "true";
        public void KickMember(CSteamID member) => Lobby.KickMember(member);
        public bool Invite(UserData user) => API.Matchmaking.Client.InviteUserToLobby(new CSteamID(lobbyId), user);
        public LobbyMember[] Members => API.Matchmaking.Client.GetLobbyMembers(new CSteamID(lobbyId));

        [Serializable]
        public struct NumericFilter
        {
            public string key;
            public int value;
            public ELobbyComparison comparison;
        }
        [Serializable]
        public struct NearFilter
        {
            public string key;
            public int value;
        }
        [Serializable]
        public struct StringFilter
        {
            public string key;
            public string value;
            public ELobbyComparison comparison;
        }
        [Serializable]
        public struct MetadataTempalate
        {
            public string key;
            public string value;
        }
        [Serializable]
        public class SearchArguments
        {
            /// <summary>
            /// If less than or equal to 0 then we wont use the open slot filter
            /// </summary>
            public int slots = -1;
            public ELobbyDistanceFilter distance = ELobbyDistanceFilter.k_ELobbyDistanceFilterDefault;
            public List<NearFilter> nearValues = new List<NearFilter>();
            public List<NumericFilter> numericFilters = new List<NumericFilter>();
            public List<StringFilter> stringFilters = new List<StringFilter>();
        }
        [Serializable]
        public class CreateArguments
        {
            public string name;
            public int slots;
            public ELobbyType type;
            public List<MetadataTempalate> metadata = new List<MetadataTempalate>();
        }
    }
}
#endif