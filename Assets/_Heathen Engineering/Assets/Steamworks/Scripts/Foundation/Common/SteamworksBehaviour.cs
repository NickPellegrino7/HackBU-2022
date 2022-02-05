#if !DISABLESTEAMWORKS
using HeathenEngineering.Events;
using Steamworks;
using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace HeathenEngineering.SteamAPI
{
    /// <summary>
    /// <para>This replaces the SteamManager concept from classic Steamworks.NET</para>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="SteamworksBehaviour"/> uses <see cref="SteamSettings"/> to initalize the Steam API and handles callbacks for the system.
    /// For the convenance of users using a singleton model <see cref="SteamSettings.current"/> can be used to access the active settings object.
    /// <see cref="SteamSettings.Client"/> is a static member containing all client scoped objects.
    /// <see cref="SteamSettings.Server"/> is a static member containing all server scoped objects.
    /// </para>
    /// <para>
    /// The <see cref="SteamworksBehaviour"/> is the core compoenent to the Heathen Steamworks kit and replaces the funcitonality present in Steamworks.NET's SteamManager.
    /// The primary funciton of the manager is to operate the update loop required by the Steamworks API and to handle and direct callbacks from the Steamworks API.</para>
    /// <para>It is strongly advised that you never unload or reload the <see cref="SteamworksBehaviour"/> for example you should not place the <see cref="SteamworksBehaviour"/> in your title scene because that scene will be unloaded and reloaded multiple times.
    /// Even if you mark the object as Do Not Destroy on Load, on reload of the title or similar scene Unity will create a second <see cref="SteamworksBehaviour"/> creating issues with the memory it manages.</para>
    /// <para>The recomended approch is place your <see cref="SteamworksBehaviour"/> and any other "manager" in a bootstrap scene that loads first and is never reloaded through the life of the game.
    /// This will help insure that through the life of your users play session 1 and exsactly 1 <see cref="SteamworksBehaviour"/> is created and never destroyed.
    /// While there are other approches you could take to insure this using a simple bootstrap scene is typically the simplest. For more information on this subject see <a href="https://heathen-engineering.mn.co/posts/scenes-management-quick-start"/>.
    /// This article referes to another tool available from Heathen Engineering however the concepts within apply rather or not your using that tool.</para>
    /// </remarks>
    [HelpURL("")]
    [DisallowMultipleComponent]
    public class SteamworksBehaviour : MonoBehaviour
    {
        #region Editor Exposed Values
        /// <summary>
        /// Reference to the <see cref="SteamworksClientApiSettings"/> object containing the configuration to be used for intialization of the Steamworks API
        /// </summary>
        public SteamSettings settings;
        /// <summary>
        /// An event raised when the Steamworks API has been intialzied
        /// </summary>
        public UnityEvent evtSteamInitalized;
        /// <summary>
        /// An event raised when an error has occred while intializing the Steamworks API
        /// </summary>
        public UnityStringEvent evtSteamInitalizationError;

#if !UNITY_SERVER
#if HE_STEAMPLAYERSERVICES
        public UnityLeaderboardRankChangeEvent evtLeaderboardRankChanged;
        public UnityLeaderboardRankUpdateEvent evtLeaderboardRankLoaded;
        public UnityLeaderboardRankChangeEvent evtLeaderboardNewHighRank;
        /// <summary>
        /// Occures when a file read async operation is completed.
        /// </summary>
        public UnityEvent evtFileReadAsyncComplete;
        /// <summary>
        /// Occures when a file write operation is completed.
        /// </summary>
        public UnityEvent evtFileWriteAsyncComplete;
        /// <summary>
        /// Occures in responce to a request to share a remote storage file. The resulting paramiter contains the file id which can be used in detail and share requests.
        /// </summary>
        public UnityFileShareResultEvent evtFileShareResult;
#endif
#if HE_STEAMCOMPLETE
        public UnityEvent evtItemInstancesUpdated;
        public UnityItemDetailEvent evtItemsGranted;
        public UnityItemDetailEvent evtItemsConsumed;
        public UnityItemDetailEvent evtItemsExchanged;
        [FormerlySerializedAs("evtItemsDroped")]
        public UnityItemDetailEvent evtItemsDropped;

        /// <summary>
        /// Wrapper around <see cref="MatchmakingTools"/>
        /// </summary>
        /// <remarks>
        /// Occures when a request to join the lobby has been recieved such as through Steamworks's invite friend dialog in the Steamworks Overlay
        /// </remarks>
        public UnityGameLobbyJoinRequestedEvent evtGameLobbyJoinRequest = new UnityGameLobbyJoinRequestedEvent();
        /// <summary>
        /// Wrapper around <see cref="MatchmakingTools"/>
        /// </summary>
        /// <remarks>
        /// Occures when list of Lobbies is retured from a search
        /// </remarks>
        public UnityLobbyQueryResultListEvent evtLobbyMatchList = new UnityLobbyQueryResultListEvent();
        /// <summary>
        /// Wrapper around <see cref="MatchmakingTools"/>
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
        public UnityLobbyCreatedEvent evtLobbyCreated = new UnityLobbyCreatedEvent();
        /// <summary>
        /// Wrapper around <see cref="MatchmakingTools"/>
        /// </summary>
        /// <remarks>
        /// Occures when the player joins a lobby
        /// </remarks>
        public UnityLobbyEvent evtLobbyEnter = new UnityLobbyEvent();
        /// <summary>
        /// Wrapper around <see cref="MatchmakingTools"/>
        /// </summary>
        /// <remarks>
        /// Occures when the player leaves a lobby
        /// </remarks>
        public UnityLobbyEvent evtLobbyExit = new UnityLobbyEvent();
        /// <summary>
        /// Wrapper around <see cref="MatchmakingTools"/>
        /// </summary>
        /// <remarks>
        /// Occures when the host of the lobby starts the game e.g. sets game server data on the lobby
        /// </remarks>
        public UnityLobbyGameCreatedEvent evtGameServerSet = new UnityLobbyGameCreatedEvent();
        /// <summary>
        /// Wrapper around <see cref="MatchmakingTools"/>
        /// </summary>
        /// <remarks>
        /// Occures when lobby chat metadata has been updated such as a kick or ban.
        /// </remarks>
        public UnityLobbyChatUpdateEvent evtLobbyChatUpdate = new UnityLobbyChatUpdateEvent();
        /// <summary>
        /// Wrapper around <see cref="MatchmakingTools"/>
        /// </summary>
        /// <remarks>
        /// Occures when a quick match search fails to return a lobby match
        /// </remarks>
        public UnityEvent evtQuickMatchFailed = new UnityEvent();
        /// <summary>
        /// Wrapper around <see cref="MatchmakingTools"/>
        /// </summary>
        /// <remarks>
        /// Occures when a search for a lobby has started
        /// </remarks>
        public UnityEvent evtSearchStarted = new UnityEvent();
        /// <summary>
        /// Wrapper around <see cref="MatchmakingTools"/>
        /// </summary>
        /// <remarks>
        /// Occures when a lobby chat message is recieved
        /// </remarks>
        public LobbyChatMessageEvent evtOnChatMessageReceived = new LobbyChatMessageEvent();
#endif
        /// <summary>
        /// An event raised when the overlay is opened by the user.
        /// This is only avilable in client builds.
        /// </summary>
        public UnityBoolEvent evtOverlayActivated;
        /// <summary>
        /// An event raised when user stats are updated.
        /// This is only avilable in client builds.
        /// </summary>
        public UnityUserStatsReceivedEvent evtUserStatsRecieved;
        /// <summary>
        /// An event raised when user stats are stored to the server.
        /// This is only avilable in client builds.
        /// </summary>
        public UnityUserStatsStoredEvent evtUserStatsStored;
        /// <summary>
        /// An event raised when number of current players of this game has been updated in the local cashe.
        /// This is only avilable in client builds.
        /// </summary>
        public UnityNumberOfCurrentPlayersResultEvent evtNumberOfCurrentPlayersResult;
        /// <summary>
        /// An event raised when achievements have been stored to the server.
        /// This is only avilable in client builds.
        /// </summary>
        public UnityUserAchievementStoredEvent evtAchievementStored;
        /// <summary>
        /// An event raised when a user avatar has been loaded e.g. the image represening a user.
        /// This is only avilable in client builds.
        /// </summary>
        public UnityAvatarImageLoadedEvent evtAvatarLoaded;
        /// <summary>
        /// An event raised when information about a Steamworks User's persona state has been updated.
        /// This is only avilable in client builds.
        /// </summary>
        public UnityPersonaStateChangeEvent evtPersonaStateChanged;
        /// <summary>
        /// An event raised when when a chat message from a friend has been recieved.
        /// This is only avilable in client builds.
        /// </summary>
        public FriendChatMessageEvent evtRecievedFriendChatMessage;
#endif
#if !CONDITIONAL_COMPILE || UNITY_SERVER || UNITY_EDITOR
        /// <summary>
        /// An event raised when the Steamworks Game Server shut down has been called.
        /// This is only avilable in server builds.
        /// </summary>
        [FormerlySerializedAs("GameServerShuttingDown")]
        public UnityEvent evtGameServerShuttingDown;
        /// <summary>
        /// An event raised when by Steamworks debugging on disconnected.
        /// This is only avilable in server builds.
        /// </summary>
        [FormerlySerializedAs("Disconnected")]
        public SteamSettings.GameServer.DisconnectedEvent evtDisconnected;
        /// <summary>
        /// An event raised by Steamworks debugging on connected.
        /// This is only avilable in server builds.
        /// </summary>
        [FormerlySerializedAs("Connected")]
        public SteamSettings.GameServer.ConnectedEvent evtConnected;
        /// <summary>
        /// An event raised by Steamworks debugging on failure.
        /// This is only avilable in server builds.
        /// </summary>
        [FormerlySerializedAs("Failure")]
        public SteamSettings.GameServer.FailureEvent evtFailure;

        public SteamSettings.GameServer.UnloadedStatsEvent evtStatusUnloaded;
#endif
        #endregion
                
        private ENotificationPosition currentNotificationPosition = ENotificationPosition.k_EPositionBottomRight;
        private Vector2Int currentNotificationIndent = Vector2Int.zero;

        private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;
        private static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText)
        {
            Debug.LogWarning(pchDebugText);
        }

        // This should only ever get called on first load and after an Assembly reload, You should never Disable the Steamworks Manager yourself.
        private void OnEnable()
        {
            settings.evtSteamInitalized.AddListener(evtSteamInitalized.Invoke);
            settings.evtSteamInitalizationError.AddListener(evtSteamInitalizationError.Invoke);

#if !UNITY_SERVER
            if (settings.isDebugging)
                Debug.Log("Client Startup Detected!");
            settings.Init();

            if (m_SteamAPIWarningMessageHook == null)
            {
                // Set up our callback to recieve warning messages from Steamworks.
                // You must launch with "-debug_steamapi" in the launch args to recieve warnings.
                m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
                SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
            }

            //Register the overlay callbacks
            settings.client.m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(settings.client.HandleOnOverlayOpen);
            settings.client.evtOverlayActivated.AddListener(evtOverlayActivated.Invoke);

            //Register the achievements system
            settings.client.evtAchievementStored.AddListener(evtAchievementStored.Invoke);
            settings.client.evtUserStatsReceived.AddListener(evtUserStatsRecieved.Invoke);
            settings.client.evtUserStatsStored.AddListener(evtUserStatsStored.Invoke);
            settings.client.evtNumberOfCurrentPlayersResult.AddListener(evtNumberOfCurrentPlayersResult.Invoke);
            
            //Register the friends system
            settings.client.evtAvatarLoaded.AddListener(evtAvatarLoaded.Invoke);
            settings.client.evtPersonaStateChanged.AddListener(evtPersonaStateChanged.Invoke);
            settings.client.evtRecievedFriendChatMessage.AddListener(evtRecievedFriendChatMessage.Invoke);

#if HE_STEAMPLAYERSERVICES
            settings.client.evtLeaderboardRankChanged.AddListener(evtLeaderboardRankChanged.Invoke);
            settings.client.evtLeaderboardRankLoaded.AddListener(evtLeaderboardRankLoaded.Invoke);
            settings.client.evtLeaderboardNewHighRank.AddListener(evtLeaderboardNewHighRank.Invoke);
            settings.client.remoteStorage.evtFileReadAsyncComplete.AddListener(evtFileReadAsyncComplete.Invoke);
            settings.client.remoteStorage.evtFileShareResult.AddListener(evtFileShareResult.Invoke);
            settings.client.remoteStorage.evtFileWriteAsyncComplete.AddListener(evtFileWriteAsyncComplete.Invoke);
#endif
#if HE_STEAMCOMPLETE
            settings.client.inventory.evtItemInstancesUpdated.AddListener(evtItemInstancesUpdated.Invoke);
            settings.client.inventory.evtItemsGranted.AddListener(evtItemsGranted.Invoke);
            settings.client.inventory.evtItemsConsumed.AddListener(evtItemsConsumed.Invoke);
            settings.client.inventory.evtItemsExchanged.AddListener(evtItemsExchanged.Invoke);
            settings.client.inventory.evtItemsDropped.AddListener(evtItemsDropped.Invoke);

            MatchmakingTools.evtGameLobbyJoinRequest.AddListener(evtGameLobbyJoinRequest.Invoke);
            MatchmakingTools.evtLobbyMatchList.AddListener(evtLobbyMatchList.Invoke);
            MatchmakingTools.evtLobbyCreated.AddListener(evtLobbyCreated.Invoke);
            MatchmakingTools.evtLobbyEnter.AddListener(evtLobbyEnter.Invoke);
            MatchmakingTools.evtLobbyExit.AddListener(evtLobbyExit.Invoke);
            MatchmakingTools.evtGameServerSet.AddListener(evtGameServerSet.Invoke);
            MatchmakingTools.evtLobbyChatUpdate.AddListener(evtLobbyChatUpdate.Invoke);
            MatchmakingTools.evtQuickMatchFailed.AddListener(evtQuickMatchFailed.Invoke);
            MatchmakingTools.evtSearchStarted.AddListener(evtSearchStarted.Invoke);
            MatchmakingTools.evtChatMessageReceived.AddListener(evtOnChatMessageReceived.Invoke);
#endif
#endif
#if UNITY_SERVER //|| UNITY_EDITOR
            if (settings.isDebugging)
                Debug.Log("Game Server Startup Detected!");
            if (settings.server.autoInitalize)
                InitializeGameServer();
#endif
        }

        private void OnDisable()
        {
#if !UNITY_SERVER
            settings.client.evtOverlayActivated.RemoveListener(evtOverlayActivated.Invoke);

            //Register the achievements system
            settings.client.evtAchievementStored.RemoveListener(evtAchievementStored.Invoke);
            settings.client.evtUserStatsReceived.RemoveListener(evtUserStatsRecieved.Invoke);
            settings.client.evtUserStatsStored.RemoveListener(evtUserStatsStored.Invoke);
            settings.client.evtNumberOfCurrentPlayersResult.RemoveListener(evtNumberOfCurrentPlayersResult.Invoke);

            //Register the friends system
            settings.client.evtAvatarLoaded.RemoveListener(evtAvatarLoaded.Invoke);
            settings.client.evtPersonaStateChanged.RemoveListener(evtPersonaStateChanged.Invoke);
            settings.client.evtRecievedFriendChatMessage.RemoveListener(evtRecievedFriendChatMessage.Invoke);

#if HE_STEAMPLAYERSERVICES
            settings.client.evtLeaderboardRankChanged.RemoveListener(evtLeaderboardRankChanged.Invoke);
            settings.client.evtLeaderboardRankLoaded.RemoveListener(evtLeaderboardRankLoaded.Invoke);
            settings.client.evtLeaderboardNewHighRank.RemoveListener(evtLeaderboardNewHighRank.Invoke);
            settings.client.remoteStorage.evtFileReadAsyncComplete.RemoveListener(evtFileReadAsyncComplete.Invoke);
            settings.client.remoteStorage.evtFileShareResult.RemoveListener(evtFileShareResult.Invoke);
            settings.client.remoteStorage.evtFileWriteAsyncComplete.RemoveListener(evtFileWriteAsyncComplete.Invoke);
#endif
#if HE_STEAMCOMPLETE
            settings.client.inventory.evtItemInstancesUpdated.RemoveListener(evtItemInstancesUpdated.Invoke);
            settings.client.inventory.evtItemsGranted.RemoveListener(evtItemsGranted.Invoke);
            settings.client.inventory.evtItemsConsumed.RemoveListener(evtItemsConsumed.Invoke);
            settings.client.inventory.evtItemsExchanged.RemoveListener(evtItemsExchanged.Invoke);
            settings.client.inventory.evtItemsDropped.RemoveListener(evtItemsDropped.Invoke);

            MatchmakingTools.evtGameLobbyJoinRequest.RemoveListener(evtGameLobbyJoinRequest.Invoke);
            MatchmakingTools.evtLobbyMatchList.RemoveListener(evtLobbyMatchList.Invoke);
            MatchmakingTools.evtLobbyCreated.RemoveListener(evtLobbyCreated.Invoke);
            MatchmakingTools.evtLobbyEnter.RemoveListener(evtLobbyEnter.Invoke);
            MatchmakingTools.evtLobbyExit.RemoveListener(evtLobbyExit.Invoke);
            MatchmakingTools.evtGameServerSet.RemoveListener(evtGameServerSet.Invoke);
            MatchmakingTools.evtLobbyChatUpdate.RemoveListener(evtLobbyChatUpdate.Invoke);
            MatchmakingTools.evtQuickMatchFailed.RemoveListener(evtQuickMatchFailed.Invoke);
            MatchmakingTools.evtSearchStarted.RemoveListener(evtSearchStarted.Invoke);
            MatchmakingTools.evtChatMessageReceived.RemoveListener(evtOnChatMessageReceived.Invoke);
#endif
#endif

#if UNITY_SERVER //|| UNITY_EDITOR
            Debug.Log("Logging off the Steam Game Server");

            if (settings.server.usingGameServerAuthApi)
                SteamGameServer.EnableHeartbeats(false);

            //Notify listeners of the shutdown
            settings.server.gameServerShuttingDown.Invoke();

#if MIRROR
            if (settings.server.enableMirror)
                Mirror.NetworkManager.singleton.StopServer();
#endif

            //Remove the settings event listeners
            settings.server.gameServerShuttingDown.RemoveListener(evtGameServerShuttingDown.Invoke);
            settings.server.disconnected.RemoveListener(evtDisconnected.Invoke);
            settings.server.connected.RemoveListener(evtConnected.Invoke);
            settings.server.failure.RemoveListener(evtFailure.Invoke);
            settings.server.gameServerShuttingDown.RemoveListener(LogShutDown);
            settings.server.disconnected.RemoveListener(LogDisconnect);
            settings.server.connected.RemoveListener(LogConnect);
            settings.server.failure.RemoveListener(LogFailure);

            //Log the server off of Steam
            SteamGameServer.LogOff();
            Debug.Log("Steam Game Server has been logged off");
#endif
        }

        // OnApplicationQuit gets called too early to shutdown the SteamAPI.
        // Because the SteamManager should be persistent and never disabled or destroyed we can shutdown the SteamAPI here.
        // Thus it is not recommended to perform any Steamworks work in other OnDestroy functions as the order of execution can not be garenteed upon Shutdown. Prefer OnDisable().
        private void OnDestroy()
        {
#if !UNITY_SERVER

            if (!SteamSettings.Initialized)
            {
                return;
            }

            if (settings != null && settings.client.user != null)
                settings.client.user.ClearData();

            Steamworks.SteamAPI.Shutdown();
#endif
#if UNITY_SERVER //|| UNITY_EDITOR
            Steamworks.GameServer.Shutdown();
#endif
        }

        private void Update()
        {
            if (!SteamSettings.Initialized)
            {
                return;
            }

#if !CONDITIONAL_COMPILE || !UNITY_SERVER
            Steamworks.SteamAPI.RunCallbacks();            

            if (settings != null)
            {
                //Refresh the notification position
                if (currentNotificationPosition != settings.client.overlay.notificationPosition)
                {
                    currentNotificationPosition = settings.client.overlay.notificationPosition;
                    settings.client.SetNotificationPosition(settings.client.overlay.notificationPosition);
                }

                if (currentNotificationIndent != settings.client.overlay.notificationInset)
                {
                    currentNotificationIndent = settings.client.overlay.notificationInset;
                    settings.client.SetNotificationInset(settings.client.overlay.notificationInset);
                }
            }
#endif
#if UNITY_SERVER //|| UNITY_EDITOR
            Steamworks.GameServer.RunCallbacks();
#endif
        }

        #region Server Only Logic
#if !CONDITIONAL_COMPILE || UNITY_SERVER || UNITY_EDITOR
        /// <summary>
        /// Initalizes the Steam Game Server API.
        /// </summary>
        /// <remarks>
        /// This is called automatically if <see cref="SteamSettings.GameServer.autoInitalize"/> is set to true on the <see cref="SteamSettings.server"/> member
        /// e.g. check the toggles on your Steam Settings object.
        /// </remarks>
        public void InitializeGameServer()
        {
            if (SteamSettings.Initialized)
            {
                Debug.LogError("Tried to initalize the Steamworks API twice in one session!");
                return;
            }

            if (settings.isDebugging)
                Debug.Log("Adding Steam Game Server connection listeners.");
            settings.server.connected.AddListener(evtConnected.Invoke);
            settings.server.connected.AddListener(OnSteamServersConnected);

            //If debugging
            if (settings.isDebugging)
            {
                Debug.Log("Adding Steam Game Server debug listeners");
                settings.server.gameServerShuttingDown.AddListener(LogShutDown);
                settings.server.disconnected.AddListener(LogDisconnect);
                settings.server.connected.AddListener(LogConnect);
                settings.server.failure.AddListener(LogFailure);
            }

            settings.Init();

            if (SteamSettings.Initialized)
                StartCoroutine(DelayLogOn());
        }

        IEnumerator DelayLogOn()
        {
            yield return null;
#if MIRROR
            if (settings.server.enableMirror)
            {
                yield return new WaitUntil(() => { return Mirror.NetworkManager.singleton != null; });

                Mirror.NetworkManager.singleton.StartServer();
                var startTime = Time.unscaledTime;

                yield return new WaitUntil(() => { return Mirror.NetworkServer.active || Time.unscaledTime - startTime > 30; });

                if (Mirror.NetworkServer.active)
                {
                    if (settings.server.autoLogon)
                        settings.server.LogOn();
                }
                else
                {
                    if (settings.server.autoLogon)
                        Debug.LogWarning("Mirror has taken more than 30 seconds to start the server process. SteamSettings was waiting for Mirror to finish before logging the GameServer on to Steam's backend. Due to the time constraint SteamSettings has abandoned the process. You can still log the GameServer on to Steam manually by calling SteamSettings.Server.LogOn(); This should only be done once you are sure Mirror's NetworkServer is active and listening.\n\nNOTE: You can determin if the server is currently logged on by either listening to the connect, disconnect and failure events of the server object or by checking the SteamSettings.Server.LoggedOn boolean value.");
                }
            }
            else
#endif
            {
                if (settings.server.autoLogon)
                    settings.server.LogOn();
            }
        }

        private void OnSteamServersConnected(SteamServersConnected_t pLogonSuccess)
        {
            settings.server.serverId = SteamGameServer.GetSteamID();
            Debug.Log("Game Server connected to Steamworks successfully!\n\tMod Directory = " + settings.server.gameDirectory + "\n\tApplicaiton ID = " + settings.applicationId.m_AppId.ToString() + "\n\tServer ID = " + settings.server.serverId.m_SteamID.ToString() + "\n\tServer Name = " + settings.server.serverName + "\n\tGame Description = " + settings.server.gameDescription + "\n\tMax Player Count = " + settings.server.maxPlayerCount.ToString());

            // Tell Steamworks about our server details
            SendUpdatedServerDetailsToSteam();
        }

        private void SendUpdatedServerDetailsToSteam()
        {
            if (settings.server.rulePairs != null && settings.server.rulePairs.Count > 0)
            {
                var pairString = "Set the following rules:\n";

                foreach (var pair in settings.server.rulePairs)
                {
                    SteamGameServer.SetKeyValue(pair.key, pair.value);
                    pairString += "\n\t[" + pair.key + "] = [" + pair.value + "]";
                }

                if (settings.isDebugging)
                    Debug.Log(pairString);
            }
        }

        private void LogFailure(SteamServerConnectFailure_t arg0)
        {
            Debug.LogError("Steamworks.GameServer.LogOn reported connection Failure: " + arg0.m_eResult.ToString());
        }

        private void LogConnect(SteamServersConnected_t arg0)
        {
            Debug.LogError("Steamworks.GameServer.LogOn reported connection Ready");
        }

        private void LogDisconnect(SteamServersDisconnected_t arg0)
        {
            Debug.LogError("Steamworks.GameServer reported connection Closed: " + arg0.m_eResult.ToString());
        }

        private void LogShutDown()
        {
            Debug.LogError("Steamworks.GameServer Logging Off");
        }
#endif
        #endregion

        #region Client Only Logic
#if !CONDITIONAL_COMPILE || !UNITY_SERVER
        [Obsolete("Use SteamSettings.Client.ListenForFriendMessages(bool) instead.")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool ListenForFriendMessages(bool isOn) => settings.client.ListenForFriendMessages(isOn);

        [Obsolete("Use friend.SendMessage(message) instead.")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool SendFriendChatMessage(UserData friend, string message) => friend.SendMessage(message);

        [Obsolete("Use SteamSettings.Client.SendFriendChatMessage(friend, message) instead.")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool SendFriendChatMessage(ulong friendId, string message) => SendFriendChatMessage(new CSteamID(friendId), message);

        [Obsolete("Use SteamSettings.Client.SendFriendChatMessage(friend, message) instead.")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool SendFriendChatMessage(CSteamID friend, string message) => SteamFriends.ReplyToFriendMessage(friend, message);
#endif
        #endregion

    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Replaced with SteamworksBehaviour\n"
        + "Its preferable to use the SteamSettings.Client or SteamSettings.Server objects directly.\n"
        + "For features that requrie the MonoBehaviour such as accessing insepctor exposed events or on demand initalization use SteamworksBehaviour e.g::\n"
        + "SteamworksBehaviour.InitializeGameServer(); (as an example)", true)]
    public class ClientApiSystem
    {

    }
}
#endif