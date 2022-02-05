#if !DISABLESTEAMWORKS
using UnityEngine;

namespace HeathenEngineering.SteamAPI.UI
{
    /// <summary>
    /// <para>Used to display the avatar image of a given <see cref="SteamUserData"/> object.</para>
    /// </summary>
    [RequireComponent(typeof(UnityEngine.UI.RawImage))]
    public class AvatarRawImage : MonoBehaviour
    {
        /// <summary>
        /// The image to load the avatar into.
        /// </summary>
        private UnityEngine.UI.RawImage image;
        /// <summary>
        /// The <see cref="UserData"/> to load.
        /// This should be set by calling <see cref="AvatarRawImage.LinkSteamUser(UserData)"/>
        /// </summary>
        public UserData userData;

        private void Awake()
        {
            image = GetComponent<UnityEngine.UI.RawImage>();

            LinkSteamUser(userData);
        }

        /// <summary>
        /// Sets and registeres for the provided <see cref="SteamUserData"/> object.
        /// </summary>
        /// <param name="newUserData">The user to connect to and to display the avatar for.</param>
        /// <example>
        /// <list type="bullet">
        /// <item>
        /// <description>Set the icon to display the current user as read from the SteamSettings settings member.</description>
        /// <code>
        /// myAvatarRawImage.LinkSteamUser(settings.UserData);
        /// </code>
        /// </item>
        /// </list>
        /// </example>
        public void LinkSteamUser(UserData newUserData)
        {
            if (userData != null)
                userData.evtAvatarChanged.RemoveListener(handleAvatarChange);

            userData = newUserData;

            if (userData != null)
            {
                if(image == null)
                    image = GetComponent<UnityEngine.UI.RawImage>();

                image.texture = userData.avatar;
                userData.evtAvatarChanged.AddListener(handleAvatarChange);
            }
        }

        private void handleAvatarChange()
        {
            image.texture = userData.avatar;
        }

        private void OnDestroy()
        {
            if (userData != null)
                userData.evtAvatarChanged.RemoveListener(handleAvatarChange);
        }
    }
}
#endif