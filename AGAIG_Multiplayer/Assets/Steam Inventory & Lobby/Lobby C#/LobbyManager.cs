using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;
using System.Linq;
using BrettArnett;

namespace BrettArnett
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager instance;

        [Header("UI Elements")]
        [SerializeField] private Text LobbyNameText;
        [SerializeField] private GameObject ContentPanel;
        [SerializeField] private GameObject PlayerListItemPrefab;

        public bool havePlayerListItemsBeenCreated = false;
        private List<PlayerListItem> playerListItems = new List<PlayerListItem>();
        public GameObject localGamePlayerObject;
        public GamePlayer localGamePlayerScript;

        public ulong currentLobbyId;


        private MyNetworkManager game;
        private MyNetworkManager Game
        {
            get
            {
                if (game != null)
                {
                    return game;
                }
                return game = MyNetworkManager.singleton as MyNetworkManager;
            }
        }

        void Awake()
        {
            MakeInstance();
        }

        void MakeInstance()
        {
            if (instance == null)
                instance = this;
        }

        public void FindLocalGamePlayer()
        {
            localGamePlayerObject = GameObject.Find("LocalGamePlayer");
            localGamePlayerScript = localGamePlayerObject.GetComponent<GamePlayer>();
        }

        public void UpdateLobbyName()
        {
            Debug.Log("UpdateLobbyName");
            currentLobbyId = Game.GetComponent<SteamLobby>().current_lobbyID;
            string lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)currentLobbyId, "name");
            Debug.Log("UpdateLobbyName: new lobby name will be: " + lobbyName);
            LobbyNameText.text = lobbyName;
        }

        public void UpdateUI()
        {
            Debug.Log("Executing UpdateUI");
            if (!havePlayerListItemsBeenCreated)
                CreatePlayerListItems();
            if (playerListItems.Count < Game.GamePlayers.Count)
                CreateNewPlayerListItems();
            if (playerListItems.Count > Game.GamePlayers.Count)
                RemovePlayerListItems();
            if (playerListItems.Count == Game.GamePlayers.Count)
                UpdatePlayerListItems();
        }

        private void CreatePlayerListItems()
        {
            Debug.Log("Executing CreatePlayerListItems. This many players to create: " + Game.GamePlayers.Count.ToString());
            foreach (GamePlayer player in Game.GamePlayers)
            {
                Debug.Log("CreatePlayerListItems: Creating playerlistitem for player: " + player.playerName);
                GameObject newPlayerListItem = Instantiate(PlayerListItemPrefab) as GameObject;
                PlayerListItem newPlayerListItemScript = newPlayerListItem.GetComponent<PlayerListItem>();

                newPlayerListItemScript.playerName = player.playerName;
                newPlayerListItemScript.ConnectionId = player.ConnectionId;
                newPlayerListItemScript.playerSteamId = player.playerSteamId;
                newPlayerListItemScript.SetPlayerListItemValues();


                newPlayerListItem.transform.SetParent(ContentPanel.transform);
                newPlayerListItem.transform.localScale = Vector3.one;

                playerListItems.Add(newPlayerListItemScript);
            }
            havePlayerListItemsBeenCreated = true;
        }

        private void CreateNewPlayerListItems()
        {
            Debug.Log("Executing CreateNewPlayerListItems");
            foreach (GamePlayer player in Game.GamePlayers)
            {
                if (!playerListItems.Any(b => b.ConnectionId == player.ConnectionId))
                {
                    Debug.Log("CreateNewPlayerListItems: Player not found in playerListItems: " + player.playerName);
                    GameObject newPlayerListItem = Instantiate(PlayerListItemPrefab) as GameObject;
                    PlayerListItem newPlayerListItemScript = newPlayerListItem.GetComponent<PlayerListItem>();

                    newPlayerListItemScript.playerName = player.playerName;
                    newPlayerListItemScript.ConnectionId = player.ConnectionId;
                    newPlayerListItemScript.playerSteamId = player.playerSteamId;
                    newPlayerListItemScript.SetPlayerListItemValues();


                    newPlayerListItem.transform.SetParent(ContentPanel.transform);
                    newPlayerListItem.transform.localScale = Vector3.one;

                    playerListItems.Add(newPlayerListItemScript);
                }
            }
        }

        private void RemovePlayerListItems()
        {
            List<PlayerListItem> playerListItemsToRemove = new List<PlayerListItem>();
            foreach (PlayerListItem playerListItem in playerListItems)
            {
                if (!Game.GamePlayers.Any(b => b.ConnectionId == playerListItem.ConnectionId))
                {
                    Debug.Log("RemovePlayerListItems: player list item fro connection id: " + playerListItem.ConnectionId.ToString() + " does not exist in the game players list");
                    playerListItemsToRemove.Add(playerListItem);
                }
            }
            if (playerListItemsToRemove.Count > 0)
            {
                foreach (PlayerListItem playerListItemToRemove in playerListItemsToRemove)
                {
                    GameObject playerListItemToRemoveObject = playerListItemToRemove.gameObject;
                    playerListItems.Remove(playerListItemToRemove);
                    Destroy(playerListItemToRemoveObject);
                    playerListItemToRemoveObject = null;
                }
            }
        }

        private void UpdatePlayerListItems()
        {
            Debug.Log("Executing UpdatePlayerListItems");
            foreach (GamePlayer player in Game.GamePlayers)
            {
                foreach (PlayerListItem playerListItemScript in playerListItems)
                {
                    if (playerListItemScript.ConnectionId == player.ConnectionId)
                    {
                        playerListItemScript.playerName = player.playerName;
                        playerListItemScript.SetPlayerListItemValues();
                    }
                }
            }
        }

        public void DestroyPlayerListItems()
        {
            foreach (PlayerListItem playerListItem in playerListItems)
            {
                GameObject playerListItemObject = playerListItem.gameObject;
                Destroy(playerListItemObject);
                playerListItemObject = null;
            }
            playerListItems.Clear();
        }

        public void PlayerQuitLobby()
        {
            localGamePlayerScript.QuitLobby();
        }
    }
}