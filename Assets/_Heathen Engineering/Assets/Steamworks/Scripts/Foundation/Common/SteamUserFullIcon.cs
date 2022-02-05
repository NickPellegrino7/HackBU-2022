#if !DISABLESTEAMWORKS && !UNITY_SERVER
using UnityEngine;

using UnityEngine.Serialization;
using System.Collections;

namespace HeathenEngineering.SteamAPI.UI
{
    /// <summary>
    /// <para>A composit control for displaying the avatar, name and status of a given <see cref="UserData"/> object.</para>
    /// </summary>
    [HelpURL("https://heathen-engineering.github.io/steamworks-v2-documentation/class_heathen_engineering_1_1_steam_a_p_i_1_1_u_i_1_1_steam_user_full_icon.html")]
    public class SteamUserFullIcon : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="UserData"/> to load.
        /// This should be set by calling <see cref="LinkSteamUser(UserData)"/>
        /// </summary>
        [FormerlySerializedAs("UserData")]
        public UserData userData;
        /// <summary>
        /// Should the status label be shown or not
        /// </summary>
        [FormerlySerializedAs("ShowStatusLabel")]
        public BoolReference showStatusLabel;

        /// <summary>
        /// The image to load the avatar into.
        /// </summary>
        [Header("References")]
        [FormerlySerializedAs("Avatar")]
        public UnityEngine.UI.RawImage avatar;
        /// <summary>
        /// The text field used to display the users name
        /// </summary>
        [FormerlySerializedAs("PersonaName")]
        public UnityEngine.UI.Text personaName;
        /// <summary>
        /// The text field used to display the users status
        /// </summary>
        [FormerlySerializedAs("StatusLabel")]
        public UnityEngine.UI.Text statusLabel;
        /// <summary>
        /// An image board around the icon ... this will have its color changed based on status
        /// </summary>
        [FormerlySerializedAs("IconBorder")]
        public UnityEngine.UI.Image iconBorder;
        /// <summary>
        /// The root object containing the status label parts ... this is what is enabled or disabled as the label is shown or hidden.
        /// </summary>
        [FormerlySerializedAs("StatusLabelContainer")]
        public GameObject statusLabelContainer;
        /// <summary>
        /// Should the persona name be colored based on status
        /// </summary>
        [FormerlySerializedAs("ColorThePersonaName")]
        public bool colorThePersonaName = true;
        /// <summary>
        /// Should the status label be colored based on status
        /// </summary>
        [FormerlySerializedAs("ColorTheStatusLabel")]
        public bool colorTheStatusLabel = true;
        /// <summary>
        /// <para></para>
        /// <para>You can use <see cref="HeathenEngineering.Scriptable.ColorVariable"/> to configure these with an asset or set them in editor.</para>
        /// </summary>
        [Header("Border Colors")]
        [FormerlySerializedAs("OfflineColor")]
        public ColorReference offlineColor;
        /// <summary>
        /// <para>The color to use for Online</para>
        /// <para>You can use <see cref="HeathenEngineering.Scriptable.ColorVariable"/> to configure these with an asset or set them in editor.</para>
        /// </summary>
        [FormerlySerializedAs("OnlineColor")]
        public ColorReference onlineColor;
        /// <summary>
        /// <para>The color to use for Away</para>
        /// <para>You can use <see cref="HeathenEngineering.Scriptable.ColorVariable"/> to configure these with an asset or set them in editor.</para>
        /// </summary>
        [FormerlySerializedAs("AwayColor")]
        public ColorReference awayColor;
        /// <summary>
        /// <para>The color to use for Buisy</para>
        /// <para>You can use <see cref="HeathenEngineering.Scriptable.ColorVariable"/> to configure these with an asset or set them in editor.</para>
        /// </summary>
        [FormerlySerializedAs("BuisyColor")]
        public ColorReference buisyColor;
        /// <summary>
        /// <para>The color to use for Snooze</para>
        /// <para>You can use <see cref="HeathenEngineering.Scriptable.ColorVariable"/> to configure these with an asset or set them in editor.</para>
        /// </summary>
        [FormerlySerializedAs("SnoozeColor")]
        public ColorReference snoozeColor;
        /// <summary>
        /// <para>The color to use for the Want to Play status</para>
        /// <para>You can use <see cref="HeathenEngineering.Scriptable.ColorVariable"/> to configure these with an asset or set them in editor.</para>
        /// </summary>
        [FormerlySerializedAs("WantPlayColor")]
        public ColorReference wantPlayColor;
        /// <summary>
        /// <para>The color to use for the Want to Trade status</para>
        /// <para>You can use <see cref="HeathenEngineering.Scriptable.ColorVariable"/> to configure these with an asset or set them in editor.</para>
        /// </summary>
        [FormerlySerializedAs("WantTradeColor")]
        public ColorReference wantTradeColor;
        /// <summary>
        /// <para>Color to use for In Game satus</para>
        /// <para>You can use <see cref="HeathenEngineering.Scriptable.ColorVariable"/> to configure these with an asset or set them in editor.</para>
        /// </summary>
        [FormerlySerializedAs("InGameColor")]
        public ColorReference inGameColor;
        /// <summary>
        /// <para>The color to use when in this specific game</para>
        /// <para>You can use <see cref="HeathenEngineering.Scriptable.ColorVariable"/> to configure these with an asset or set them in editor.</para>
        /// </summary>
        [FormerlySerializedAs("ThisGameColor")]
        public ColorReference thisGameColor;

        private RectTransform _selfTransform;
        public RectTransform SelfTransform
        {
            get
            {
                if (_selfTransform == null)
                    _selfTransform = GetComponent<RectTransform>();
                return _selfTransform;
            }
        }


        private void Start()
        {
            StartCoroutine(DelayStart());
        }

        IEnumerator DelayStart()
        {
            yield return new WaitUntil(() => { return SteamSettings.Initialized; });
            if (userData != null)
                LinkSteamUser(userData);
        }

        private void Update()
        {
            if (showStatusLabel.Value != statusLabelContainer.activeSelf)
                statusLabelContainer.SetActive(showStatusLabel.Value);
        }

        /// <summary>
        /// Sets and registeres for the provided <see cref="UserData"/> object.
        /// </summary>
        /// <param name="newUserData">The user to connect to and to display the avatar for.</param>
        /// <example>
        /// <list type="bullet">
        /// <item>
        /// <description>Set the icon to display the current user as read from the SteamSettings settings member.</description>
        /// <code>
        /// myUserFullIcon.LinkSteamUser(settings.UserData);
        /// </code>
        /// </item>
        /// </list>
        /// </example>
        public void LinkSteamUser(UserData newUserData)
        {
            if (!SteamSettings.Initialized)
            {
                Debug.LogWarning("Steam API is not yet initalized, Cannot link user data.");
                return;
            }

            if (userData != null)
            {
                if (userData.evtAvatarChanged != null)
                    userData.evtAvatarChanged.RemoveListener(HandleAvatarChange);
                if (userData.evtStateChange != null)
                    userData.evtStateChange.RemoveListener(HandleStateChange);
                if (userData.evtNameChanged != null)
                    userData.evtNameChanged.RemoveListener(HandleNameChanged);
                if (userData.evtAvatarLoaded != null)
                    userData.evtAvatarLoaded.RemoveListener(HandleAvatarChange);
            }

            userData = newUserData;
            HandleAvatarChange();
            HandleNameChanged();
            HandleStateChange();

            if (userData != null)
            {
                if (!userData.iconLoaded)
                    SteamSettings.current.client.RefreshAvatar(userData);

                avatar.texture = userData.avatar;
                if (userData.evtAvatarChanged == null)
                    userData.evtAvatarChanged = new UnityEngine.Events.UnityEvent();
                userData.evtAvatarChanged.AddListener(HandleAvatarChange);
                if (userData.evtStateChange == null)
                    userData.evtStateChange = new UnityEngine.Events.UnityEvent();
                userData.evtStateChange.AddListener(HandleStateChange);
                if (userData.evtNameChanged == null)
                    userData.evtNameChanged = new UnityEngine.Events.UnityEvent();
                userData.evtNameChanged.AddListener(HandleNameChanged);
                if (userData.evtAvatarLoaded == null)
                    userData.evtAvatarLoaded = new UnityEngine.Events.UnityEvent();
                userData.evtAvatarLoaded.AddListener(HandleAvatarChange);
            }
        }

        /// <summary>
        /// Force a refresh of all data for this user.
        /// </summary>
        public void RefreshUserData()
        {
            if (userData != null)
            {
                SteamSettings.current.client.RefreshAvatar(userData);
                HandleAvatarChange();
                HandleNameChanged();
                HandleStateChange();
            }
        }

        private void HandleNameChanged()
        {
            personaName.text = userData.DisplayName;
        }

        private void HandleAvatarChange()
        {
            avatar.texture = userData.avatar;
        }

        private void HandleStateChange()
        {
            switch(userData.State)
            {
                case Steamworks.EPersonaState.k_EPersonaStateAway:
                    if (userData.InGame)
                    {
                        if (userData.GameInfo.m_gameID.AppID().m_AppId == SteamSettings.current.applicationId.m_AppId)
                        {
                            statusLabel.text = "Playing";
                            iconBorder.color = thisGameColor.Value;
                        }
                        else
                        {
                            statusLabel.text = "In-Game";
                            iconBorder.color = inGameColor.Value;
                        }
                    }
                    else
                    {
                        statusLabel.text = "Away";
                        iconBorder.color = awayColor.Value;
                    }
                    break;
                case Steamworks.EPersonaState.k_EPersonaStateBusy:
                    if (userData.InGame)
                    {
                        if (userData.GameInfo.m_gameID.AppID().m_AppId == SteamSettings.current.applicationId.m_AppId)
                        {
                            statusLabel.text = "Playing";
                            iconBorder.color = thisGameColor.Value;
                        }
                        else
                        {
                            statusLabel.text = "In-Game";
                            iconBorder.color = inGameColor.Value;
                        }
                    }
                    else
                    {
                        statusLabel.text = "Buisy";
                        iconBorder.color = buisyColor.Value;
                    }
                    break;
                case Steamworks.EPersonaState.k_EPersonaStateLookingToPlay:
                    statusLabel.text = "Looking to Play";
                    iconBorder.color = wantPlayColor.Value;
                    break;
                case Steamworks.EPersonaState.k_EPersonaStateLookingToTrade:
                    statusLabel.text = "Looking to Trade";
                    iconBorder.color = wantTradeColor.Value;
                    break;
                case Steamworks.EPersonaState.k_EPersonaStateOffline:
                    statusLabel.text = "Offline";
                    iconBorder.color = offlineColor.Value;
                    break;
                case Steamworks.EPersonaState.k_EPersonaStateOnline:
                    if (userData.InGame)
                    {
                        if (userData.GameInfo.m_gameID.AppID().m_AppId == SteamSettings.current.applicationId.m_AppId)
                        {
                            statusLabel.text = "Playing";
                            iconBorder.color = thisGameColor.Value;
                        }
                        else
                        {
                            statusLabel.text = "In-Game";
                            iconBorder.color = inGameColor.Value;
                        }
                    }
                    else
                    {
                        statusLabel.text = "Online";
                        iconBorder.color = onlineColor.Value;
                    }
                    break;
                case Steamworks.EPersonaState.k_EPersonaStateSnooze:
                    if (userData.InGame)
                    {
                        if (userData.GameInfo.m_gameID.AppID().m_AppId == SteamSettings.current.applicationId.m_AppId)
                        {
                            statusLabel.text = "Playing";
                            iconBorder.color = thisGameColor.Value;
                        }
                        else
                        {
                            statusLabel.text = "In-Game";
                            iconBorder.color = inGameColor.Value;
                        }
                    }
                    else
                    {
                        statusLabel.text = "Snooze";
                        iconBorder.color = snoozeColor.Value;
                    }
                    break;
            }
            if (colorTheStatusLabel)
                statusLabel.color = iconBorder.color;
            if (colorThePersonaName)
                personaName.color = iconBorder.color;
        }

        private void OnDestroy()
        {
            if (userData != null)
                userData.evtAvatarChanged.RemoveListener(HandleAvatarChange);
        }
    }
}
#endif