#if !DISABLESTEAMWORKS
using UnityEditor;
using UnityEngine;

namespace HeathenEngineering.SteamAPI.Editors
{

    [CustomEditor(typeof(SteamworksBehaviour))]
    public class SteamworksClientApiSystemEditor : Editor
    {
        private SteamworksBehaviour pManager;
        private SerializedProperty Settings;

        //General
        private SerializedProperty DoNotDestroyOnLoad;
        private SerializedProperty OnSteamInitalized;
        private SerializedProperty OnSteamInitalizationError;
        
        //Client
        private SerializedProperty OnAvatarLoaded;
        private SerializedProperty OnPersonaStateChanged;
        private SerializedProperty OnUserStatsRecieved;
        private SerializedProperty OnUserStatsStored;
        private SerializedProperty OnOverlayActivated;
        private SerializedProperty OnAchievementStored;
        private SerializedProperty OnRecievedFriendChatMessage;
        private SerializedProperty OnNumberOfCurrentPlayersResult;

#if HE_STEAMPLAYERSERVICES
        private SerializedProperty leaderboardRankChanged;
        private SerializedProperty leaderboardRankLoaded;
        private SerializedProperty leaderboardNewHighRank;
        private SerializedProperty fileReadAsyncComplete;
        private SerializedProperty fileWriteAsyncComplete;
        private SerializedProperty fileShareResult;
#endif
#if HE_STEAMCOMPLETE
        public SerializedProperty itemInstancesUpdated;
        public SerializedProperty itemsGranted;
        public SerializedProperty itemsConsumed;
        public SerializedProperty itemsExchanged;
        public SerializedProperty itemsDroped;

        public SerializedProperty OnGameLobbyJoinRequest;
        public SerializedProperty OnLobbyMatchList;
        public SerializedProperty OnLobbyCreated;
        public SerializedProperty OnLobbyEnter;
        public SerializedProperty OnLobbyExit;
        public SerializedProperty OnGameServerSet;
        public SerializedProperty OnLobbyChatUpdate;
        public SerializedProperty QuickMatchFailed;
        public SerializedProperty SearchStarted;
        public SerializedProperty OnChatMessageReceived;
#endif

        //Server
        private SerializedProperty disconnected;
        private SerializedProperty connected;
        private SerializedProperty failure;

        public Texture2D achievementIcon;
        public Texture2D statIcon;
        public Texture2D leaderboardIcon;
        public Texture2D dlcIcon;
        public Texture itemIcon;
        public Texture generatorIcon;
        public Texture tagIcon;
        public Texture bundleIcon;
        public Texture recipeIcon;
        public Texture pointerIcon;
        public Texture2D dropBoxTexture;

        private int tabPage = 0;

        private void OnEnable()
        {
            Settings = serializedObject.FindProperty(nameof(SteamworksBehaviour.settings));

            OnSteamInitalized = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtSteamInitalized));
            OnSteamInitalizationError = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtSteamInitalizationError));
            OnOverlayActivated = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtOverlayActivated));

#if HE_STEAMPLAYERSERVICES
            leaderboardRankChanged = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtLeaderboardRankChanged));
            leaderboardRankLoaded = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtLeaderboardRankLoaded));
            leaderboardNewHighRank = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtLeaderboardNewHighRank));

            fileReadAsyncComplete = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtFileReadAsyncComplete));
            fileWriteAsyncComplete = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtFileWriteAsyncComplete));
            fileShareResult = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtFileShareResult));
#endif

#if HE_STEAMCOMPLETE
            itemInstancesUpdated = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtItemInstancesUpdated));
            itemsGranted = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtItemsGranted));
            itemsConsumed = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtItemsConsumed));
            itemsExchanged = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtItemsExchanged));
            itemsDroped = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtItemsDropped));

            OnGameLobbyJoinRequest = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtGameLobbyJoinRequest));
            OnLobbyMatchList = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtLobbyMatchList));
            OnLobbyCreated = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtLobbyCreated));
            OnLobbyEnter = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtLobbyEnter));
            OnLobbyExit = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtLobbyExit));
            OnGameServerSet = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtGameServerSet));
            OnLobbyChatUpdate = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtLobbyChatUpdate));
            QuickMatchFailed = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtQuickMatchFailed));
            SearchStarted = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtSearchStarted));
            OnChatMessageReceived = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtOnChatMessageReceived));
#endif

            OnUserStatsRecieved = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtUserStatsRecieved));
            OnUserStatsStored = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtUserStatsStored));
            OnAchievementStored = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtAchievementStored));
            OnAvatarLoaded = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtAvatarLoaded));
            OnPersonaStateChanged = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtPersonaStateChanged));
            OnRecievedFriendChatMessage = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtRecievedFriendChatMessage));

            disconnected = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtDisconnected));
            connected = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtConnected));
            failure = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtFailure));

            OnNumberOfCurrentPlayersResult = serializedObject.FindProperty(nameof(SteamworksBehaviour.evtNumberOfCurrentPlayersResult));
        }

        public override void OnInspectorGUI()
        {
            pManager = target as SteamworksBehaviour;

            if (pManager != null)
            {
                if (pManager.settings != null)
                {
                    if (pManager.settings.client == null)
                        pManager.settings.client = new SteamSettings.GameClient();

                    if (pManager.settings.server == null)
                        pManager.settings.server = new SteamSettings.GameServer();

                    if (pManager.settings.achievements == null)
                        pManager.settings.achievements = new System.Collections.Generic.List<AchievementObject>();

                    if (pManager.settings.stats == null)
                        pManager.settings.stats = new System.Collections.Generic.List<StatObject>();

                    pManager.settings.stats.RemoveAll(p => p == null);
                    pManager.settings.achievements.RemoveAll(p => p == null);
                }
            }
            
            EditorGUILayout.PropertyField(Settings, GUIContent.none, true);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Events");
            Rect hRect = EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("");

            Rect bRect = new Rect(hRect);
            bRect.width = (hRect.width - 25) / 3f;
            var tWidth = bRect.width;
            bRect.width = 25;
            tabPage = GUI.Toggle(bRect, tabPage == 0, "X", EditorStyles.toolbarButton) ? 0 : tabPage;
            bRect.x += 25;
            bRect.width = tWidth;
            tabPage = GUI.Toggle(bRect, tabPage == 1, "Common", EditorStyles.toolbarButton) ? 1 : tabPage;
            bRect.x += bRect.width;
            tabPage = GUI.Toggle(bRect, tabPage == 2, "Stat & Achievements", EditorStyles.toolbarButton) ? 2 : tabPage;
            bRect.x += bRect.width;
            tabPage = GUI.Toggle(bRect, tabPage == 3, "Game Server", EditorStyles.toolbarButton) ? 3 : tabPage;
            EditorGUILayout.EndHorizontal();

            Rect nhRect = EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("");

#if HE_STEAMCOMPLETE
            bRect = new Rect(nhRect);
            bRect.width = nhRect.width / 4f;
            tabPage = GUI.Toggle(bRect, tabPage == 4, "Leaderboard", EditorStyles.toolbarButton) ? 4 : tabPage;
            bRect.x += bRect.width;
            tabPage = GUI.Toggle(bRect, tabPage == 5, "Remote Storage", EditorStyles.toolbarButton) ? 5 : tabPage;
            bRect.x += bRect.width;
            tabPage = GUI.Toggle(bRect, tabPage == 6, "Inventory", EditorStyles.toolbarButton) ? 6 : tabPage;
            bRect.x += bRect.width;
            tabPage = GUI.Toggle(bRect, tabPage == 7, "Lobby", EditorStyles.toolbarButton) ? 7 : tabPage;
#elif HE_STEAMPLAYERSERVICES
            bRect = new Rect(nhRect);
            bRect.width = nhRect.width / 2f;
            tabPage = GUI.Toggle(bRect, tabPage == 4, "Leaderboard", EditorStyles.toolbarButton) ? 4 : tabPage;
            bRect.x += bRect.width;
            tabPage = GUI.Toggle(bRect, tabPage == 5, "Remote Storage", EditorStyles.toolbarButton) ? 5 : tabPage;
#endif
            EditorGUILayout.EndHorizontal();

            switch(tabPage)
            {
                case 1:
                    EditorGUILayout.PropertyField(OnSteamInitalized);
                    EditorGUILayout.PropertyField(OnSteamInitalizationError);
                    EditorGUILayout.PropertyField(OnOverlayActivated);
                    EditorGUILayout.PropertyField(OnPersonaStateChanged);
                    EditorGUILayout.PropertyField(OnNumberOfCurrentPlayersResult);
                    EditorGUILayout.PropertyField(OnRecievedFriendChatMessage);
                    EditorGUILayout.PropertyField(OnAvatarLoaded);
                    break;
                case 2:
                    EditorGUILayout.PropertyField(OnUserStatsRecieved);
                    EditorGUILayout.PropertyField(OnUserStatsStored);
                    EditorGUILayout.PropertyField(OnAchievementStored);
                    break;
                case 3:
                    EditorGUILayout.PropertyField(disconnected);
                    EditorGUILayout.PropertyField(connected);
                    EditorGUILayout.PropertyField(failure);
                    break;
#if HE_STEAMPLAYERSERVICES || HE_STEAMCOMPLETE
                case 4:
                    EditorGUILayout.PropertyField(leaderboardRankChanged);
                    EditorGUILayout.PropertyField(leaderboardRankLoaded);
                    EditorGUILayout.PropertyField(leaderboardNewHighRank);
                    break;
                case 5:
                    EditorGUILayout.PropertyField(fileReadAsyncComplete);
                    EditorGUILayout.PropertyField(fileWriteAsyncComplete);
                    EditorGUILayout.PropertyField(fileShareResult);
                    break;
#endif
#if HE_STEAMCOMPLETE
                case 6:
                    EditorGUILayout.PropertyField(itemInstancesUpdated);
                    EditorGUILayout.PropertyField(itemsGranted);
                    EditorGUILayout.PropertyField(itemsConsumed);
                    EditorGUILayout.PropertyField(itemsExchanged);
                    EditorGUILayout.PropertyField(itemsDroped);
                    break;
                case 7:
                    EditorGUILayout.PropertyField(OnGameLobbyJoinRequest);
                    EditorGUILayout.PropertyField(OnLobbyMatchList);
                    EditorGUILayout.PropertyField(OnLobbyCreated);
                    EditorGUILayout.PropertyField(OnLobbyEnter);
                    EditorGUILayout.PropertyField(OnLobbyExit);
                    EditorGUILayout.PropertyField(OnGameServerSet);
                    EditorGUILayout.PropertyField(OnLobbyChatUpdate);
                    EditorGUILayout.PropertyField(QuickMatchFailed);
                    EditorGUILayout.PropertyField(SearchStarted);
                    EditorGUILayout.PropertyField(OnChatMessageReceived);
                    break;
#endif
                default:
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif