#if !DISABLESTEAMWORKS
using UnityEngine;

namespace HeathenEngineering.SteamAPI.Demo
{
    public class ToggleInvertActive : MonoBehaviour
    {
        public UnityEngine.UI.Toggle toggle;
        public GameObject icon;

        private void Start()
        {
            icon.SetActive(!toggle.isOn);
            toggle.onValueChanged.AddListener(handleChange);
        }

        private void handleChange(bool arg0)
        {
            icon.SetActive(!arg0);
        }
    }
}
#endif