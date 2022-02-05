#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES
using HeathenEngineering.SteamAPI.UI;
using Steamworks;
using UnityEditor;

namespace HeathenEngineering.SteamAPI.Editors
{
    [CustomEditor(typeof(LeaderboardDisplayList))]
    public class HeathenSteamLeaderboardEditor : Editor
    {
        private LeaderboardDisplayList board;

        public SerializedProperty Settings;
        public SerializedProperty entryPrototype;
        public SerializedProperty collection;
        public SerializedProperty focusPlayer;
        public SerializedProperty ignorePlayerRefresh;

        private void OnEnable()
        {
            Settings = serializedObject.FindProperty("Settings");
            entryPrototype = serializedObject.FindProperty("entryPrototype");
            collection = serializedObject.FindProperty("collection");
            focusPlayer = serializedObject.FindProperty("focusPlayer");
            ignorePlayerRefresh = serializedObject.FindProperty("ignorePlayerRefresh");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(Settings);
            serializedObject.ApplyModifiedProperties();

            board = target as LeaderboardDisplayList;

            if (board.Settings == null)
                return;

            var b = EditorGUILayout.Toggle("Create Leaderboard if missing?", board.Settings.createIfMissing);
            if (b != board.Settings.createIfMissing)
            {
                board.Settings.createIfMissing = b;
                EditorUtility.SetDirty(board.Settings);
            }

            if (board.Settings.createIfMissing)
            {
                var v1 = (ELeaderboardSortMethod)EditorGUILayout.EnumPopup("Sort Method", board.Settings.sortMethod);
                if (v1 != board.Settings.sortMethod)
                {
                    board.Settings.sortMethod = v1;
                    EditorUtility.SetDirty(board.Settings);
                }

                var v2 = (ELeaderboardDisplayType)EditorGUILayout.EnumPopup("Display Type", board.Settings.displayType);
                if (v2 != board.Settings.displayType)
                {
                    board.Settings.displayType = v2;
                    EditorUtility.SetDirty(board.Settings);
                }
            }

            EditorGUILayout.PropertyField(focusPlayer);
            var n = EditorGUILayout.TextField("Name", board.Settings.leaderboardName);
            if (n != board.Settings.leaderboardName)
            {
                board.Settings.leaderboardName = n;
                EditorUtility.SetDirty(board.Settings);
            }
            
            EditorGUILayout.PropertyField(entryPrototype);
            EditorGUILayout.PropertyField(collection);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif