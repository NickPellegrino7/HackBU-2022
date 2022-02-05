#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE && !UNITY_SERVER
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace HeathenEngineering.SteamAPI
{
    /// <summary>
    /// Represents a Lobby in memory
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="MatchmakingTools.lobbies"/> is a collection of this object type and represents the lobbies the system is currently tracking.
    /// Typically a lobby would only be in this list if the user had joined it, and commonly there would be only 1 lobby. 
    /// The most common situation where a user would be in multiple lobbies is where a user is in a private lobby functioning as a "party" and searching for and joining a public lobby representing a pending game session.
    /// </para>
    /// <para>
    /// Lobbies are primarly used for matchmaking, that is players will search for a lobby that matches there desired game paramiters.
    /// These paramiters are stored on the lobby as 'metadata' which is simply a string to string key value pair of data.
    /// Lobby metadata can only be set by the lobby owner but can be read by anyone rather or not they are a member of the lobby.
    /// To set lobby metadata use the indexer such as
    /// </para>
    /// <code>
    /// steamLobby["data field name"] = "data field value";
    /// </code>
    /// <para>
    /// Note that this will add a new entry with key "data field name" if none exists or will update an existing one if present.
    /// Metadata can also be used to display rich information to a lobby browser. 
    /// For example you can store the game mode and map type currently selected by the members of the lobby as metadata.
    /// This data is available to users who query the lobby and can be displayed to them or used as a filter argument
    /// </para>
    /// </remarks>
    [Serializable]
    public class Lobby
    {
        /// <summary>
        /// Standard metadata field representing the name of the lobby.
        /// This field is typically only used in lobby metadata
        /// </summary>
        public const string DataName = "name";
        /// <summary>
        /// Heathen standard metadata field representing the version of the game.
        /// This field is commonly used in lobby and member metadata
        /// </summary>
        public const string DataVersion = "z_heathenGameVersion";
        /// <summary>
        /// Heathen standard metadata field indicating that the user is ready to play.
        /// This field is commonly only used on member metadata
        /// </summary>
        public const string DataReady = "z_heathenReady";
        /// <summary>
        /// Heathen standard metadata field indicating that these users should leave the lobby.
        /// This is a string containing each CSteamID of members that should not join this lobby and if present should leave it.
        /// Data in this list is in the form of [ + CSteamID + ] e.g. [123456789][987654321] would indicate 2 users that should leave
        /// This field is commonly only used on lobby metadata
        /// </summary>
        public const string DataKick = "z_heathenKick";
        /// <summary>
        /// Heathen standard metadata field indicating the mode of the lobby e.g. group or general
        /// If this is blank its assumed to be general
        /// </summary>
        public const string DataMode = "z_heathenMode";
        /// <summary>
        /// Heathen standard metadata field indicating the type of lobby e.g. private, friend, public or invisible
        /// </summary>
        public const string DataType = "z_heathenType";

        /// <summary>
        /// Loads the lobby and its related data
        /// </summary>
        /// <param name="lobbyId">The lobby to load data for</param>
        /// <remarks>
        /// <see cref="Lobby"/> should only be initalized for a lobby that the user is a member of e.g. on create or join of a lobby.
        /// Constructing a <see cref="Lobby"/> for a lobby you are not a member of will cause some data to be missing due to security on Valve's side.
        /// </remarks>
        public Lobby(CSteamID lobbyId)
        {
            id = lobbyId;

            var memberCount = SteamMatchmaking.GetNumLobbyMembers(id);
            for (int i = 0; i < memberCount; i++)
            {
                var memberId = SteamMatchmaking.GetLobbyMemberByIndex(id, i);
                members.Add(new LobbyMember(id, memberId));
            }

            previousOwner = Owner;
        }

        /// <summary>
        /// The id of the lobby as reported by Steamworks.
        /// </summary>
        public CSteamID id;

        private LobbyMember previousOwner = null;
        /// <summary>
        /// The current owner of the lobby.
        /// </summary>
        /// <remarks>
        /// This looks up the owner from members list and repairs the members if required (adds missing member data if needed).
        /// When setting this value it will call <see cref="SteamMatchmaking.SetLobbyOwner(CSteamID, CSteamID)"/>
        /// </remarks>
        public LobbyMember Owner
        {
            get
            {
                var ownerId = SteamMatchmaking.GetLobbyOwner(id);
                var result = members.FirstOrDefault(p => p.userData != null && p.userData.id == ownerId);
                if (result == null)
                {
                    result = new LobbyMember(id, ownerId);
                    members.Add(result);
                }

                return result;
            }
            set
            {
                if (value.lobbyId == id && value.userData != null)
                    SteamMatchmaking.SetLobbyOwner(id, value.userData.id);
            }
        }

        /// <summary>
        /// The id of the owner of the lobby
        /// </summary>
        /// <remarks>
        /// <para>
        /// This looks up the owner from the members list and repairs the members if required (adds missing member data if needed).
        /// When setting this value it will call <see cref="SteamMatchmaking.SetLobbyOwner(CSteamID, CSteamID)"/>
        /// </para>
        /// </remarks>
        public CSteamID OwnerId
        {
            get => Owner.userData.id;
            set
            {
                if (members.Any(p => p.userData != null && p.userData.id == value))
                    SteamMatchmaking.SetLobbyOwner(id, value);
            }
        }

        /// <summary>
        /// The member data for this user
        /// </summary>
        /// <remarks>
        /// This looks up the users record in the members list and repairs the list if the entry is missing.
        /// This does confirm that the user is a member of this lobby, if the use is not a member of the lobby it will return null.
        /// </remarks>
        public LobbyMember User
        {
            get
            {
                var userId = SteamUser.GetSteamID();
                if (id == CSteamID.Nil)
                    return null;

                var result = members.FirstOrDefault(p => p.userData != null && p.userData.id == userId);
                if(result != null)
                {
                    return result;
                }
                else
                {
                    var lobbyMemberCount = SteamMatchmaking.GetNumLobbyMembers(id);
                    for (int i = 0; i < lobbyMemberCount; i++)
                    {
                        var memberId = SteamMatchmaking.GetLobbyMemberByIndex(id, i);
                        if (memberId == userId)
                        {
                            result = new LobbyMember(id, userId);
                            members.Add(result);
                            return result;
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// The collection of all members of this lobby including the owner of the lobby.
        /// </summary>
        /// <remarks>
        /// This should never be set from code, it is updated via Steamworks callbacks and contains the <see cref="UserData"/> and metadata for each member.
        /// </remarks>
        public readonly List<LobbyMember> members = new List<LobbyMember>();

        #region Events
        [Obsolete("Use evtDataUpdateFailed instead")]
        public UnityEvent LobbyDataUpdateFailed => evtDataUpdateFailed;
        [FormerlySerializedAs("LobbyDataUpdateFailed")]
        [HideInInspector]
        public UnityEvent evtDataUpdateFailed = new UnityEvent();
        [Obsolete("Use evtExitLobby instead")]
        public UnityLobbyEvent OnExitLobby => evtExitLobby;
        [FormerlySerializedAs("OnExitLobby")]
        [HideInInspector]
        public UnityLobbyEvent evtExitLobby = new UnityLobbyEvent();
        [Obsolete("Use evtKickedFromLobby instead")]
        public SteamIDEvent OnKickedFromLobby => evtKickedFromLobby;
        [FormerlySerializedAs("OnKickedFromLobby")]
        [HideInInspector]
        public SteamIDEvent evtKickedFromLobby = new SteamIDEvent();
        [Obsolete("Use the new name instead: evtOwnershipChange")]
        [HideInInspector]
        public LobbyMemberEvent OnOwnershipChange => evtOwnershipChange;
        /// <summary>
        /// Occures when the owner of the currently tracked lobby changes
        /// </summary>
        [HideInInspector]
        public LobbyMemberEvent evtOwnershipChange = new LobbyMemberEvent();
        [Obsolete("Use the new name instead: evtMemberJoined")]
        [HideInInspector]
        public LobbyMemberEvent OnMemberJoined => evtMemberJoined;
        /// <summary>
        /// Occures when a member joins the lobby
        /// </summary>
        [HideInInspector]
        public LobbyMemberEvent evtMemberJoined = new LobbyMemberEvent();
        [Obsolete("Use the new name instead: evtMemberLeft")]
        [HideInInspector]
        public LobbyMemberEvent OnMemberLeft => evtMemberLeft;
        /// <summary>
        /// Occures when a member leaves the lobby
        /// </summary>
        [HideInInspector]
        public LobbyMemberEvent evtMemberLeft = new LobbyMemberEvent();
        [Obsolete("Use the new name instead: evtMemberDataChanged")]
        [HideInInspector]
        public LobbyMemberEvent OnMemberDataChanged => evtMemberDataChanged;
        /// <summary>
        /// Occures when Steamworks metadata for a member changes
        /// </summary>
        [HideInInspector]
        public LobbyMemberEvent evtMemberDataChanged = new LobbyMemberEvent();
        [Obsolete("Use the new name instead: evtLobbyDataChanged")]
        [HideInInspector]
        public UnityEvent OnLobbyDataChanged = new UnityEvent();
        /// <summary>
        /// Occures when lobby metadata changes
        /// </summary>
        [HideInInspector]
        public UnityEvent evtLobbyDataChanged = new UnityEvent();
        [Obsolete("Use the new name instead: evtGameServerSet")]
        [HideInInspector]
        public UnityLobbyGameCreatedEvent OnGameServerSet => evtGameServerSet;
        /// <summary>
        /// Occures when the host of the lobby starts the game e.g. sets game server data on the lobby
        /// </summary>
        [HideInInspector]
        public UnityLobbyGameCreatedEvent evtGameServerSet = new UnityLobbyGameCreatedEvent();
        [Obsolete("Use the new name instead: evtLobbyChatUpdate")]
        [HideInInspector]
        public UnityLobbyChatUpdateEvent OnLobbyChatUpdate => evtLobbyChatUpdate;
        /// <summary>
        /// Occures when lobby chat metadata has been updated such as a kick or ban.
        /// </summary>
        [HideInInspector]
        public UnityLobbyChatUpdateEvent evtLobbyChatUpdate = new UnityLobbyChatUpdateEvent();
        [Obsolete("Use the new name instead: evtChatMessageReceived")]
        [HideInInspector]
        public LobbyChatMessageEvent OnChatMessageReceived => evtChatMessageReceived;
        /// <summary>
        /// Occures when a lobby chat message is recieved
        /// </summary>
        [HideInInspector]
        public LobbyChatMessageEvent evtChatMessageReceived = new LobbyChatMessageEvent();
        [Obsolete("Use the new name instead: evtChatMemberEntered")]
        [HideInInspector]
        public LobbyMemberEvent ChatMemberStateChangeEntered => evtChatMemberEntered;
        /// <summary>
        /// Occures when a member of the lobby chat enters the chat
        /// </summary>
        [HideInInspector]
        public LobbyMemberEvent evtChatMemberEntered = new LobbyMemberEvent();
        [Obsolete("Use the new name instead: evtChatMemberLeft")]
        [HideInInspector]
        public UnityPersonaEvent ChatMemberStateChangeLeft => evtChatMemberLeft;
        /// <summary>
        /// Occures when a member of the lobby chat leaves the chat
        /// </summary>
        [HideInInspector]
        public UnityPersonaEvent evtChatMemberLeft = new UnityPersonaEvent();
        [Obsolete("Use the new name instead: evtChatMemberDisconnected")]
        [HideInInspector]
        public UnityPersonaEvent ChatMemberStateChangeDisconnected = new UnityPersonaEvent();
        /// <summary>
        /// Occures when a member of the lobby chat is disconnected from the chat
        /// </summary>
        [HideInInspector]
        public UnityPersonaEvent evtChatMemberDisconnected = new UnityPersonaEvent();
        [Obsolete("Use the new name instead: evtChatMemberKicked")]
        [HideInInspector]
        public UnityPersonaEvent ChatMemberStateChangeKicked => evtChatMemberKicked;
        /// <summary>
        /// Occures when a member of the lobby chat is kicked out of the chat
        /// </summary>
        [HideInInspector]
        public UnityPersonaEvent evtChatMemberKicked = new UnityPersonaEvent();
        [Obsolete("Use the new name instead: evtChatMemberBanned")]
        [HideInInspector]
        public UnityPersonaEvent ChatMemberStateChangeBanned = new UnityPersonaEvent();
        /// <summary>
        /// Occures when a member of the lobby chat is banned from the chat
        /// </summary>
        [HideInInspector]
        public UnityPersonaEvent evtChatMemberBanned = new UnityPersonaEvent();
        #endregion

        /// <summary>
        /// Get or set the lobby name
        /// </summary>
        /// <remarks>
        /// <para>
        /// The lobby name is a metadata field whoes key is "name". Setting this field will update the lobby metadata accordinly and this update will be reflected to all members.
        /// Only the owner of the lobby can set this value.
        /// </para>
        /// </remarks>
        public string Name 
        { 
            get => this[DataName]; 
            set => this[DataName] = value; 
        }

        /// <summary>
        /// Gets or sets the version of the game the lobby is configured for ... this should match the owners version
        /// </summary>
        public string GameVersion
        {
            get => this[DataVersion];
            set => this[DataVersion] = value;
        }

        /// <summary>
        /// Gets or sets the type of lobby this is
        /// </summary>
        /// <remarks>
        /// This will update the <see cref="MatchmakingTools.groupLobby"/> and <see cref="MatchmakingTools.normalLobby"/> properties as required.
        /// Note that a party lobby must be invisiible e.g. setting a party lobby to any other type will remove it as the party lobby.
        /// Note that a normal lobby is any not invisible lobby e.g. setting a normal lobby invisible removes it as the normal lobby.
        /// </remarks>
        public ELobbyType Type
        {
            get
            {
                switch(this[DataType])
                {
                    case "Public":
                    case "public":
                        return ELobbyType.k_ELobbyTypePublic;
                    case "Private":
                    case "private":
                        return ELobbyType.k_ELobbyTypePrivate;
                    case "Friend":
                    case "friend":
                        return ELobbyType.k_ELobbyTypeFriendsOnly;
                    case "Invisible":
                    case "invisible":
                        return ELobbyType.k_ELobbyTypeInvisible;
                    case "Unique":
                    case "unique":
                        return ELobbyType.k_ELobbyTypePrivateUnique;
                    default:
                        Debug.LogWarning("Query lobby type but this field has not been set so the value is assumed to be Public");
                        return ELobbyType.k_ELobbyTypePublic;
                }
            }
            set
            {
                if (IsHost)
                {
                    switch (value)
                    {
                        case ELobbyType.k_ELobbyTypePublic:
                            this[DataType] = "Public";
                            SteamMatchmaking.SetLobbyType(id, ELobbyType.k_ELobbyTypePublic);
                            if (MatchmakingTools.groupLobby == this)
                            {
                                MatchmakingTools.groupLobby = null;
                                MatchmakingTools.normalLobby = this;
                            }
                            break;
                        case ELobbyType.k_ELobbyTypePrivate:
                            this[DataType] = "Private";
                            SteamMatchmaking.SetLobbyType(id, ELobbyType.k_ELobbyTypePrivate);
                            if (MatchmakingTools.groupLobby == this)
                            {
                                MatchmakingTools.groupLobby = null;
                                MatchmakingTools.normalLobby = this;
                            }
                            break;
                        case ELobbyType.k_ELobbyTypeInvisible:
                            this[DataType] = "Invisible";
                            SteamMatchmaking.SetLobbyType(id, ELobbyType.k_ELobbyTypeInvisible);
                            if (MatchmakingTools.normalLobby == this)
                            {
                                MatchmakingTools.normalLobby = null;
                            }
                            break;
                        case ELobbyType.k_ELobbyTypeFriendsOnly:
                            this[DataType] = "Friend";
                            SteamMatchmaking.SetLobbyType(id, ELobbyType.k_ELobbyTypeFriendsOnly);
                            if (MatchmakingTools.groupLobby == this)
                            {
                                MatchmakingTools.groupLobby = null;
                                MatchmakingTools.normalLobby = this;
                            }
                            break;
                    }
                }
                else
                    Debug.LogWarning("Only the host of a lobby can set its type");
            }
        }

        /// <summary>
        /// Indicates rather or not this lobby is a party lobby
        /// </summary>
        /// <remarks>
        /// If the <see cref="MatchmakingTools.groupLobby"/> lobby is set IsParty = false it will be removed from the <see cref="MatchmakingTools.groupLobby"/> property.
        /// If the <see cref="MatchmakingTools.normalLobby"/> is set IsParty = true it will be removed from the <see cref="MatchmakingTools.normalLobby"/> property.
        /// If a lobby is set IsParty = true and the <see cref="MatchmakingTools.groupLobby"/> lobby is null it will be assigned to this lobby.
        /// </remarks>
        public bool IsGroup
        {
            get
            {
                return this[DataMode] == "Group";
            }
            set
            {
                if (IsHost)
                {
                    if(value)
                    {
                        Type = ELobbyType.k_ELobbyTypeInvisible;
                        
                        if (MatchmakingTools.groupLobby == null)
                            MatchmakingTools.groupLobby = this;

                        this[DataMode] = "Group";
                    }
                    else
                    {
                        if (MatchmakingTools.groupLobby == this)
                            MatchmakingTools.groupLobby = null;

                        this[DataMode] = "General";
                    }
                }
                else
                    Debug.LogWarning("Only the host of a lobby can set its mode");
            }
        }

        /// <summary>
        /// The current limit for member count
        /// </summary>
        public int MemberCountLimit
        {
            get => SteamMatchmaking.GetLobbyMemberLimit(id);
            set => SteamMatchmaking.SetLobbyMemberLimit(id, value);
        }

        /// <summary>
        /// Returns the number of users that are members of this lobby
        /// </summary>
        public int MemberCount => members.Count;

        private bool p_joinable = true;
        /// <summary>
        /// <para>
        /// Sets whether or not a lobby is joinable by other players. This always defaults to enabled for a new lobby.
        /// If joining is disabled, then no players can join, even if they are a friend or have been invited.
        /// Lobbies with joining disabled will not be returned from a lobby search.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is only accruate on the lobby owner's system and may not be accurate if the owner has changed since it was set.
        /// In general its advisable that you have a new owner set Joinable to the desired value when they are made owner doing so will cause the value to sync for that user.
        /// </para>
        /// </remarks>
        public bool Joinable
        {
            get => p_joinable;
            set
            {
                if (SteamMatchmaking.SetLobbyJoinable(id, value))
                    p_joinable = value;
            }
        }

        /// <summary>
        /// The game server information stored against the lobby
        /// </summary>
        /// <remarks>
        /// <para>
        /// This data is set when the host calls <see cref="SetLobbyGameServer"/> or one of its variants. Uppon calling <see cref="SetLobbyGameServer"/> the Valve backend will raise <see cref="evtGameServerSet"/> for all members other than the host the paramiter of which also contains server data.
        /// The typical use case of this field is when a member has join a persistent lobby after the game server has been started.
        /// </para>
        /// </remarks>
        public LobbyGameServerInformation GameServer { get; private set; }

        /// <summary>
        /// Is the user the host of this lobby
        /// </summary>
        /// <remarks>
        /// <para>
        /// Calls <see cref="SteamMatchmaking.GetLobbyOwner(CSteamID)"/> and compares the results to <see cref="SteamUser.GetSteamID()"/>.
        /// This returns true if the provided lobby ID is a legitimate ID and if Valve indicates that the lobby has members and if the owner of the lobby is the current player.
        /// </para>
        /// </remarks>
        public bool IsHost
        {
            get
            {
                return SteamUser.GetSteamID() == SteamMatchmaking.GetLobbyOwner(id);
            }
        }

        /// <summary>
        /// Does this lobby have a game server registered to it
        /// </summary>
        /// <remarks>
        /// <para>
        /// Calls <see cref="SteamMatchmaking.GetLobbyGameServer(CSteamID, out uint, out ushort, out CSteamID)"/> and cashes the data to <see cref="GameServer"/>.
        /// It is not usually nessisary to check this value since the set game server callback from Steamworks will automatically update these values if the user was connected to the lobby when the set game server data was called.
        /// </para>
        /// </remarks>
        public bool HasGameServer
        {
            get
            {
                uint ipBuffer;
                ushort portBuffer;
                CSteamID steamIdBuffer;
                var result = SteamMatchmaking.GetLobbyGameServer(id, out ipBuffer, out portBuffer, out steamIdBuffer);

                GameServer = new LobbyGameServerInformation()
                {
                    ipAddress = ipBuffer,
                    port = portBuffer,
                    serverId = steamIdBuffer
                };

                return result;
            }
        }

        /// <summary>
        /// Read and write metadata values to the lobby
        /// </summary>
        /// <param name="metadataKey">The key of the value to be read or writen</param>
        /// <returns>The value of the key if any otherwise returns and empty string.</returns>
        public string this[string metadataKey]
        {
            get
            {
                return SteamMatchmaking.GetLobbyData(id, metadataKey);
            }
            set
            {
                SteamMatchmaking.SetLobbyData(id, metadataKey, value);
            }
        }

        /// <summary>
        /// Returns the number of metadata keys set on the lobby
        /// </summary>
        /// <returns></returns>
        public int GetMetadataCount()
        {
            return SteamMatchmaking.GetLobbyDataCount(id);
        }

        /// <summary>
        /// Gets the dictionary of metadata values assigned to this lobby.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetMetadataEntries()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            var count = SteamMatchmaking.GetLobbyDataCount(id);

            for (int i = 0; i < count; i++)
            {
                string key;
                string value;
                SteamMatchmaking.GetLobbyDataByIndex(id, i, out key, Constants.k_nMaxLobbyKeyLength, out value, Constants.k_cubChatMetadataMax);
                result.Add(key, value);
            }

            return result;
        }

        /// <summary>
        /// Returns true if all of the players 'IsReady' is true
        /// </summary>
        /// <remarks>
        /// <para>
        /// This can be used to determin if the players are ready to play the game.
        /// </para>
        /// </remarks>
        public bool AllPlayersReady
        {
            get
            {
                //If we have any that are not ready then return false ... else return true
                return members.Any( p => !p.IsReady) ? false : true;
            }
        }

        /// <summary>
        /// Returns true if all of the players 'IsReady' is false
        /// </summary>
        /// <remarks>
        /// <para>
        /// This can be used to determin if all players have reset the ready flag such as when some change is made after a previous ready check had already passed.
        /// </para>
        /// </remarks>
        public bool AllPlayersNotReady
        {
            get
            {
                //If we have any that are not ready then return false ... else return true
                return members.Any(p => p.IsReady) ? false : true;
            }
        }

        #region Callback Handlers
        internal void HandleLobbyGameCreated(LobbyGameCreated_t param)
        {
            GameServer = new LobbyGameServerInformation
            {
                ipAddress = param.m_unIP,
                port = param.m_usPort,
                serverId = new CSteamID(param.m_ulSteamIDGameServer)
            };

            evtGameServerSet.Invoke(param);
        }

        internal void HandleLobbyChatUpdate(LobbyChatUpdate_t pCallback)
        {
            if (id.m_SteamID != pCallback.m_ulSteamIDLobby)
                return;

            if (pCallback.m_rgfChatMemberStateChange == (uint)EChatMemberStateChange.k_EChatMemberStateChangeLeft)
            {
                var memberId = new CSteamID(pCallback.m_ulSteamIDUserChanged);
                var member = members.FirstOrDefault(p => p.userData != null && p.userData.id == memberId);

                if (member != null)
                {
                    members.Remove(member);
                    evtMemberLeft.Invoke(member);
                    evtChatMemberLeft.Invoke(member.userData);
                }
            }
            else if (pCallback.m_rgfChatMemberStateChange == (uint)EChatMemberStateChange.k_EChatMemberStateChangeEntered)
            {
                var memberId = new CSteamID(pCallback.m_ulSteamIDUserChanged);
                var member = members.FirstOrDefault(p => p.userData != null && p.userData.id == memberId);

                if (member == null)
                {
                    member = new LobbyMember(id, memberId);
                    members.Add(member);
                    evtMemberJoined.Invoke(member);
                }

                evtChatMemberEntered.Invoke(member);
            }
            else if (pCallback.m_rgfChatMemberStateChange == (uint)EChatMemberStateChange.k_EChatMemberStateChangeDisconnected)
            {
                var memberId = new CSteamID(pCallback.m_ulSteamIDUserChanged);
                var member = members.FirstOrDefault(p => p.userData != null && p.userData.id == memberId);

                if (member != null)
                {
                    members.Remove(member);
                    evtMemberLeft.Invoke(member);
                    evtChatMemberDisconnected.Invoke(member.userData);
                }
            }
            else if (pCallback.m_rgfChatMemberStateChange == (uint)EChatMemberStateChange.k_EChatMemberStateChangeKicked)
            {
                var memberId = new CSteamID(pCallback.m_ulSteamIDUserChanged);
                var member = members.FirstOrDefault(p => p.userData != null && p.userData.id == memberId);

                if (member != null)
                {
                    members.Remove(member);
                    evtMemberLeft.Invoke(member);
                    evtChatMemberKicked.Invoke(member.userData);
                }
            }
            else if (pCallback.m_rgfChatMemberStateChange == (uint)EChatMemberStateChange.k_EChatMemberStateChangeBanned)
            {
                var memberId = new CSteamID(pCallback.m_ulSteamIDUserChanged);
                var member = members.FirstOrDefault(p => p.userData != null && p.userData.id == memberId);

                if (member != null)
                {
                    members.Remove(member);
                    evtMemberLeft.Invoke(member);
                    evtChatMemberBanned.Invoke(member.userData);
                }
            }

            evtLobbyChatUpdate.Invoke(pCallback);

            var currentOwner = Owner;
            if(previousOwner != currentOwner)
            {
                previousOwner = currentOwner;
                evtOwnershipChange.Invoke(currentOwner);
            }
        }

        internal void HandleLobbyDataUpdate(LobbyDataUpdate_t param)
        {
            var askedToLeave = false;

            if (param.m_bSuccess == 0)
            {
                evtDataUpdateFailed.Invoke();
                return;
            }

            if (param.m_ulSteamIDLobby == param.m_ulSteamIDMember)
            {
                if(SteamMatchmaking.GetLobbyData(id, DataKick).Contains("[" + SteamUser.GetSteamID().m_SteamID.ToString() + "]"))
                {
                    Debug.Log("User has been kicked from the lobby.");
                    askedToLeave = true;
                }
                UpdateLobbyState();
                evtLobbyDataChanged.Invoke();
            }
            else
            {
                var userId = new CSteamID(param.m_ulSteamIDMember);

                var member = members.FirstOrDefault(p => p.userData != null && p.userData.id == userId);
                if (member == null)
                {
                    member = new LobbyMember(id, userId);
                    members.Add(member);
                    evtMemberJoined.Invoke(member);
                }

                member.HandleDataUpdate(param);
                evtMemberDataChanged.Invoke(member);
            }

            if (askedToLeave)
            {
                var id = this.id;
                Leave();
                evtKickedFromLobby.Invoke(id);
            }
            else
            {
                var currentOwner = Owner;
                if (previousOwner != currentOwner)
                {
                    previousOwner = currentOwner;
                    evtOwnershipChange.Invoke(currentOwner);
                }
            }
        }

        internal LobbyChatMessageData HandleLobbyChatMessage(LobbyChatMsg_t pCallback)
        {
            var subjectLobby = (CSteamID)pCallback.m_ulSteamIDLobby;
            if (subjectLobby != id)
                return null;

            CSteamID SteamIDUser;
            byte[] Data = new byte[4096];
            EChatEntryType ChatEntryType;
            int ret = SteamMatchmaking.GetLobbyChatEntry(subjectLobby, (int)pCallback.m_iChatID, out SteamIDUser, Data, Data.Length, out ChatEntryType);
            byte[] truncated = new byte[ret];
            Array.Copy(Data, truncated, ret);

            LobbyChatMessageData record = new LobbyChatMessageData
            {
                sender = members.FirstOrDefault(p => p.userData.id == SteamIDUser),
                message = System.Text.Encoding.UTF8.GetString(truncated),
                recievedTime = DateTime.Now,
                chatEntryType = ChatEntryType,
                lobby = this,
            };

            evtChatMessageReceived.Invoke(record);

            return record;
        }
        #endregion

        /// <summary>
        /// Updates the static lobby references based on the type of this lobby
        /// </summary>
        /// <remarks>
        /// Valve only allows 1 normal e.g. non-invisible lobby.
        /// So if this lobby is invisible it cant be <see cref="MatchmakingTools.normalLobby"/> and should be <see cref="MatchmakingTools.groupLobby"/> unless some other lobby is already set there
        /// If this lobby is not invisible it must be <see cref="MatchmakingTools.normalLobby"/> and cant be <see cref="MatchmakingTools.groupLobby"/>
        /// </remarks>
        public void UpdateLobbyState()
        {
            if(!string.IsNullOrEmpty(this[DataType]))
            {
                switch(Type)
                {
                    case ELobbyType.k_ELobbyTypeInvisible:
                        if (MatchmakingTools.normalLobby == this)
                            MatchmakingTools.normalLobby = null;
                        if (MatchmakingTools.groupLobby == null)
                            MatchmakingTools.groupLobby = this;
                        break;
                    default:
                        if (MatchmakingTools.normalLobby != this)
                            MatchmakingTools.normalLobby = this;
                        if (MatchmakingTools.groupLobby == this)
                            MatchmakingTools.groupLobby = null;
                        break;
                }
            }
        }

        /// <summary>
        /// Leaves the current lobby if any
        /// </summary>
        public void Leave()
        {
            if (id == CSteamID.Nil)
                return;

            try
            {
                SteamMatchmaking.LeaveLobby(id);
            }
            catch { }

            evtExitLobby.Invoke(this);

            id = CSteamID.Nil;
            members.Clear();
        }

        public bool DeleteLobbyData(string dataKey)
        {
            return SteamMatchmaking.DeleteLobbyData(id, dataKey);
        }

        public bool InviteUserToLobby(CSteamID targetUser)
        {
            return SteamMatchmaking.InviteUserToLobby(id, targetUser);
        }

        public bool SendChatMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return false;

            byte[] MsgBody = System.Text.Encoding.UTF8.GetBytes(message);
            return SteamMatchmaking.SendLobbyChatMsg(id, MsgBody, MsgBody.Length);
        }

        /// <summary>
        /// <para>
        /// Sets the game server associated with the lobby.
        /// This can only be set by the owner of the lobby.
        /// Either the IP/Port or the Steamworks ID of the game server must be valid, depending on how you want the clients to be able to connect.
        /// A LobbyGameCreated_t callback will be sent to all players in the lobby, usually at this point, the users will join the specified game server.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// See <see href="https://partner.steamgames.com/doc/api/ISteamMatchmaking#SetLobbyGameServer">SetLobbyGameServer</see> in Valve's documentaiton.
        /// This will update the game server settings for the lobby notifying all members of the new data.
        /// This should only be called when you are ready for the members to join the server.
        /// </para>
        /// </remarks>
        /// <param name="address">The IP address of the game server as typicall string address "127.0.0.1"</param>
        /// <param name="port">The port of the game server</param>
        /// <param name="gameServerId">The steam ID of the game server, if this is P2P then this would be the host's CSteamID</param>
        public void SetGameServer(string address, ushort port, CSteamID gameServerId)
        {
            GameServer = new LobbyGameServerInformation
            {
                port = port,
                StringAddress = address,
                serverId = gameServerId
            };

            SteamMatchmaking.SetLobbyGameServer(id, GameServer.ipAddress, port, gameServerId);
        }

        /// <summary>
        /// <para>
        /// Sets the game server associated with the lobby.
        /// This can only be set by the owner of the lobby.
        /// Either the IP/Port or the Steamworks ID of the game server must be valid, depending on how you want the clients to be able to connect.
        /// A LobbyGameCreated_t callback will be sent to all players in the lobby, usually at this point, the users will join the specified game server.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// See <see href="https://partner.steamgames.com/doc/api/ISteamMatchmaking#SetLobbyGameServer">SetLobbyGameServer</see> in Valve's documentaiton.
        /// This will update the game server settings for the lobby notifying all members of the new data.
        /// This should only be called when you are ready for the members to join the server.
        /// </para>
        /// </remarks>
        /// <param name="address">The IP address of the game server as typicall string address "127.0.0.1"</param>
        /// <param name="port">The port of the game server</param>
        public void SetGameServer(string address, ushort port)
        {
            GameServer = new LobbyGameServerInformation
            {
                port = port,
                StringAddress = address,
                serverId = CSteamID.Nil
            };

            SteamMatchmaking.SetLobbyGameServer(id, GameServer.ipAddress, port, CSteamID.Nil);
        }

        /// <summary>
        /// <para>
        /// Sets the game server associated with the lobby.
        /// This can only be set by the owner of the lobby.
        /// Either the IP/Port or the Steamworks ID of the game server must be valid, depending on how you want the clients to be able to connect.
        /// A LobbyGameCreated_t callback will be sent to all players in the lobby, usually at this point, the users will join the specified game server.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// See <see href="https://partner.steamgames.com/doc/api/ISteamMatchmaking#SetLobbyGameServer">SetLobbyGameServer</see> in Valve's documentaiton.
        /// This will update the game server settings for the lobby notifying all members of the new data.
        /// This should only be called when you are ready for the members to join the server.
        /// </para>
        /// </remarks>
        /// <param name="gameServerId">The steam ID of the game server, if this is P2P then this would be the host's CSteamID</param>
        public void SetGameServer(CSteamID gameServerId)
        {
            GameServer = new LobbyGameServerInformation
            {
                port = 0,
                ipAddress = 0,
                serverId = gameServerId
            };

            SteamMatchmaking.SetLobbyGameServer(id, 0, 0, gameServerId);
        }

        /// <summary>
        /// <para>
        /// This overload uses the lobby owner's CSteamID as the server ID which is typical of P2P session.
        /// </para>
        /// <para>
        /// Sets the game server associated with the lobby.
        /// This can only be set by the owner of the lobby.
        /// Either the IP/Port or the Steamworks ID of the game server must be valid, depending on how you want the clients to be able to connect.
        /// A LobbyGameCreated_t callback will be sent to all players in the lobby, usually at this point, the users will join the specified game server.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// See <see href="https://partner.steamgames.com/doc/api/ISteamMatchmaking#SetLobbyGameServer">SetLobbyGameServer</see> in Valve's documentaiton.
        /// This will update the game server settings for the lobby notifying all members of the new data.
        /// This should only be called when you are ready for the members to join the server.
        /// </para>
        /// </remarks>
        public void SetGameServer()
        {
            GameServer = new LobbyGameServerInformation
            {
                port = 0,
                ipAddress = 0,
                serverId = SteamUser.GetSteamID()
            };

            SteamMatchmaking.SetLobbyGameServer(id, 0, 0, GameServer.serverId);
        }

        /// <summary>
        /// <para>
        /// Updates what type of lobby this is.
        /// This is also set when you create the lobby with CreateLobby.
        /// This can only be set by the owner of the lobby.
        /// </para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <remarks>
        /// <para>
        /// See <see href="https://partner.steamgames.com/doc/api/ISteamMatchmaking#SetLobbyType">SetLobbyType</see> in Valve's documentaiton.
        /// </para>
        /// </remarks>
        public bool SetLobbyType(ELobbyType type)
        {
            if (IsHost)
            {
                Type = type;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Marks the user to be removed
        /// </summary>
        /// <param name="memberId"></param>
        /// <remarks>
        /// This creates an entry in the metadata named z_heathenKick which contains a string array of Ids of users that should leave the lobby.
        /// When users detect their ID in the string they will automatically leave the lobby on leaving the lobby the users ID will be removed from the array.
        /// </remarks>
        public void KickMember(CSteamID memberId)
        {
            if (!IsHost)
            {
                Debug.LogError("Only the host of a lobby can kick a member from it.");
                return;
            }

            if (memberId.m_SteamID == SteamUser.GetSteamID().m_SteamID)
            {
                Leave();
                evtKickedFromLobby.Invoke(SteamUser.GetSteamID());
                return;
            }
            else
            {
                Debug.Log("Marking " + memberId.m_SteamID + " for removal");
            }

            var kickList = SteamMatchmaking.GetLobbyData(id, DataKick);

            if (kickList == null)
                kickList = string.Empty;

            if (!kickList.Contains("[" + memberId.ToString() + "]"))
                kickList += "[" + memberId.ToString() + "]";

            SteamMatchmaking.SetLobbyData(id, DataKick, kickList);
        }

        /// <summary>
        /// Sets metadata for the player on the first lobby
        /// </summary>
        /// <param name="key">The key of the metadata to set</param>
        /// <param name="value">The value of the metadata to set</param>
        public void SetMemberMetadata(string key, string value)
        {
            SteamMatchmaking.SetLobbyMemberData(id, key, value);
        }

        /// <summary>
        /// Returns the metadata field of the local user
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetMemberMetadata(string key)
        {
            return SteamMatchmaking.GetLobbyMemberData(id, SteamUser.GetSteamID(), key);
        }

        /// <summary>
        /// Returns the metadata field of the user indicated by <paramref name="memberId"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetMemberMetadata(CSteamID memberId, string key)
        {
            return SteamMatchmaking.GetLobbyMemberData(id, memberId, key);
        }

        /// <summary>
        /// Returns the metadata field of the user indicated by <paramref name="member"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetMemberMetadata(LobbyMember member, string key)
        {
            return SteamMatchmaking.GetLobbyMemberData(id, member.userData.id, key);
        }
    }

}
#endif