#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE && !UNITY_SERVER
using Steamworks;
using UnityEngine;

namespace HeathenEngineering.SteamAPI.Demo
{
    /// <summary>
    /// An example implamentation of <see cref="LobbyRecordBehvaiour"/>.
    /// This is a UI behvaiour used to display <see cref="LobbyQueryResultRecord"/> objects to the user.
    /// </summary>
    public class ExampleLobbyRecord : LobbyRecordBehvaiour
    {
        public UnityEngine.UI.Text lobbyId;
        public UnityEngine.UI.Text lobbySize;
        public UnityEngine.UI.Button connectButton;
        public UnityEngine.UI.Text buttonLabel;

        [Header("List Record")]
        public LobbyQueryResultRecord record;

        /// <summary>
        /// Registers the <see cref="LobbyQueryResultRecord"/> to the UI behaviour.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="lobbySettings"></param>
        public override void SetLobby(LobbyQueryResultRecord record)
        {
            Debug.Log("Setting lobby data for " + record.lobbyId);

            this.record = record;
            lobbyId.text = string.IsNullOrEmpty(record.name) ? "<unknown>" : record.name;

            if(record.metadata.ContainsKey("gamemode"))

            lobbySize.text = record.maxSlots.ToString();
        }

        /// <summary>
        /// Called when the UI object is selected in the by the user
        /// </summary>
        public void Selected()
        {
            OnSelected.Invoke(record.lobbyId);
        }

        private void Update()
        {
            if (record.lobbyId != CSteamID.Nil
                && MatchmakingTools.HasLobby
                && MatchmakingTools.lobbies[0].id.m_SteamID == record.lobbyId.m_SteamID)
            {
                connectButton.interactable = false;
                buttonLabel.text = "You are here!";
            }
            else
            {
                connectButton.interactable = true;
                buttonLabel.text = "Join lobby!";
            }
        }
    }

    
}
#endif