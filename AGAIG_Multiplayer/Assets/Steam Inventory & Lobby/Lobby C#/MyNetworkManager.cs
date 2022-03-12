using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using BrettArnett;
using UnityEngine.SceneManagement;

namespace BrettArnett
{
    public class MyNetworkManager : NetworkManager
    {
        [SerializeField] private GamePlayer gamePlayerPrefab;
        [SerializeField] public int minPlayers = 1;
        public List<GamePlayer> GamePlayers { get; } = new List<GamePlayer>();


        public override void OnStartServer()
        {
            Debug.Log("Starting Server");
            ServerChangeScene("Scene_Lobby"); // Your intened scene for lobby play.
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            Debug.Log("Client connected.");
            base.OnClientConnect(conn);
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            bool isGameLeader = GamePlayers.Count == 0; // isLeader is true when you are the first player

            GamePlayer GamePlayerInstance = Instantiate(gamePlayerPrefab);

            GamePlayerInstance.IsGameLeader = isGameLeader;
            GamePlayerInstance.ConnectionId = conn.connectionId;
            GamePlayerInstance.playerNumber = GamePlayers.Count + 1;

            GamePlayerInstance.playerSteamId = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instance.current_lobbyID, GamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
            Debug.Log("Player added. Player name: " + GamePlayerInstance.playerName + ". Player connection id: " + GamePlayerInstance.ConnectionId.ToString());
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            if (conn.identity != null)
            {
                GamePlayer player = conn.identity.GetComponent<GamePlayer>();
                GamePlayers.Remove(player);
            }
            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            GamePlayers.Clear();
        }

        public void HostShutDownServer()
        {
            GameObject NetworkManagerObject = GameObject.Find("NetworkManager");
            Destroy(this.GetComponent<SteamManager>());
            Destroy(NetworkManagerObject);
            Shutdown();
            SceneManager.LoadScene("Scene_Steamworks"); // Steamworks is your game start scene

            Start();
        }
    }
}