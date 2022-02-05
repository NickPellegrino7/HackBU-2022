#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES
using HeathenEngineering.SteamAPI.UI;
using UnityEngine;
using UnityEngine.UI;

namespace HeathenEngineering.SteamAPI.Demo
{
    public class DataModelEditorExample : MonoBehaviour
    {
        public ExampleNewDataModel dataModel;
        public SteamDataFileList displayList;
        public InputField exampleString;
        public Slider exampleFloat;
        public Toggle exampleBool;
        public Transform copyTransformValues;

        public void StringEndEdit(string value)
        {
            dataModel.data.exampleString = value;
        }

        public void SliderValueChanged(float value)
        {
            dataModel.data.exampleFloat = value;
        }

        public void ToggleValueChanged(bool value)
        {
            dataModel.data.exampleBool = value;
        }

        public void SelectedFileChanged()
        {
            if (displayList.SelectedFile.HasValue)
                dataModel.LoadFileAddress(displayList.SelectedFile.Value);
        }

        private void Start()
        {
            dataModel.dataUpdated.AddListener(HandleDataLoaded);
        }

        private void HandleDataLoaded()
        {
            if (dataModel == null || dataModel.data == null)
            {
                exampleString.text = string.Empty;
                exampleFloat.value = 0;
                exampleBool.isOn = false;
                copyTransformValues.position = new Vector3(0, -0.74f, -5);
                copyTransformValues.rotation = Quaternion.identity;
                copyTransformValues.localScale = Vector3.one;
            }
            else
            {
                exampleString.text = dataModel.data.exampleString;
                exampleFloat.value = dataModel.data.exampleFloat;
                exampleBool.isOn = dataModel.data.exampleBool;
                copyTransformValues.position = dataModel.data.exampleTransform.position;
                copyTransformValues.rotation = new Quaternion(dataModel.data.exampleTransform.rotation.x, dataModel.data.exampleTransform.rotation.y, dataModel.data.exampleTransform.rotation.z, dataModel.data.exampleTransform.rotation.w);
                copyTransformValues.localScale = dataModel.data.exampleTransform.localScale;
            }
        }

        private void Update()
        {
            if (dataModel != null && dataModel.data != null)
                dataModel.data.exampleTransform = copyTransformValues;
        }
    }
}
#endif