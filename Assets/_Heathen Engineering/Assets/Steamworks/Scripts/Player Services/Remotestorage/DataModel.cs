#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace HeathenEngineering.SteamAPI
{
    /// <summary>
    /// Abstract structure for game data models.
    /// </summary>
    /// <remarks>
    /// See <see cref="DataModel{T}"/> for more informaiton on the usage of <see cref="DataModel"/>
    /// </remarks>
    public abstract class DataModel : ScriptableObject
    {
        /// <summary>
        /// The extension assoceated with this model
        /// </summary>
        /// <remarks>
        /// When loading file addresses from Valve's backend the system will check if the address ends with this string.
        /// Note this test ignores case.
        /// When writing data to Valve's backend from this model the system will check for and append this extension if required
        /// </remarks>
        public string extension;

        public UnityEvent dataUpdated;
        
        [NonSerialized]
        public RemoteStorageDataFile activeFile;
        [NonSerialized]
        public List<RemoteStorageSystem.FileAddress> availableFiles = new List<RemoteStorageSystem.FileAddress>();

        /// <summary>
        /// Gets the base type of the data stored by this model
        /// </summary>
        public abstract Type DataType { get; }

        public abstract void LoadByteArray(byte[] data);

        public abstract void LoadJson(string json);

        public abstract void LoadFileAddress(RemoteStorageSystem.FileAddress addresss);

        public abstract void LoadFileAddress(string addresss);

        public abstract void LoadFileAddressAsync(RemoteStorageSystem.FileAddress addresss, Action<bool> callback = null);

        public abstract void LoadFileAddressAsync(string addresss, Action<bool> callback = null);

        public abstract byte[] ToByteArray();

        public abstract string ToJson();

        public abstract void Save(string filename);

        public abstract void SaveAsync(string filename, Action<bool> callback = null);
    }

    /// <summary>
    /// Used to create data models suitable for save and load operations via <see cref="RemoteStorageSystem"/>
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is an abstract class and must be inherited from to create a unique data model class suitable for your game.
    /// The use is similar to that of UnityEngine's UnityEvent&lt;T&gt; in that you can you create any data structure you like as a class or struct assuming of course that it is marked as [Serializable].
    /// You can then declare a class and derive from <see cref="DataModel{T}"/> as demonstrated below. 
    /// Note that <see cref="DataModel"/> is derived from Unity's ScriptableObject allowing you to create your data model object as an asset in your project
    /// </para>
    /// <code>
    /// [Serializable]
    /// public class MyCharacterData
    /// {
    ///     public string characterName;
    ///     public int level;
    ///     public Serializable.SerializableVector3 position;
    ///     public Serializable.SerializableQuaternion rotation;
    /// }
    ///
    /// [CreateAssetMenu(menuName = "My Objects/Character Data Model")]
    /// public class CharacterDataModel : DataModel&lt;MyCharacterData&gt; { }
    /// </code>
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public abstract class DataModel<T> : DataModel
    {
        /// <summary>
        /// The currently loaded data related to this model
        /// </summary>
        /// <remarks>
        /// The type provided must be serializable by Unity's JsonUtility
        /// </remarks>
        public T data;

        public override Type DataType => typeof(T);

        /// <summary>
        /// Stores <paramref name="data"/> to the <see cref="data"/> member
        /// </summary>
        /// <param name="data">The UTF8 encoded bytes of JSON represening this object</param>
        public override void LoadByteArray(byte[] data)
        {
            this.data = JsonUtility.FromJson<T>(Encoding.UTF8.GetString(data));
            dataUpdated.Invoke();
        }

        /// <summary>
        /// Stores the <paramref name="json"/> string to the <see cref="data"/> member
        /// </summary>
        /// <param name="json">The JSON formated string containing the data of this object</param>
        public override void LoadJson(string json)
        {
            data = JsonUtility.FromJson<T>(json);
            dataUpdated.Invoke();
        }

        /// <summary>
        /// Returns a JSON formated string representing the <see cref="data"/> member
        /// </summary>
        /// <returns>JSON formated string of the <see cref="data"/> member</returns>
        public override string ToJson()
        {
            return JsonUtility.ToJson(data);
        }

        /// <summary>
        /// Returns the UTF8 encoded bytes of the JSON representation of the <see cref="data"/> member
        /// </summary>
        /// <returns></returns>
        public override byte[] ToByteArray()
        {
            return Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
        }

        /// <summary>
        /// Starts an asynchronious save operation for the <see cref="data"/> of this object
        /// </summary>
        /// <remarks>
        /// You can monitor <see cref="RemoteStorageSystem.evtFileWriteAsyncComplete"/> to know when this operation has completed.
        /// This will add <see cref="DataModel.extension"/> to the end of the name if required.
        /// </remarks>
        /// <param name="filename"></param>
        public override void SaveAsync(string filename, Action<bool> callback = null)
        {
            RemoteStorageDataFile dataFile;
            if (filename.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                dataFile = RemoteStorageSystem.FileWriteAsync(filename, ToByteArray());
            else
                dataFile = RemoteStorageSystem.FileWriteAsync(filename + extension, ToByteArray());

            if (callback != null)
            {
                dataFile.Complete = (r) =>
                {
                    callback.Invoke(r.result == global::Steamworks.EResult.k_EResultOK);
                };
            }
        }

        /// <summary>
        /// Saves the <see cref="data"/> member of this object with the name provided
        /// </summary>
        /// <remarks>
        /// This will add <see cref="DataModel.extension"/> to the end of the name if required.
        /// </remarks>
        /// <param name="filename"></param>
        public override void Save(string filename)
        {
            if (filename.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                RemoteStorageSystem.FileWrite(filename, ToByteArray());
            else
                RemoteStorageSystem.FileWrite(filename + extension, ToByteArray());
        }

        /// <summary>
        /// Loads data from the address provided into <see cref="data"/>
        /// </summary>
        /// <param name="addresss">The address to load from</param>
        public override void LoadFileAddress(RemoteStorageSystem.FileAddress addresss)
        {
            data = RemoteStorageSystem.FileReadJson<T>(addresss, Encoding.UTF8);
            dataUpdated.Invoke();
        }

        /// <summary>
        /// Loads data from the address provided into <see cref="data"/>
        /// </summary>
        /// <param name="addresss">The address to load from</param>
        /// <param name="callback">An action to invoke when the process is complete, this can be null</param>
        public override void LoadFileAddressAsync(RemoteStorageSystem.FileAddress addresss, Action<bool> callback = null)
        {
            var dataFile = RemoteStorageSystem.FileReadAsync(addresss);
            dataFile.dataModel = this;

            if (callback != null)
            {
                dataFile.Complete = (r) =>
                {
                    callback.Invoke(r.result == global::Steamworks.EResult.k_EResultOK);
                };
            }
        }

        /// <summary>
        /// Loads data from the address provided into <see cref="data"/>
        /// </summary>
        /// <param name="addresss">The address to load from</param>
        public override void LoadFileAddress(string addresss)
        {
            data = RemoteStorageSystem.FileReadJson<T>(addresss, Encoding.UTF8);
            dataUpdated.Invoke();
        }

        /// <summary>
        /// Loads data from the address provided into <see cref="data"/>
        /// </summary>
        /// <param name="addresss">The address to load from</param>
        /// <param name="callback">An action to invoke when the process is complete, this can be null</param>
        public override void LoadFileAddressAsync(string addresss, Action<bool> callback = null)
        {
            var dataFile = RemoteStorageSystem.FileReadAsync(addresss);
            dataFile.dataModel = this;

            if (callback != null)
            {
                dataFile.Complete = (r) =>
                {
                    callback.Invoke(r.result == global::Steamworks.EResult.k_EResultOK);
                };
            }
        }
    }
}
#endif
