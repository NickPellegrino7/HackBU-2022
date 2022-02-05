#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE

namespace HeathenEngineering.SteamAPI
{
    /// <summary>
    /// Used in game server browser examples on the example game server browser entry
    /// </summary>
    public interface IGameServerDisplayBrowserEntry
    {
        void SetEntryRecord(GameServerBrowserEntery entry);
    }
}
#endif