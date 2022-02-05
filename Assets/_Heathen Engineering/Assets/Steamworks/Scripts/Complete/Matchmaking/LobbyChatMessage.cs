#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE && !UNITY_SERVER
using HeathenEngineering.SteamAPI.UI;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HeathenEngineering.SteamAPI
{
    public class LobbyChatMessage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public SteamUserIconButton PersonaButton;
        public UnityEngine.UI.Text Message;
        public DateTime timeStamp;
        public UnityEngine.UI.Text timeRecieved;
        public bool ShowStamp = true;
        public bool AllwaysShowStamp = false;

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


        public void OnPointerEnter(PointerEventData eventData)
        {
            if (ShowStamp && !timeRecieved.gameObject.activeSelf)
                timeRecieved.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(!AllwaysShowStamp && timeRecieved.gameObject.activeSelf)
                timeRecieved.gameObject.SetActive(false);
        }
    }
}
#endif
