#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using System;

namespace HeathenEngineering.SteamAPI
{
    /// <summary>
    /// Structure of the player entry data returned by the <see cref="GameServerBrowserTools.PlayerDetails(GameServerBrowserEntery, Action{GameServerBrowserEntery, bool})"/> method
    /// </summary>
    [Serializable]
    public class ServerPlayerEntry
    {
        public string name;
        public int score;
        public TimeSpan timePlayed;
    }
}
#endif