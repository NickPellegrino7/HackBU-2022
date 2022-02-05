#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES
using HeathenEngineering.SteamAPI.UI;
using UnityEditor;
using UnityEditor.UI;

namespace HeathenEngineering.SteamAPI.Editors
{
    [CustomEditor(typeof(SteamDataFileRecord))]
    public class SteamDataFileRecordEditor : ButtonEditor
    {
        //private SteamDataFileRecord record;
        private SerializedProperty FileName;
        private SerializedProperty Timestamp;
        private SerializedProperty SelectedIndicator;

        protected override void OnEnable()
        {
            base.OnEnable();

            FileName = serializedObject.FindProperty("FileName");
            Timestamp = serializedObject.FindProperty("Timestamp");
            SelectedIndicator = serializedObject.FindProperty("SelectedIndicator");
        }

        public override void OnInspectorGUI()
        {
            //record = target as SteamDataFileRecord;

            EditorGUILayout.PropertyField(FileName);
            EditorGUILayout.PropertyField(Timestamp);
            EditorGUILayout.PropertyField(SelectedIndicator);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Button Settings", EditorStyles.boldLabel);
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
#endif