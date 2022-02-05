#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Collections.Generic;
using UnityEditor.UIElements;

namespace HeathenEngineering.SteamAPI.Editors
{

    public class SteamInspector_Code : EditorWindow
    {
        public static SteamInspector_Code instance;

        public VisualTreeAsset headerTree;
        public VisualTreeAsset dlcEntry;
        public VisualTreeAsset statEntry;
        public VisualTreeAsset achievementEntry;
        public VisualTreeAsset leaderboardEntry;
        public StyleSheet styleSheet;

        public Texture2D avatarPlaceholderImage;

        private Label initalizationField;
        private Label listedAppId;
        private Label reportedAppId;
        private Label steamAppId;
        private VisualElement avatarImage;
        private Label csteamId;
        private Label userName;
        private Label dlcCount;
        private Label statsCount;
        private Label achievmentCount;
        private Label leaderboardCount;
        public ToolbarToggle commonToggle;
        public ToolbarToggle lobbyToggle;
        public ToolbarToggle inventoryToggle;
        private VisualElement dlcCollection;
        private VisualElement statCollection;
        private VisualElement achievmentCollection;
        private VisualElement leaderboardCollection;
        private VisualElement commonContainer;
        private VisualElement lobbyContainer;
        private VisualElement inventoryContainer;
        private IMGUIContainer inventoryIMGUI;
        private IMGUIContainer lobbyIMGUI;
        public InventoryIMGUITool generator;
        private LobbyIMGUITool lobbyTools;

        private bool internalUpdate = false;

        [MenuItem("Steamworks/Inspector")]
        public static void ShowExample()
        {
            instance = GetWindow<SteamInspector_Code>();
            instance.titleContent = new GUIContent("Steamworks Inspector");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            generator = new InventoryIMGUITool();
            lobbyTools = new LobbyIMGUITool();

            VisualElement labelFromUXML = headerTree.CloneTree();
            root.Add(labelFromUXML);

            initalizationField = root.Q<Label>(name = "lblInit");
            listedAppId = root.Q<Label>(name = "lblListedAppId");
            reportedAppId = root.Q<Label>(name = "lblRptAppId");
            steamAppId = root.Q<Label>(name = "lblSteamAppIdTxt");
            avatarImage = root.Q<VisualElement>(name = "imgAvatar");
            csteamId = root.Q<Label>(name = "lblCsteamId");
            userName = root.Q<Label>(name = "lblUserName");
            dlcCount = root.Q<Label>(name = "lblDlcCount");
            dlcCollection = root.Q<VisualElement>(name = "dlcContent");
            achievmentCount = root.Q<Label>(name = "lblAchCount");
            leaderboardCount = root.Q<Label>(name = "lblLdrBrdCount");
            statsCount = root.Q<Label>(name = "lblStatsCount");
            statCollection = root.Q<VisualElement>(name = "statContent");
            achievmentCollection = root.Q<VisualElement>(name = "achContent");
            leaderboardCollection = root.Q<VisualElement>(name = "ldrBoardContent");            

            commonToggle = root.Q<ToolbarToggle>(name = "tglCmn");
            lobbyToggle = root.Q<ToolbarToggle>(name = "tglLby");
            inventoryToggle = root.Q<ToolbarToggle>(name = "tglInv");

            commonContainer = root.Q<ScrollView>(name = "pgCommon");
            lobbyContainer = root.Q<VisualElement>(name = "pgLobby");
            inventoryContainer = root.Q<VisualElement>(name = "pgInventory");
            inventoryIMGUI = root.Q<IMGUIContainer>(name = "invContainer");
            inventoryIMGUI.onGUIHandler = generator.OnGUI;
            lobbyIMGUI = root.Q<IMGUIContainer>(name = "lobbyContainer");
            lobbyIMGUI.onGUIHandler = lobbyTools.OnGUI;


            commonToggle.RegisterValueChangedCallback(HandleCommonToggleChange);
            lobbyToggle.RegisterValueChangedCallback(HandleLobbyToggleChange);
            inventoryToggle.RegisterValueChangedCallback(HandleInventoryToggleChange);
        }

        private void HandleCommonToggleChange(ChangeEvent<bool> evt)
        {
            if (!evt.newValue || internalUpdate)
                return;

            internalUpdate = true;

            if (commonToggle.value)
            {
                if (lobbyToggle.value)
                    lobbyToggle.value = false;
                if (inventoryToggle.value)
                    inventoryToggle.value = false;

                lobbyContainer.style.display = DisplayStyle.None;
                inventoryContainer.style.display = DisplayStyle.None;
                commonContainer.style.display = DisplayStyle.Flex;
            }

            internalUpdate = false;
        }

        private void HandleLobbyToggleChange(ChangeEvent<bool> evt)
        {
            if (!evt.newValue || internalUpdate)
                return;

            internalUpdate = true;

            if (lobbyToggle.value)
            {
                if (commonToggle.value)
                    commonToggle.value = false;
                if (inventoryToggle.value)
                    inventoryToggle.value = false;

                lobbyContainer.style.display = DisplayStyle.Flex;
                inventoryContainer.style.display = DisplayStyle.None;
                commonContainer.style.display = DisplayStyle.None;
            }

            internalUpdate = false;
        }

        private void HandleInventoryToggleChange(ChangeEvent<bool> evt)
        {
            if (!evt.newValue || internalUpdate)
                return;

            internalUpdate = true;

            if (inventoryToggle.value)
            {
                if (lobbyToggle.value)
                    lobbyToggle.value = false;
                if (commonToggle.value)
                    commonToggle.value = false;

                lobbyContainer.style.display = DisplayStyle.None;
                inventoryContainer.style.display = DisplayStyle.Flex;
                commonContainer.style.display = DisplayStyle.None;
            }

            internalUpdate = false;
        }

        private void OnGUI()
        {

            if (steamAppId == null)
            {
                VisualElement root = rootVisualElement;
                steamAppId = root.Q<Label>(name = "lblSteamAppIdTxt");
            }

            var appIdPath = Application.dataPath.Replace("/Assets", "") + "/steam_appid.txt";

            var appIdTxtExists = File.Exists(appIdPath);

            if (!appIdTxtExists)
            {
                File.WriteAllText(appIdPath, "480");
                steamAppId.text = "480";
                Debug.LogWarning("The steam_appid.txt file was missing and has been populated with the default 480.\nThis should be updated to your proper app id, when you change this file you must restart both Unity and Visual Studio in order to fully reinialize the Steamworks API.");
            }
            else
            {
                steamAppId.text = File.ReadAllText(appIdPath);
            }

            if (Application.isPlaying)
            {
                if (HeathenEngineering.SteamAPI.SteamSettings.Initialized)
                {
                    initalizationField.text = "Initialized";
                    listedAppId.text = HeathenEngineering.SteamAPI.SteamSettings.ApplicationId.m_AppId.ToString();
                    reportedAppId.text = HeathenEngineering.SteamAPI.SteamSettings.GetAppId().m_AppId.ToString();
                }
                else if (HeathenEngineering.SteamAPI.SteamSettings.HasInitalizationError)
                    initalizationField.text = "Erred";
                else
                    initalizationField.text = "Pending";

                var userData = HeathenEngineering.SteamAPI.SteamSettings.Initialized ? HeathenEngineering.SteamAPI.SteamSettings.Client.user : null;

                if (userData != null)
                {
                    if (userData.avatar != null)
                        avatarImage.style.backgroundImage = new StyleBackground(userData.avatar);

                    csteamId.text = userData.id.ToString();
                    userName.text = userData.DisplayName;
                }
                else
                {
                    avatarImage.style.backgroundImage = new StyleBackground(avatarPlaceholderImage);
                    csteamId.text = "0";
                    userName.text = "unknown";
                }

                UpdateAchievements();
                UpdateDLC();
                UpdateStats();
                UpdateLeaderboard();


            }
            else
            {
                initalizationField.text = "Idle";
                listedAppId.text = "unknown";
                reportedAppId.text = "unknown";

                avatarImage.style.backgroundImage = new StyleBackground(avatarPlaceholderImage);
                csteamId.text = "0";
                userName.text = "unknown";

                dlcCount.text = "DLC:";
                achievmentCount.text = "Achievements:";
                statsCount.text = "Stats:";
                leaderboardCount.text = "Leaderboards:";

                if (dlcCollection.childCount > 0)
                    dlcCollection.Clear();

                if (achievmentCollection.childCount > 0)
                    achievmentCollection.Clear();

                if (statCollection.childCount > 0)
                    statCollection.Clear();

                if (leaderboardCollection.childCount > 0)
                    leaderboardCollection.Clear();
            }
        }

        private void UpdateAchievements()
        {
            achievmentCount.text = "Achievements: " + HeathenEngineering.SteamAPI.SteamSettings.Achievements.Count.ToString();

            if (achievmentCollection.childCount > HeathenEngineering.SteamAPI.SteamSettings.Achievements.Count)
                achievmentCollection.Clear();

            while (achievmentCollection.childCount < HeathenEngineering.SteamAPI.SteamSettings.Achievements.Count)
            {
                var nElement = achievementEntry.CloneTree();
                achievmentCollection.Add(nElement);
            }

            var itemList = new List<VisualElement>(achievmentCollection.Children());

            for (int i = 0; i < HeathenEngineering.SteamAPI.SteamSettings.Achievements.Count; i++)
            {
                var ach = HeathenEngineering.SteamAPI.SteamSettings.Achievements[i];
                Label lblName = itemList[i].Query<Label>(name = "lblAchName");
                lblName.text = ach.displayName;

                Label lblId = itemList[i].Query<Label>(name = "lblAchId");
                lblId.text = ach.achievementId;

                Label lblStatus = itemList[i].Query<Label>(name = "lblStatus");
                lblStatus.text = ach.isAchieved.ToString();
            }
        }

        private void UpdateStats()
        {
            statsCount.text = "Stats: " + HeathenEngineering.SteamAPI.SteamSettings.Stats.Count.ToString();

            if (statCollection.childCount > HeathenEngineering.SteamAPI.SteamSettings.Stats.Count)
                statCollection.Clear();

            while (statCollection.childCount < HeathenEngineering.SteamAPI.SteamSettings.Stats.Count)
            {
                var nElement = statEntry.CloneTree();
                statCollection.Add(nElement);
            }

            var statList = new List<VisualElement>(statCollection.Children());

            for (int i = 0; i < HeathenEngineering.SteamAPI.SteamSettings.Stats.Count; i++)
            {
                var stat = HeathenEngineering.SteamAPI.SteamSettings.Stats[i];
                Label statName = statList[i].Query<Label>(name = "lblStatName");
                statName.text = stat.name;

                Label statDataType = statList[i].Query<Label>(name = "lblDataType");
                statDataType.text = stat.DataType.ToString();

                Label statValue = statList[i].Query<Label>(name = "lblValue");
                if (stat.DataType == HeathenEngineering.SteamAPI.StatObject.StatDataType.Float)
                    statValue.text = stat.GetFloatValue().ToString();
                else
                    statValue.text = stat.GetIntValue().ToString();
            }
        }

        private void UpdateDLC()
        {
            dlcCount.text = "DLC: " + HeathenEngineering.SteamAPI.SteamSettings.DLC.Count.ToString();

            if (dlcCollection.childCount > HeathenEngineering.SteamAPI.SteamSettings.DLC.Count)
                dlcCollection.Clear();

            while (dlcCollection.childCount < HeathenEngineering.SteamAPI.SteamSettings.DLC.Count)
            {
                var nElement = dlcEntry.CloneTree();
                dlcCollection.Add(nElement);
            }

            var dlcList = new List<VisualElement>(dlcCollection.Children());

            for (int i = 0; i < HeathenEngineering.SteamAPI.SteamSettings.DLC.Count; i++)
            {
                var dlc = HeathenEngineering.SteamAPI.SteamSettings.DLC[i];
                Label dlcName = dlcList[i].Query<Label>(name = "lblDlcName");
                dlcName.text = dlc.name;

                Label dlcId = dlcList[i].Query<Label>(name = "lblAppId");
                dlcId.text = dlc.AppId.ToString();

                Label dlcSub = dlcList[i].Query<Label>(name = "lblSubscribed");
                dlcSub.text = dlc.IsSubscribed.ToString();
            }
        }

        private void UpdateLeaderboard()
        {
            leaderboardCount.text = "Leaderboards: " + HeathenEngineering.SteamAPI.SteamSettings.Leaderboards.Count.ToString();

            if (leaderboardCollection.childCount > HeathenEngineering.SteamAPI.SteamSettings.Leaderboards.Count)
                leaderboardCollection.Clear();

            while (leaderboardCollection.childCount < HeathenEngineering.SteamAPI.SteamSettings.Leaderboards.Count)
            {
                var nElement = leaderboardEntry.CloneTree();
                leaderboardCollection.Add(nElement);
            }

            var itemList = new List<VisualElement>(leaderboardCollection.Children());

            for (int i = 0; i < HeathenEngineering.SteamAPI.SteamSettings.Leaderboards.Count; i++)
            {
                var leaderboard = HeathenEngineering.SteamAPI.SteamSettings.Leaderboards[i];
                Label lblName = itemList[i].Query<Label>(name = "lblName");
                lblName.text = leaderboard.leaderboardName;

                Label lblId = itemList[i].Query<Label>(name = "lblId");
                lblId.text = leaderboard.leaderboardId?.ToString();

                Label lblScore = itemList[i].Query<Label>(name = "lblScore");
                lblScore.text = leaderboard.userEntry?.entry.m_nScore.ToString();

                Label lblRank = itemList[i].Query<Label>(name = "lblRank");
                lblRank.text = leaderboard.userEntry?.entry.m_nGlobalRank.ToString();
            }
        }
    }
}
#endif