#if !CONDITIONAL_COMPILE || !UNITY_SERVER
#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace HeathenEngineering.SteamAPI
{

    /// <summary>
    /// Gathers and stores information about a Steamworks Clan aka Steamworks Group
    /// </summary>
    /// <remarks>
    /// <para>Steamworks Clans also know as Steamworks Groups are a social feature of the Steamworks API.
    /// You can fetch a list <see cref="SteamClan"/> objects for all the clans/groups a user belongs to via the <see cref="GameClient.ListClans"/> method accessable from the <see cref="client"/> member.</para>
    /// </remarks>
    [Serializable]
    public class SteamClan
    {
        public CSteamID id;
        public string displayName;
        public string tagString;
        public UserData Owner;
        public List<UserData> Officers;

        /// <summary>
        /// Occures when a chat message is recieved from this clan's chat room
        /// </summary>
        /// <remarks>
        /// The paramiters of the event indicate the user who is chatting, the message of the chat and the type of the chat.
        /// See <see cref="UnityChatMessageEvent"/> for more details.
        /// </remarks>
        public UnityChatMessageEvent chatMessageRecieved;
        /// <summary>
        /// Occures on responce of the request to join the clan's chat.
        /// </summary>
        /// <remarks>
        /// This will indicate rather or not there was an IO error and what the responce code from the clan was.
        /// See <see cref="UnityChatJoinEvent"/> for more details.
        /// </remarks>
        public UnityChatJoinEvent joinChatResponceRecieved;
        /// <summary>
        /// Occures when another user enters the clan's chat ... this will only trigger if the user is listening to the chat
        /// </summary>
        public UnitUserJoinedEvent userJoinedChat;
        /// <summary>
        /// Occures when anotyher user exits the clan's chat ... this will only trigger if the user is listening to the chat
        /// </summary>
        public UnitUserLeaveEvent userLeftChat;

        public SteamClan(CSteamID clanId)
        {
            id = clanId;
            Refresh();
        }

        /// <summary>
        /// Reloads data regarding a clan.
        /// </summary>
        public void Refresh()
        {
            displayName = SteamFriends.GetClanName(id);
            tagString = SteamFriends.GetClanTag(id);
            Owner = SteamSettings.current.client.GetUserData(SteamFriends.GetClanOwner(id));

            var officerCount = SteamFriends.GetClanOfficerCount(id);
            Officers = new List<UserData>();
            for (int i = 0; i < officerCount; i++)
            {
                Officers.Add(SteamSettings.current.client.GetUserData(SteamFriends.GetClanOfficerByIndex(id, i)));
            }
        }

        /// <summary>
        /// Opens the Steamworks Overlay to the clan chat
        /// </summary>
        /// <remarks>
        /// Note that it is possible to handle chat messages in game however Valve doesn't offier sufficent events to track membership and messages well.
        /// As a result we strongly recomend you use Clan chat through the Steamworks Overlay ... if you are interested in learning more check Valve's documentation at
        /// <see href="https://partner.steamgames.com/doc/api/ISteamFriends#JoinClanChatRoom"/>
        /// </remarks>
        public void OpenChat()
        {
            SteamFriends.ActivateGameOverlayToUser("chat", id);
        }

        /// <summary>
        /// Joins this clan's chat room if available
        /// </summary>
        public void JoinChat()
        {
            SteamSettings.Client.clanSystem.JoinChat(id);
        }

        /// <summary>
        /// Leaves the chat room
        /// </summary>
        /// <remarks>
        /// <see href="https://partner.steamgames.com/doc/api/ISteamFriends#LeaveClanChatRoom"/>
        /// </remarks>
        /// <returns>true if the user was in the chat room, false otherwise</returns>
        public bool LeaveChat()
        {
            return SteamSettings.Client.clanSystem.LeaveChat(id);
        }

        /// <summary>
        /// Returns a list of chat members in the clan chat.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This has some limitaitons as outlined in the Valve documetnation <see href="https://partner.steamgames.com/doc/api/ISteamFriends#GetClanChatMemberCount"/>.
        /// In particular large groups will not be able to be iterated over so will return 0 or incomplete lists of users. It is strongly recomended that you use the <see cref="OpenChat"/> feature to open the Steamworks Overlay to the chat as opposed to trying to manage it in game.
        /// </remarks>
        public List<UserData> GetChatMembers()
        {
            var results = new List<UserData>();
            var chatCount = SteamFriends.GetClanChatMemberCount(id);

            for (int i = 0; i < chatCount; i++)
            {
                results.Add(SteamSettings.current.client.GetUserData(SteamFriends.GetChatMemberByIndex(id, i)));
            }

            return results;
        }

        /// <summary>
        /// Sends a message to the clan's chat channel.
        /// </summary>
        /// <remarks>
        /// This should be used after you have called <see cref="JoinChat"/> and you have recived a positive responce on the <see cref="joinChatResponceRecieved"/>
        /// </remarks>
        /// <param name="message"></param>
        public void SendChatMessage(string message)
        {
            SteamFriends.SendClanChatMessage(id, message);
        }

        /// <summary>
        /// Internal Use
        /// </summary>
        /// <remarks>
        /// This is used by internal systems to process inbound chat messages from this clan. The message will be parsed and if successful the <see cref="chatMessageRecieved"/> event will be raised
        /// </remarks>
        /// <param name="param"></param>
        public void ProcessChatMsg(GameConnectedClanChatMsg_t param)
        {
            string buffer;
            EChatEntryType chatType;
            CSteamID chatter;
            if(SteamFriends.GetClanChatMessage(id, param.m_iMessageID, out buffer, 2048, out chatType, out chatter) > 0)
            {
                chatMessageRecieved.Invoke(SteamSettings.Client.GetUserData(chatter), buffer, chatType);
            }
        }

        /// <summary>
        /// Clan based chat message that carries the chatter's user data object, the message and the type of message, 
        /// </summary>
        /// <remarks>
        /// <para>
        /// param 1; <see cref="UserData"/> is the user that sent the chat message
        /// </para>
        /// <para>
        /// param 2; string is the message that was sent
        /// </para>
        /// <para>
        /// param 3; EChatEntryType is the type of message sent <see href="https://partner.steamgames.com/doc/api/steam_api#EChatEntryType" />
        /// </para>
        /// </remarks>
        public class UnityChatMessageEvent : UnityEvent<UserData, string, EChatEntryType> { }

        /// <summary>
        /// Clan based joined chat event.
        /// This occures when the current or local user joins a chat room and indicates rather the attempt to join was a success or not.
        /// </summary>
        /// <remarks>
        /// <para>
        /// param 1; bool is true when IO succedded, false otherwise
        /// </para>
        /// <para>
        /// param 2; EChatRoomEnterResponce indicates the responce from the server <see href="https://partner.steamgames.com/doc/api/steam_api#EChatRoomEnterResponse"/>
        /// </para>
        /// </remarks>
        public class UnityChatJoinEvent : UnityEvent<bool, EChatRoomEnterResponse> { }

        /// <summary>
        /// Clan based user joined chat event.
        /// This occures when other users join a chat that the local user is listening to and simply indicates people entering
        /// </summary>
        public class UnitUserJoinedEvent : UnityEvent<UserData> { }

        /// <summary>
        /// Clan based user left chat event.
        /// This occures when other users leaves a chat that the local user is listening to and simply indicates people exiting
        /// </summary>
        /// <remarks>
        /// <para>
        /// param 1; <see cref="UserData"/> is the user that left
        /// </para>
        /// <para>
        /// param 2; bool was the user kicked out
        /// </para>
        /// <para>
        /// param 3; bool did the connection to Steamworks drop
        /// </para>
        /// </remarks>
        public class UnitUserLeaveEvent : UnityEvent<UserData, bool, bool> { }
    }
}
#endif
#endif