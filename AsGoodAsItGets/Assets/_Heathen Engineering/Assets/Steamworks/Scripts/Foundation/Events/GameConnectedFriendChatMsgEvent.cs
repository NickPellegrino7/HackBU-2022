#if !DISABLESTEAMWORKS
using Steamworks;
using UnityEngine.Events;

namespace HeathenEngineering.SteamworksIntegration
{
    [System.Serializable]
    public class GameConnectedFriendChatMsgEvent : UnityEvent<UserData, string, EChatEntryType> { }
}
#endif