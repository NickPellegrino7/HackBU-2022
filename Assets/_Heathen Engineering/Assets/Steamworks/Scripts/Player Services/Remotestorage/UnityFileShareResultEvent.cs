#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES
using Steamworks;
using System;
using UnityEngine.Events;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public class UnityFileShareResultEvent : UnityEvent<RemoteStorageFileShareResult_t> { }
}
#endif
