#if !DISABLESTEAMWORKS
using UnityEngine;

namespace HeathenEngineering.SteamAPI.Demo
{
    public class ConcatinateString : MonoBehaviour
    {
        public UnityEngine.UI.Text output;
        public UnityEngine.UI.InputField input;

        public void Concat()
        {
            output.text += "\n" + input.text;
        }
    }
}
#endif
