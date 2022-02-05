#if !CONDITIONAL_COMPILE || !UNITY_SERVER
#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public class ClanTools
    {
        public List<SteamClan> clans = new List<SteamClan>();

#pragma warning disable IDE0052 // Remove unread private members
        private Callback<GameConnectedClanChatMsg_t> m_GameConnectedClanChatMsg;
        private Callback<GameConnectedChatJoin_t> m_GameConnectedChatJoin;
        private Callback<GameConnectedChatLeave_t> m_GameConnectedChatLeave;
        private CallResult<JoinClanChatRoomCompletionResult_t> m_JoinCLanChatRoomCompletionResult;
#pragma warning restore IDE0052 // Remove unread private members

        public void RegisterSystem()
        {
            m_GameConnectedClanChatMsg = Callback<GameConnectedClanChatMsg_t>.Create(HandleClanChatMsg);
            m_GameConnectedChatJoin = Callback<GameConnectedChatJoin_t>.Create(HandleChatJoin);
            m_GameConnectedChatLeave = Callback<GameConnectedChatLeave_t>.Create(HandleChatLeave);

            RefreshClanList();
        }

        private void HandleChatLeave(GameConnectedChatLeave_t param)
        {
            var clan = clans.FirstOrDefault(p => p.id == param.m_steamIDClanChat);

            if (clan != default)
                clan.userLeftChat.Invoke(SteamSettings.Client.GetUserData(param.m_steamIDUser), param.m_bKicked, param.m_bDropped);
        }

        private void HandleChatJoin(GameConnectedChatJoin_t param)
        {
            var clan = clans.FirstOrDefault(p => p.id == param.m_steamIDClanChat);

            if (clan != default)
                clan.userJoinedChat.Invoke(SteamSettings.Client.GetUserData(param.m_steamIDUser));
        }

        private void HandleChatJoined(JoinClanChatRoomCompletionResult_t param, bool bIOFailure)
        {
            var clan = clans.FirstOrDefault(p => p.id == param.m_steamIDClanChat);

            if (clan != default)
                clan.joinChatResponceRecieved.Invoke(!bIOFailure, param.m_eChatRoomEnterResponse);
        }

        private void HandleClanChatMsg(GameConnectedClanChatMsg_t param)
        {
            var clan = clans.FirstOrDefault(p => p.id == param.m_steamIDClanChat);

            if (clan != default)
                clan.ProcessChatMsg(param);
        }

        /// <summary>
        /// Gets a list of <see cref="SteamClan"/> representing the 'clans' or 'groups' the local user is a member of
        /// </summary>
        /// <remarks>
        /// For more details on what a Steamworks Clan or Group is and how this method works please read <see href="https://partner.steamgames.com/doc/api/ISteamFriends#GetClanCount"/> in Valve's documentation.
        /// </remarks>
        /// <returns>A list of <see cref="SteamClan"/> objects for each clan/group the user is a member of</returns>
        public void RefreshClanList()
        {
            var clanCount = SteamFriends.GetClanCount();
            for (int i = 0; i < clanCount; i++)
            {
                var clanId = SteamFriends.GetClanByIndex(i);
                var clan = clans.FirstOrDefault(p => p.id == clanId);

                if (clan == default)
                {
                    clan = new SteamClan(clanId);
                    clans.Add(clan);
                }
                else
                    clan.Refresh();
            }
        }

        /// <summary>
        /// Joins the chat of a specific clan.
        /// </summary>
        /// <remarks>
        /// callbacks from this request will be routed through the matching <see cref="SteamClan"/> object.
        /// You should insure that the clan indicated exists in the <see cref="clans"/> member
        /// </remarks>
        /// <param name="clanId"></param>
        public void JoinChat(CSteamID clanId)
        {
            var call = SteamFriends.JoinClanChatRoom(clanId);
            m_JoinCLanChatRoomCompletionResult.Set(call, HandleChatJoined);
        }

        /// <summary>
        /// Diconnects from a clan chat
        /// </summary>
        /// <param name="id">The clan to diconnect from</param>
        /// <returns>true if the user was in the specified chat, false otherwise</returns>
        public bool LeaveChat(CSteamID id)
        {
            return SteamFriends.LeaveClanChatRoom(id);
        }
    }
}
#endif
#endif