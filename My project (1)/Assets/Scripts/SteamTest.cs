using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

// DOCUMENTATION: https://wiki.facepunch.com/steamworks/SteamFriends

public class SteamTest : MonoBehaviour
{

		/* private Callback<P2PSessionRequest_t> _p2PSessionRequestCallback; */

    // Start is called before the first frame update
    void Start()
    {
			/* _p2PSessionRequestCallback = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest); */

      if (!SteamManager.Initialized) { return; }

      string name = SteamFriends.GetPersonaName();
      Debug.Log(name);

      int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);

			for (int i = 0; i < friendCount; ++i)
			{
				CSteamID friendSteamId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
				string friendName = SteamFriends.GetFriendPersonaName(friendSteamId);
				EPersonaState friendState = SteamFriends.GetFriendPersonaState(friendSteamId);

				Debug.Log( $"{friendName} is {friendState}");
			}

		/*

		(ALMOST) EVERYTHING WE NEED IS HERE:
		https://blog.theknightsofunity.com/steamworks-and-unity-p2p-multiplayer/

		WE JUST NEED TO FIGURE OUT HOW TO FIND A CSteamID

		*/

		// public static bool SendP2PPacket(CSteamID steamIDRemote, byte[] pubData, uint cubData, EP2PSend eP2PSendType, int nChannel = 0

		/*

		CSteamID receiver = ...;
		string hello = "Hello!";

		// allocate new bytes array and copy string characters as bytes
		byte[] bytes = new byte[hello.Length * sizeof(char)];
		System.Buffer.BlockCopy(hello.ToCharArray(), 0, bytes, 0, bytes.Length);

		SteamNetworking.SendP2PPacket(receiver, bytes, (uint) bytes.Length, EP2PSend.k_EP2PSendReliable);

		*/
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

		/*

		void Update()
		{
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
		            Buffer.BlockCopy(buffer, 0, chars, 0, length);

		            string message = new string(chars, 0, chars.Length);
		            Debug.Log("Received a message: " + message);
		        }
		    }
		}

		void OnP2PSessionRequest(P2PSessionRequest_t request)
		{
		    CSteamID clientId = request.m_steamIDRemote;
		    if (ExpectingClient(clientId))
		    {
		        SteamNetworking.AcceptP2PSessionWithUser(clientId);
		    } else {
		        Debug.LogWarning("Unexpected session request from " + clientId);
		    }
		}

		*/

}
