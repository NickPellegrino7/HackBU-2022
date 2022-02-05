#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using System;
using UnityEngine.Events;

namespace HeathenEngineering.SteamAPI
{
    /// <summary>
    /// Custom Unity event used by <see cref="GameServerBrowser"/>
    /// </summary>
    [Serializable]
    public class ServerQueryEvent : UnityEvent<GameServerBrowserEntery>
    { }
}
#endif