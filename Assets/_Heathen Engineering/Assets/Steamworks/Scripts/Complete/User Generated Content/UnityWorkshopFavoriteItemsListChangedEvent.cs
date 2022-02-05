#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using Steamworks;
using System;
using UnityEngine.Events;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public class UnityWorkshopFavoriteItemsListChangedEvent : UnityEvent<UserFavoriteItemsListChanged_t>
    { }
}
#endif
