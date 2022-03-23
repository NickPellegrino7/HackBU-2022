using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;
using BrettArnett;

public class AGAIG : MonoBehaviour
{
    public LobbyManager lobby;
    private ReinBot bot;

    private int deckSize = 100;

    private CSteamID _myID;
    private bool _isHost;
    private CSteamID _hostID;
    private bool _hostFound;

    public CardsPile getDeck;
    public GameObject getCardPrefab;

    // Start is called before the first frame update (host hasn't been found yet when this is called)
    void Start()
    {
        _hostFound = false;
    }

    // Just like Start(), but delayed to the first frame (host is found right before this is called)
    void FirstFrame()
    {
        Debug.Log("~~~~~~~~~~~~ CONNECTED ~~~~~~~~~~~~");
        PrintPlayerData();
        SendMessageOut("Hello World!");

        // Creating a deck of GET cards
        for (int i = 1; i < deckSize + 1; i++)
        {
            Card card = Instantiate(getCardPrefab).GetComponent<Card>();
            card.gameObject.name = card.gameObject.name + i.ToString();
            card.Initialize(i);
            getDeck.Add(card, true);
            Debug.Log(i);
        }
    }

    void Update()
    {
        // ~~~~~~~~~~~~~~~~~~~~
        // This section finds the host, then allows FirstFrame() to run once
        // ~~~~~~~~~~~~~~~~~~~~
        if (!_hostFound) {
            _myID = SteamUser.GetSteamID();
            foreach (GamePlayer player in lobby.GetPlayers()) {
                if (player.ConnectionId == 0) {
                    _hostFound = true;
                    _hostID = (CSteamID)player.playerSteamId;
                    _isHost = (_myID == _hostID);
                    FirstFrame();
                    break;
                }
            }
        }

        // ~~~~~~~~~~~~~~~~~~~~
        // This section checks for incoming messages
        // ~~~~~~~~~~~~~~~~~~~~

        uint size;

        // repeat while there's a P2P message available
        // will write its size to size variable
        while (SteamNetworking.IsP2PPacketAvailable(out size))
        {
            // allocate buffer and needed variables
            var buffer = new byte[size];
            uint bytesRead;
            CSteamID remoteId;

            // read the message into the buffer
            if (SteamNetworking.ReadP2PPacket(buffer, size, out bytesRead, out remoteId))
            {
                // convert to string
                char[] chars = new char[bytesRead / sizeof(char)];
                // Buffer.BlockCopy(buffer, 0, chars, 0, length);
                Buffer.BlockCopy(buffer, 0, chars, 0, (int) bytesRead);

            string message = new string(chars, 0, chars.Length);
                Debug.Log("Received a message: " + message);
            }
        }
    }

    // Debugging Function to see who is in the lobby
    void PrintPlayerData()
    {
        foreach (GamePlayer player in lobby.GetPlayers()) {
            Debug.Log(player.playerName);
            Debug.Log(player.ConnectionId); // 0 = you're the host
            Debug.Log(player.playerSteamId);
        }
    }

    // IF YOU ARE HOST: This function sends a "HOST_" message to all players, including yourself
    // IF YOU ARE NOT HOST: This function sends a message to the host
    void SendMessageOut(string message) {
        if (_isHost) {
            message = "HOST_" + message;
            foreach (GamePlayer player in lobby.GetPlayers()) {
                SendString((CSteamID)player.playerSteamId, message);
            }
        } else {
            SendString(_hostID, message);
        }
        Debug.Log("Sent a message: " + message);
    }

    // Send a message to a specific SteamID
    // This should only be called by the host, when telling a specific player what they drew
    // Non-hosts should exclusively use SendMessageOut() to talk to the Host
    void SendString(CSteamID receiver, string message) {
        // allocate new bytes array and copy string characters as bytes
        byte[] bytes = new byte[message.Length * sizeof(char)];
        System.Buffer.BlockCopy(message.ToCharArray(), 0, bytes, 0, bytes.Length);

        SteamNetworking.SendP2PPacket(receiver, bytes, (uint) bytes.Length, EP2PSend.k_EP2PSendReliable);
        /*
        There are four of these regarding the delivery method:
        k_EP2PSendUnreliable – Small packets, may get lost, can arrive out of order, but fast.
        k_EP2PSendUnreliableNoDelay – As above, but won’t do any connections checks. For this purpose it can be thrown away, but it is the fastest possible method of delivery.
        k_EP2PSendReliable – Reliable message send. For big packets, will arrive in order.
        k_EP2PSendReliableWithBuffering – As above but buffers the data before sending. Usually when you’re sending a lot of small packages that is not so important to be delivered immediately. (will be forced to send after 200 ms.)
        */
    }

    // Helper function, do not use or modify this please!
    void OnP2PSessionRequest(P2PSessionRequest_t request)
    {
        CSteamID clientId = request.m_steamIDRemote;

        // if (ExpectingClient(clientId))
        if (true)
        {
            SteamNetworking.AcceptP2PSessionWithUser(clientId);
        }/* else {
            Debug.LogWarning("Unexpected session request from " + clientId);
        }*/
    }

}
