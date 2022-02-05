#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using UnityEngine;

namespace HeathenEngineering.SteamAPI
{
    public class LobbyRecordBehvaiour : MonoBehaviour
    {
        public UnitySteamIdEvent OnSelected;

        public virtual void SetLobby(LobbyQueryResultRecord record)
        {

        }
    }
}
#endif
