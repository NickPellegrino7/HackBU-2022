#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using System;
using UnityEditor;
using UnityEngine;

namespace HeathenEngineering.SteamAPI.Editors
{
    public class LobbyIMGUITool
    {
        public Texture icon;
        public Texture2D userBackground;

        private int lobbyIndex = 0;
        private Vector2 scrollPos;

        public void OnGUI()
        {
            if(Application.isPlaying)
            {
                if(MatchmakingTools.Initalized)
                {
                    if(MatchmakingTools.lobbies != null)
                    {
                        if(MatchmakingTools.lobbies.Count > 0)
                        {
                            DrawWindow();
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Steamworks Lobby Tools is initalized!\n No lobbies currently connected.", MessageType.Info);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Steamworks Lobby Tools is initalized however the lobbies collection is null, this indicates an issue with the initalizaiton process, please contact Heathen Support on Discord.", MessageType.Warning);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Steamworks Lobby Tools not initalized!\nThe lobby inspector only works in play mode and when the SteamworksLobbyTool system has been initalized.", MessageType.Info);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("The lobby inspector only works in play mode and when the Steamworks Lobby Tools system has been initalized.", MessageType.Info);
            }
        }

        private void DrawWindow()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            ListLobbies();
            EditorGUILayout.EndHorizontal();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            LobbyDetails(MatchmakingTools.lobbies[lobbyIndex]);
            EditorGUILayout.EndScrollView();
        }

        private void ListLobbies()
        {
            for (int i = 0; i < MatchmakingTools.lobbies.Count; i++)
            {
                var lobby = MatchmakingTools.lobbies[i];

                if (lobby != null)
                {
                    if (DrawLobbyEntry(lobby, lobbyIndex == i))
                        lobbyIndex = i;
                }
            }
        }

        private void LobbyDetails(Lobby lobby)
        {
            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Lobby Name: ");
            EditorGUILayout.SelectableLabel(lobby.Name, GUILayout.Height(15));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Lobby ID: ");
            EditorGUILayout.SelectableLabel(lobby.id.ToString(), GUILayout.Height(18));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            var entries = lobby.GetMetadataEntries();
            EditorGUILayout.LabelField("Metadata: (" + entries.Count + ")");

            EditorGUI.indentLevel++;
            
            foreach (var data in entries)
            {
                EditorGUILayout.SelectableLabel(data.Key + " : " + data.Value, GUILayout.Height(18));
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Members: (" + lobby.members.Count + ")");

            EditorGUI.indentLevel++;

            foreach (var member in lobby.members)
            {
                ListUserDetails(member);
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();

        }

        private void ListUserDetails(LobbyMember member)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Box(member.userData.avatar, GUILayout.Width(64), GUILayout.Height(64));

            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("User ID: ");
            EditorGUILayout.SelectableLabel(member.userData.id.ToString(), GUILayout.Height(18));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Display Name: ");
            EditorGUILayout.SelectableLabel(member.userData.DisplayName, GUILayout.Height(18));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("User Level: ");
            EditorGUILayout.SelectableLabel(member.userData.Level.ToString(), GUILayout.Height(18));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Is Ready: ");
            EditorGUILayout.SelectableLabel(member.IsReady.ToString(), GUILayout.Height(18));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Game Version: ");
            EditorGUILayout.SelectableLabel(member.GameVersion.ToString(), GUILayout.Height(18));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        private bool DrawLobbyEntry(Lobby lobby, bool selected)
        {
            if (MatchmakingTools.normalLobby == lobby)
                return GUILayout.Toggle(selected, "Normal: " + lobby.id.ToString(), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
            else if (MatchmakingTools.groupLobby == lobby)
                return GUILayout.Toggle(selected, "Group: " + lobby.id.ToString(), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
            else
                return GUILayout.Toggle(selected, "Invisible: " + lobby.id.ToString(), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
        }
    }
}
#endif