#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE

namespace HeathenEngineering.SteamworksIntegration
{
    [System.Serializable]
    public struct UserLobbyLeaveData
    {
        public UserData user;
        public Steamworks.EChatMemberStateChange state;
    }
}
#endif