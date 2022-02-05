#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES
using System;
using UnityEngine;

namespace HeathenEngineering.SteamAPI.Demo
{
    [CreateAssetMenu(menuName = "Steamworks/Examples/Data Model")]
    public class ExampleNewDataModel : DataModel<ExampleCustomDataModel> { }

    [Serializable]
    public class ExampleCustomDataModel
    {
        public string exampleString;
        public bool exampleBool;
        public float exampleFloat;
        public Serializable.SerializableTransform exampleTransform;
    }
}
#endif