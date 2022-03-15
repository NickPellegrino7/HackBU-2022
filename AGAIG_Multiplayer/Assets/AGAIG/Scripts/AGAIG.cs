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

    private CSteamID _myID;
    private bool _isHost;
    private CSteamID _hostID;
    private bool _hostFound;

    // Start is called before the first frame update
    void Start()
    {
        _hostFound = false;
    }

    void Update()
    {
        if (!_hostFound) {
            _myID = SteamUser.GetSteamID();
            foreach (GamePlayer player in lobby.GetPlayers()) {
                if (player.ConnectionId == 0) {
                    _hostFound = true;
                    _hostID = (CSteamID)player.playerSteamId;
                    _isHost = (_myID == _hostID);

                    Debug.Log("ADWWIJHOUBVJBKIOPK");
                    PrintPlayerData();
                    SendMessageOut("Hello World!");
                    break;
                }
            }
        }

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

    void PrintPlayerData()
    {
        foreach (GamePlayer player in lobby.GetPlayers()) {
            Debug.Log(player.playerName);
            Debug.Log(player.ConnectionId); // 0 = you're the host
            Debug.Log(player.playerSteamId);
        }
    }

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
