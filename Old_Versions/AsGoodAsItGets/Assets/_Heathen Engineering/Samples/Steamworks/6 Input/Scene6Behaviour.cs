#if HE_STEAMCOMPLETE

using UnityEngine;
using HeathenEngineering.SteamworksIntegration;
using System.Collections.Generic;

namespace HeathenEngineering.DEMO
{
    /// <summary>
    /// This is for demonstration purposes only
    /// </summary>
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    public class Scene6Behaviour : MonoBehaviour
    {
        [Header("UI References")]
        public TMPro.TextMeshProUGUI label;

        [Header("Input Action Sets")]
        public InputActionSet menuActionSet;
        public InputActionSet shipActionSet;

        [Header("Input Action Set Layers")]
        public InputActionSetLayer thustLayer;

        [Header("Input Actions")]
        public InputAction analogAction;
        public InputAction leftAction;
        public InputAction rightAction;
        public InputAction forwardAction;
        public InputAction backwardAction;
        public InputAction fireAction;
        public InputAction pauseAction;
        public InputAction menuUpAction;
        public InputAction menuDownAction;
        public InputAction menuLeftAction;
        public InputAction menuRightAction;
        public InputAction selectAction;
        public InputAction cancelAction;

        private InputActionData analogData;
        private InputActionData leftData;
        private InputActionData rightData;
        private InputActionData forwardData;
        private InputActionData backwardData;
        private InputActionData fireData;
        private InputActionData pauseData;
        private InputActionData menuUpData;
        private InputActionData menuDownData;
        private InputActionData menuLeftData;
        private InputActionData menurightData;
        private InputActionData selectData;
        private InputActionData cancelData;

        [Header("Glyph Data")]
        public List<InputActionGlyph> glyphs = new List<InputActionGlyph>();
        public List<UGUIInputActionName> names = new List<UGUIInputActionName>();

        private Steamworks.InputHandle_t[] controllers;
        private bool hackRefresh = false;

        private void Start()
        {
            //This is only needed when running in editor/debugging it forces the input API to use a specifc App's bindings
            Application.OpenURL("steam://forceinputappid/480");
            //Run the frame after the force input to get the new data
            SteamworksIntegration.API.Input.Client.RunFrame();

            controllers = SteamworksIntegration.API.Input.Client.ConnectedControllers;

            if (controllers.Length > 0)
            {
                shipActionSet.Activate(controllers[0]);
                thustLayer.Activate(controllers[0]);

                Invoke(nameof(DelayActivate), 1);

                Debug.Log("Steam Input initialized:\n\tControllers Found = " + controllers.Length);
            }
            else
            {
                Debug.LogWarning("Steam Input initialized:\n\tNo controllers found!");
            }
        }

        private void OnDestroy()
        {
            //This is only needed when running in editor/debugging it clears the forced App ID for the Input API
            Application.OpenURL("steam://forceinputappid/0");
        }

        private void Update()
        {
            if (controllers != null && controllers.Length > 0)
            {
                if(!hackRefresh)
                {
                    hackRefresh = true;
                    DelayActivate();
                }

                SteamSettings.Client.UpdateAllActions(controllers[0]);

                analogData = analogAction[controllers[0]];

                leftData = leftAction[controllers[0]];
                rightData = rightAction[controllers[0]];
                forwardData = forwardAction[controllers[0]];
                backwardData = backwardAction[controllers[0]];

                fireData = fireAction[controllers[0]];
                pauseData = pauseAction[controllers[0]];

                menuUpData = menuUpAction[controllers[0]];
                menuDownData = menuDownAction[controllers[0]];
                menuLeftData = menuLeftAction[controllers[0]];
                menurightData = menuRightAction[controllers[0]];
                selectData = selectAction[controllers[0]];
                cancelData = cancelAction[controllers[0]];

                label.text = "Analog Action: " + analogData.ToString() + "\nLeft Action: " + leftData.ToString() + "\nRight Action: " + rightData.ToString() + "\nForward Action: " + forwardData.ToString() + "\nBackward Action: " + backwardData.ToString() + "\nFire Action: " + fireData.ToString() + "\nPause Action: " + pauseData.ToString() + "\nMenu Up Action: " + menuUpData.ToString() + "\nMenu Down Action: " + menuDownData.ToString() + "\nMenu Right Action: " + menurightData.ToString() + "\nMenu Left Action: " + menuLeftData.ToString() + "\nMenu Select Action: " + selectData.ToString() + "\nCancel Action: " + cancelData.ToString();
            }
            else
                label.text = "No Controllers found";
        }

        private void DelayActivate()
        {
            //Because we have to force the App ID in Unity Editor we need to force a refresh after that
            foreach (var glyph in glyphs)
                glyph.RefreshImage();
            foreach (var iName in names)
                iName.RefreshName();
        }

        public void ActivateMenuControls()
        {
            menuActionSet.Activate(controllers[0]);
        }

        public void ActivateShipControls()
        {
            shipActionSet.Activate(controllers[0]);
            thustLayer.Activate(controllers[0]);
        }

        public void OpenKnowledgeBaseUserData()
        {
            Application.OpenURL("https://kb.heathenengineering.com/assets/steamworks");
        }
    }
}
#endif