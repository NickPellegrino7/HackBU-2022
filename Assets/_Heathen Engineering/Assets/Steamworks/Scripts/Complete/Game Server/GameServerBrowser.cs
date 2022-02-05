#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE && !UNITY_SERVER
#if MIRROR
using Mirror;
#endif
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HeathenEngineering.SteamAPI
{
    /// <summary>
    /// Helps query for servers on the Steam Game Server system
    /// </summary>
    /// <remarks>
    /// This object is not required and serves only to expose a few UnityEvents to the inspector. all funcitonlity can be had directly in the <see cref="SteamSettings.Client"/> object via the <see cref="SteamSettings.GameClient.serverBrowser"/> member
    /// <para>
    /// Simplifies the process of querying for Steam Game Servers and stores the results of queries in <see cref="InternetServers"/>, <see cref="FavoritesServers"/>, <see cref="FriendsServers"/>, <see cref="LANServers"/>, <see cref="SpectatorServers"/>, and <see cref="HistoryServers"/> lists.
    /// </para>
    /// <para>
    /// The data provdied by this object can be used to drive UI elements or facilitate matchmaking/server discovery. Specifics of records returned can be controlled via the <see cref="filter"/> member.
    /// </para>
    /// </remarks>
    [Obsolete("This object is no longer needed in order to work with SteamGameServerBrowser features consider using the SteamSettings.Client.serverBrowser instead")]
    [RequireComponent(typeof(SteamworksBehaviour))]
    [DisallowMultipleComponent]
    public class GameServerBrowser : MonoBehaviour
    {
        /// <summary>
        /// The current filter to be used when searching for servers
        /// </summary>
        /// <remarks>
        /// This is a simple string to string key value pair
        /// </remarks>
        [HideInInspector]
        [Obsolete("No longer used, queries now take the filter input directly", true)]
        public List<MatchMakingKeyValuePair_t> filter = new List<MatchMakingKeyValuePair_t>();

        /// <summary>
        /// The list of servers listed on the "internet" tab
        /// </summary>
        [HideInInspector]
        [Obsolete("No longer used, queries now return the results directly", true)]
        public List<GameServerBrowserEntery> InternetServers;
        /// <summary>
        /// The list of servers listed on the "favorites" tab
        /// </summary>
        [HideInInspector]
        [Obsolete("No longer used, queries now return the results directly", true)]
        public List<GameServerBrowserEntery> FavoritesServers;
        /// <summary>
        /// The list of servers listed on the "friends" tab
        /// </summary>
        [HideInInspector]
        [Obsolete("No longer used, queries now return the results directly", true)]
        public List<GameServerBrowserEntery> FriendsServers;
        /// <summary>
        /// The list of servers listed on the "LAN" tab
        /// </summary>
        [HideInInspector]
        [Obsolete("No longer used, queries now return the results directly", true)]
        public List<GameServerBrowserEntery> LANServers;
        /// <summary>
        /// The list of servers listed on the "spectator" tab
        /// </summary>
        [HideInInspector]
        [Obsolete("No longer used, queries now return the results directly", true)]
        public List<GameServerBrowserEntery> SpectatorServers;
        /// <summary>
        /// The list of servers listed on the "history" tab
        /// </summary>
        [HideInInspector]
        [Obsolete("No longer used, queries now return the results directly", true)]
        public List<GameServerBrowserEntery> HistoryServers;

        /// <summary>
        /// Occures when a search has completed;
        /// </summary>
        public GameServerBrowserTools.ResultsEvent evtSearchCompleted;

        [HideInInspector]
        [Obsolete("Use " + nameof(evtSearchCompleted) + " instead", true)]
        public UnityEvent InternetServerListUpdated;
        [HideInInspector]
        [Obsolete("Use " + nameof(evtSearchCompleted) + " instead", true)]
        public UnityEvent FavoriteServerListUpdated;
        [HideInInspector]
        [Obsolete("Use " + nameof(evtSearchCompleted) + " instead", true)]
        public UnityEvent FriendsServerListUpdated;
        [HideInInspector]
        [Obsolete("Use " + nameof(evtSearchCompleted) + " instead", true)]
        public UnityEvent LANServerListUpdated;
        [HideInInspector]
        [Obsolete("Use " + nameof(evtSearchCompleted) + " instead", true)]
        public UnityEvent SpectatorServerListUpdated;
        [HideInInspector]
        [Obsolete("Use " + nameof(evtSearchCompleted) + " instead", true)]
        public UnityEvent HistoryServerListUpdated;
        [HideInInspector]
        [Obsolete("Use " + nameof(evtSearchCompleted) + " instead", true)]
        public UnityEvent ServerRefreshFailed;
        [HideInInspector]
        [Obsolete("Use " + nameof(evtSearchCompleted) + " instead")]
        public ServerQueryEvent RulesQueryCompleted => evtRulesQueryCompleted;
        /// <summary>
        /// Occures when a rules query completes
        /// </summary>
        public ServerQueryEvent evtRulesQueryCompleted;
        [HideInInspector]
        [Obsolete("Use " + nameof(evtRulesQueryFailed) + " instead")]
        public ServerQueryEvent RulesQueryFailed => evtRulesQueryFailed;
        /// <summary>
        /// Occures when a rules query fails
        /// </summary>
        public ServerQueryEvent evtRulesQueryFailed;
        [HideInInspector]
        [Obsolete("Use " + nameof(evtPingCompleted) + " instead")]
        public ServerQueryEvent PingCompleted => evtPingCompleted;
        /// <summary>
        /// Occures when ping request is completed
        /// </summary>
        public ServerQueryEvent evtPingCompleted;
        [HideInInspector]
        [Obsolete("Use " + nameof(evtPingFailed) + " instead")]
        public ServerQueryEvent PingFailed => evtPingFailed;
        /// <summary>
        /// Occures when ping request fails
        /// </summary>
        public ServerQueryEvent evtPingFailed;
        [HideInInspector]
        [Obsolete("Use " + nameof(evtPlayerListQueryCompleted) + " instead")]
        public ServerQueryEvent PlayerListQueryCompleted => evtPlayerListQueryCompleted;
        /// <summary>
        /// Occures when the player list query completes
        /// </summary>
        public ServerQueryEvent evtPlayerListQueryCompleted;
        [HideInInspector]
        [Obsolete("Use " + nameof(evtPlayerListQueryFailed) + " instead")]
        public ServerQueryEvent PlayerListQueryFailed => evtPlayerListQueryFailed;
        /// <summary>
        /// Occures if the player list query fails
        /// </summary>
        public ServerQueryEvent evtPlayerListQueryFailed;

#if MIRROR
        /// <summary>
        /// Join the indicated server if we are not already part of a server
        /// </summary>
        /// <param name="steamId"></param>
        /// <returns></returns>
        public bool JoinServer(CSteamID steamId)
        {
            if (!Mirror.NetworkManager.singleton.isNetworkActive)
            {
                Mirror.NetworkManager.singleton.networkAddress = steamId.m_SteamID.ToString();
                Mirror.NetworkManager.singleton.StartClient();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Leave the current server e.g. Stop Client Networking
        /// </summary>
        public void LeaveServer()
        {
            Mirror.NetworkManager.singleton.StopClient();
        }

        /// <summary>
        /// Switch to the indicated server i.e. leave the current server if any and join the indicated one
        /// </summary>
        /// <param name="steamId"></param>
        public void SwitchServer(CSteamID steamId)
        {
            if (Mirror.NetworkManager.singleton.isNetworkActive)
                NetworkManager.singleton.StopClient();

            Mirror.NetworkManager.singleton.networkAddress = steamId.m_SteamID.ToString();
            Mirror.NetworkManager.singleton.StartClient();
        }
#endif

        private void OnEnable()
        {
            SteamSettings.Client.serverBrowser.evtSearchCompleted.AddListener(evtSearchCompleted.Invoke);
        }

        private void OnDisable()
        {
            SteamSettings.Client.serverBrowser.evtSearchCompleted.RemoveListener(evtSearchCompleted.Invoke);
        }

        [Obsolete("No longer used, filters are now passed in on each call" , true)]
        public void FilterClear()
        {
        }

        [Obsolete("No longer used, filters are now passed in on each call", true)]
        public void FilterAdd(string key, string value)
        {
        }

        [Obsolete("No longer used, filters are now passed in on each call", true)]
        public void FilterRemove(string key)
        {
        }

        /// <summary>
        /// Searches for the servers of the <paramref name="type"/> indicated which match the <paramref name="filter"/> provided, when found invokes both the <paramref name="callback"/> and the <see cref="evtSearchCompleted"/> event.
        /// </summary>
        /// <param name="type">The type of servers to search for</param>
        /// <param name="filter">You can apply a filter inline with the following syntax <code>new GameServerBrowser.Filter { { "key1", "value1" }, { "key2", "value2" } }</code></param>
        /// <param name="callback">An Action of the form: <code>Action{ List{GameServerBrowserEntry} results, bool bIOFailure }</code> This expects a method to handle the results of the search where the params should be a list of the results and a bool where true indicates a failure.
        /// This can be done inline with the following syntax:
        /// <code>
        /// (r, e) => { Debug.Log("Has Error: " + e.ToString() + ", Records found: " + r.Count.ToString() ); }
        /// </code></param>
        /// <example>
        /// <code>
        /// RefreshInternetServers(new GameServerBrowser.Filter { { "key1", "value1" }, { "key2", "value2" } }, (r, e) => { Debug.Log("Has Error: " + e.ToString() + ", Records found: " + r.Count.ToString() ); });
        /// </code>
        /// </example>
        public void RefreshServers(GameServerSearchType type, GameServerBrowserTools.Filter filter = null, Action<List<GameServerBrowserEntery>, bool> callback = null)
        {
            SteamSettings.Client.serverBrowser.GetServerList(type,
                (r, e) =>
                {
                    evtSearchCompleted.Invoke(type, r, e);
                    callback?.Invoke(r, e);
                },
                filter);
        }

        [Obsolete("Use " + nameof(RefreshServers) + " instead, this method will be removed in a later update!")]
        public void RefreshInternetServers(GameServerBrowserTools.Filter filter = null, Action<List<GameServerBrowserEntery>, bool> callback = null)
        {
            SteamSettings.Client.serverBrowser.GetServerList(GameServerSearchType.Internet,
                (r, e) =>
                {
                    evtSearchCompleted.Invoke(GameServerSearchType.Internet, r, e);
                    callback?.Invoke(r, e);
                },
                filter);
        }

        [Obsolete("Use " + nameof(RefreshServers) + " instead, this method will be removed in a later update!")]
        public void RefreshFavoriteServers(GameServerBrowserTools.Filter filter = null, Action<List<GameServerBrowserEntery>, bool> callback = null)
        {
            SteamSettings.Client.serverBrowser.GetServerList(GameServerSearchType.Favorites,
                (r, e) =>
                {
                    evtSearchCompleted.Invoke(GameServerSearchType.Favorites, r, e);
                    callback?.Invoke(r, e);
                },
                filter);
        }

        [Obsolete("Use " + nameof(RefreshServers) + " instead, this method will be removed in a later update!")]
        public void RefreshFriendServers(GameServerBrowserTools.Filter filter = null, Action<List<GameServerBrowserEntery>, bool> callback = null)
        {
            SteamSettings.Client.serverBrowser.GetServerList(GameServerSearchType.Friends,
                (r, e) =>
                {
                    evtSearchCompleted.Invoke(GameServerSearchType.Friends, r, e);
                    callback?.Invoke(r, e);
                },
                filter);
        }

        [Obsolete("Use " + nameof(RefreshServers) + " instead, this method will be removed in a later update!")]
        public void RefreshLANServers(GameServerBrowserTools.Filter filter = null, Action<List<GameServerBrowserEntery>, bool> callback = null)
        {
            SteamSettings.Client.serverBrowser.GetServerList(GameServerSearchType.LAN,
                (r, e) =>
                {
                    evtSearchCompleted.Invoke(GameServerSearchType.LAN, r, e);
                    callback?.Invoke(r, e);
                },
                filter);
        }

        [Obsolete("Use " + nameof(RefreshServers) + " instead, this method will be removed in a later update!")]
        public void RefreshSpectatorServers(GameServerBrowserTools.Filter filter = null, Action<List<GameServerBrowserEntery>, bool> callback = null)
        {
            SteamSettings.Client.serverBrowser.GetServerList(GameServerSearchType.Spectator,
                (r, e) =>
                {
                    evtSearchCompleted.Invoke(GameServerSearchType.Spectator, r, e);
                    callback?.Invoke(r, e);
                },
                filter);
        }

        [Obsolete("Use " + nameof(RefreshServers) + " instead, this method will be removed in a later update!")]
        public void RefreshHistoryServers(GameServerBrowserTools.Filter filter = null, Action<List<GameServerBrowserEntery>, bool> callback = null)
        {
            SteamSettings.Client.serverBrowser.GetServerList(GameServerSearchType.History,
                (r, e) =>
                {
                    evtSearchCompleted.Invoke(GameServerSearchType.History, r, e);
                    callback?.Invoke(r, e);
                },
                filter);
        }

        /// <summary>
        /// Refresh server rules
        /// </summary>
        /// <remarks>
        /// This method will invoke the <paramref name="callback"/> and <see cref="evtRulesQueryCompleted"/> or <see cref="evtRulesQueryFailed"/> on completion.
        /// On a successful completion the <see cref="GameServerBrowserEntery.rules"/> will be populated
        /// </remarks>
        /// <param name="target">Server to refresh the rules for</param>
        /// <param name="callback">An action of the form
        /// <code>Action{ GameeServerBrowserEntry entry, bool bIOFailure }</code>
        /// This expects a method to handle the results of the query where the params should be a <see cref="GameServerBrowserEntery"/> entry and a bool where true indicates a failure.</param>
        public void RefreshServerRules(GameServerBrowserEntery target, Action<GameServerBrowserEntery, bool> callback = null)
        {
            SteamSettings.Client.serverBrowser.ServerRules(target,
                (r, e) =>
                {
                    if (e)
                        evtRulesQueryFailed.Invoke(r);
                    else
                        evtRulesQueryCompleted.Invoke(r);

                    callback?.Invoke(r, e);
                });
        }

        /// <summary>
        /// Ping the given server refreshing its data
        /// </summary>
        /// <remarks>
        /// This method will invoke the <paramref name="callback"/> and <see cref="evtPingCompleted"/> or <see cref="evtPingFailed"/> on completion.
        /// On a successful completion the <see cref="GameServerBrowserEntery.Ping"/> and the base <see cref="gameserveritem_t.m_nPing"/> will be updated.
        /// </remarks>
        /// <param name="target">The server to refresh</param>
        /// <param name="callback">An action of the form
        /// <code>Action{ GameeServerBrowserEntry entry, bool bIOFailure }</code>
        /// This expects a method to handle the results of the query where the params should be a <see cref="GameServerBrowserEntery"/> entry and a bool where true indicates a failure.</param>
        public void PingServer(GameServerBrowserEntery target, Action<GameServerBrowserEntery, bool> callback = null)
        {
            SteamSettings.Client.serverBrowser.PingServer(target,
                (r, e) =>
                {
                    if (e)
                        evtPingFailed.Invoke(target);
                    else
                        evtPingCompleted.Invoke(target);

                    callback?.Invoke(r, e);
                });
        }

        /// <summary>
        /// Refresh the player list for the <paramref name="target"/> server
        /// </summary>
        /// <remarks>
        /// This method will invoke the <paramref name="callback"/> and <see cref="evtPlayerListQueryCompleted"/> or <see cref="evtPlayerListQueryFailed"/> on completion.
        /// On a successful completion the <see cref="GameServerBrowserEntery.players"/> list will be populated with the results.
        /// </remarks>
        /// <param name="target">The server to target</param>
        /// <param name="callback">An action of the form
        /// <code>Action{ GameeServerBrowserEntry entry, bool bIOFailure }</code>
        /// This expects a method to handle the results of the query where the params should be a <see cref="GameServerBrowserEntery"/> entry and a bool where true indicates a failure.</param>
        public void RefreshServerPlayerList(GameServerBrowserEntery target, Action<GameServerBrowserEntery, bool> callback = null)
        {
            SteamSettings.Client.serverBrowser.PlayerDetails(target,
                (r, e) =>
                {
                    if (e)
                        evtPlayerListQueryFailed.Invoke(target);
                    else
                        evtPlayerListQueryCompleted.Invoke(target);

                    callback?.Invoke(r, e);
                });
        }
    }

}
#endif