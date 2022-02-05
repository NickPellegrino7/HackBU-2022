#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES

using Steamworks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static HeathenEngineering.SteamAPI.RemoteStorageSystem;

namespace HeathenEngineering.SteamAPI.UI
{
    /// <summary>
    /// Displays a list of <see cref="RemoteStorageDataFile"/> entries related to a specific <see cref="FileDataModel"/>.
    /// </summary>
    public class SteamDataFileList : MonoBehaviour
    {
        /// <summary>
        /// <para>The library the list will map files for.</para>   
        /// <para>Note that this list will sort and display only files related to this library as determined by the prefix defined in the library.</para>
        /// </summary>
        public DataModel dataModel;
        /// <summary>
        /// <para>The prefab used to display <see cref="RemoteStorageDataFile"/> records to the UI</para>
        /// </summary>
        public SteamDataFileRecord recordPrefab;
        /// <summary>
        /// The container which newly spawned instances of the <see cref="SteamDataFileList.recordPrefab"/> will be parented to.
        /// </summary>
        public RectTransform container;
        /// <summary>
        /// The display format of the files date time ... see C# .NET DateTime ToString formating options for more information.
        /// </summary>
        public StringReference dateDisplayFormat = new StringReference("F");
        /// <summary>
        /// This event is raised when a new <see cref="RemoteStorageDataFile"/> object is selected in the UI.
        /// </summary>
        [Header("Events")]
        public UnityEvent selectionChanged;
        /// <summary>
        /// A pointer to the currently selected <see cref="RemoteStorageDataFile"/>.
        /// </summary>
        public RemoteStorageDataFile Active
        {
            get
            {
                return dataModel.activeFile;
            }
        }
        private FileAddress? s_SelectedFile;
        /// <summary>
        /// The address if any of the currenltly selected <see cref="RemoteStorageDataFile"/> object.
        /// </summary>
        public FileAddress? SelectedFile
        {
            get
            {
                return s_SelectedFile;
            }
            set
            {
                if(s_SelectedFile.HasValue && value.HasValue)
                {
                    if (s_SelectedFile.Value != value.Value)
                    {
                        s_SelectedFile = value;
                        selectionChanged.Invoke();
                    }
                }
                else if (s_SelectedFile.HasValue != value.HasValue)
                {
                    s_SelectedFile = value;
                    selectionChanged.Invoke();
                }
            }
        }

        private void OnEnable()
        {
            Refresh();
        }

        /// <summary>
        /// Updates the list from the library values sorted on the time stamp of the record
        /// </summary>
        public void Refresh()
        {
            RefreshFileList();
            var temp = new List<GameObject>();
            foreach(Transform child in container)
            {
                temp.Add(child.gameObject);
            }

            while(temp.Count > 0)
            {
                var t = temp[0];
                temp.Remove(t);
                Destroy(t);
            }

            dataModel.availableFiles.Sort((p1, p2) => { return p1.UtcTimestamp.CompareTo(p2.UtcTimestamp); });
            dataModel.availableFiles.Reverse();

            foreach (var address in dataModel.availableFiles)
            {
                var go = Instantiate(recordPrefab.gameObject, container);
                var r = go.GetComponent<SteamDataFileRecord>();
                r.parentList = this;
                r.Address = address;
                if (address.fileName.EndsWith(dataModel.extension))
                    r.FileName.text = address.fileName.Substring(0, address.fileName.Length - dataModel.extension.Length);
                else
                    r.FileName.text = address.fileName;

                r.Timestamp.text = address.LocalTimestamp.ToString(dateDisplayFormat, Thread.CurrentThread.CurrentCulture);
            }
        }

        /// <summary>
        /// Returns the address if any of the most resent file saved.
        /// </summary>
        /// <returns></returns>
        public FileAddress? GetLatest()
        {
            if (dataModel.availableFiles.Count > 0)
                return dataModel.availableFiles[0];
            else
                return null;
        }

        /// <summary>
        /// Clears the selected file pointer.
        /// </summary>
        public void ClearSelected()
        {
            SelectedFile = null;
        }

        /// <summary>
        /// Selectes a specific file by address
        /// </summary>
        /// <param name="address"></param>
        public void Select(FileAddress address)
        {
            SelectedFile = address;
        }

        /// <summary>
        /// Selects the most resent file.
        /// </summary>
        public void SelectLatest()
        {
            SelectedFile = GetLatest();
        }

        /// <summary>
        /// Loads the selected file ... this will deserialize the data of the file and populate the fileds of the related <see cref="FileDataModel"/>
        /// </summary>
        public void LoadSelected()
        {
            if (SelectedFile.HasValue)
                dataModel.LoadFileAddress(SelectedFile.Value);
        }

        /// <summary>
        /// Loads the selected file ... this will deserialize the data of the file and populate the fileds of the related <see cref="FileDataModel"/>
        /// </summary>
        public void LoadSelectedAsync()
        {
            if (SelectedFile.HasValue)
                dataModel.LoadFileAddressAsync(SelectedFile.Value);
        }

        /// <summary>
        /// Removes the selected file from the Steamworks Remote Storage system
        /// </summary>
        public void DeleteSelected()
        {
            if (SelectedFile.HasValue)
                FileDelete(SelectedFile.Value);

            Refresh();
        }

        /// <summary>
        /// Instructs the Steamworks Remote Storage system to 'forget' the file e.g. to purge it from the cloud and no longer sync it.
        /// </summary>
        public void ForgetSelected()
        {
            if (SelectedFile.HasValue)
                SteamRemoteStorage.FileForget(SelectedFile.Value.fileName);
        }

        /// <summary>
        /// Saves the selected file if any... This will attempt to serialize the related libraries data and store it to the Steamworks Remote Storage system to the address of the currently selected file.
        /// </summary>
        public void SaveActive()
        {
            if (SelectedFile.HasValue)
            {
                dataModel.Save(SelectedFile.Value.fileName);
                Refresh();
            }
            else
            {
                Debug.LogWarning("[SteamDataFileList.SaveActive] Attempted to save the active file but no file is active.");
            }
        }

        /// <summary>
        /// This saves the data of the related library to the Steamworks Remote Storage system with the indicated name... Note that the prifix as defined in the library will be added if missing.
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveAs(string fileName)
        {
            dataModel.Save(fileName);
            Refresh();

            SelectLatest();
        }

        /// <summary>
        /// This saves the data of the related library to the Steamworks Remote Storage system with the indicated name... Note that the prifix as defined in the library will be added if missing.
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveAs(InputField fileName)
        {
            if (fileName == null || string.IsNullOrEmpty(fileName.text))
            {
                Debug.LogWarning("[SteamDataFileList.SaveAs] Attempted to SaveAs but was not provided with a file name ... will attempt to save the active file instead.");
                SaveActive();
            }
            else
            {
                dataModel.Save(fileName.text);
                Refresh();

                SelectLatest();
            }
        }

        /// <summary>
        /// Saves the selected file if any... This will attempt to serialize the related libraries data and store it to the Steamworks Remote Storage system to the address of the currently selected file.
        /// </summary>
        public void SaveActiveAsync()
        {
            if (SelectedFile.HasValue)
                dataModel.SaveAsync(SelectedFile.Value.fileName);
        }

        /// <summary>
        /// This saves the data of the related library to the Steamworks Remote Storage system with the indicated name... Note that the prifix as defined in the library will be added if missing.
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveAsAsync(string fileName)
        {
            dataModel.SaveAsync(fileName);

            string fName = fileName.EndsWith(dataModel.extension) ? fileName : fileName + dataModel.extension;

            if (dataModel.availableFiles.Exists(p => p.fileName == fName))
                SelectedFile = dataModel.availableFiles.First(p => p.fileName == fName);
        }

        /// <summary>
        /// This saves the data of the related library to the Steamworks Remote Storage system with the indicated name... Note that the prifix as defined in the library will be added if missing.
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveAsAsync(InputField fileName)
        {
            dataModel.SaveAsync(fileName.text);

            string fName = fileName.text.EndsWith(dataModel.extension) ? fileName.text : fileName.text + dataModel.extension;

            if (dataModel.availableFiles.Exists(p => p.fileName == fName))
                SelectedFile = dataModel.availableFiles.First(p => p.fileName == fName);
        }
    }
}
#endif