using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;
using BrettArnett;

public class MultiplayerAGAIG : MonoBehaviour
{
    public LobbyManager lobby;

    public CSteamID _myID;
    public bool _isHost;
    public CSteamID _hostID;
    public bool _hostFound;
    public GamePlayer _myPlayer;

    public int[] _localGetCards = new int[4];
    public int[] _localButCards = new int[4];

    public bool drawReady = false;

    // Start is called before the first frame update (host hasn't been found yet when this is called)
    void Start()
    {
        _hostFound = false;
        UpdateHand("HOST_HG2G100G5G13B25B45B67B2");
    }

    // Just like Start(), but delayed to the first frame (host is found right before this is called)
    void FirstFrame()
    {
        Debug.Log("~~~~~~~~~~~~ CONNECTED ~~~~~~~~~~~~");
        // PrintPlayerData();
        // SendMessageOut("Hello World!");
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
                }
                if (_myID == (CSteamID)player.playerSteamId) {
                    _myPlayer = player;
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
                if (message.Substring(0, 6).Equals("HOST_H")) { // Update Hand
                    // Example: "HOST_HG2G100G5G13B25B45B67B2"
                    UpdateHand(message);
                }
            }
        }
    }

    private void UpdateHand(string message) {
        int index;
        for (int i = 0; i < 3; i++) {
            index = message.IndexOf('G');
            message = message.Substring(index+1);
            index = message.IndexOf('G');
            _localGetCards[i] = int.Parse(message.Substring(0, index));
        }
        index = message.IndexOf('G');
        message = message.Substring(index+1);
        index = message.IndexOf('B');
        _localGetCards[3] = int.Parse(message.Substring(0, index));
        for (int i = 4; i < 7; i++) {
            index = message.IndexOf('B');
            message = message.Substring(index+1);
            index = message.IndexOf('B');
            _localButCards[i-4] = int.Parse(message.Substring(0, index));
        }
        index = message.IndexOf('B');
        message = message.Substring(index+1);
        _localButCards[3] = int.Parse(message);
        drawReady = true;
    }

    // Debugging Function to see who is in the lobby
    public void PrintPlayerData()
    {
        foreach (GamePlayer player in lobby.GetPlayers()) {
            Debug.Log(player.playerName);
            Debug.Log(player.ConnectionId); // 0 = you're the host
            Debug.Log(player.playerSteamId);
        }
    }

    // IF YOU ARE HOST: This function sends a "HOST_" message to all players, including yourself
    // IF YOU ARE NOT HOST: This function sends a message to the host
    public void SendMessageOut(string message) {
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
    public void SendString(CSteamID receiver, string message) {
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
    public void OnP2PSessionRequest(P2PSessionRequest_t request)
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
