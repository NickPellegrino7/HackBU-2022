#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE
using UnityEngine;
using HeathenEngineering.SteamworksIntegration;

namespace HeathenEngineering.DEMO
{
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    public class LobbyDisplayRecord : MonoBehaviour
    {
        [System.Serializable]
        public class JoinFailedEvent : UnityEngine.Events.UnityEvent<Steamworks.LobbyEnter_t>
        { }

        [SerializeField]
        private TMPro.TextMeshProUGUI nameLable;
        [SerializeField]
        private LobbyManager lobbyManager;
        
        public HeathenEngineering.SteamworksIntegration.LobbyDataEvent evtJoinCompleted;
        public JoinFailedEvent evtJoinError;

        private HeathenEngineering.SteamworksIntegration.Lobby lobby;
        public HeathenEngineering.SteamworksIntegration.Lobby Lobby
        {
            get => lobby;
            set
            {
                lobby = value;
                nameLable.text = lobby.Name;
                if (string.IsNullOrEmpty(nameLable.text))
                    nameLable.text = "Name not set";
            }
        }

        public void Join()
        {
            lobbyManager.Join(lobby);

            //HeathenEngineering.SteamworksIntegration.API.Matchmaking.Client.JoinLobby(lobby, (result, error) =>
            //{
            //    if (error)
            //        evtJoinError.Invoke(result);
            //    else
            //        evtJoinCompleted.Invoke(lobby);
            //});
        }
    }
}
#endif
