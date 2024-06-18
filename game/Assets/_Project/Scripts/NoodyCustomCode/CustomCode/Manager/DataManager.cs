using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using NOOD.Extension;

namespace NOOD.Data
{
    public class DataManager<T> where T : new()
    {
        private static T data;

        // Get Instance of data

        /// <summary>
        /// Only return data if data is save to PlayerPrefs
        /// </summary>
        /// <value></value>
        public static T Data
        {
            get
            {
                if (data == null)
                {
                    QuickLoad();
                }
                return data;
            }
        }

        #region LoadData
        /// <summary>
        /// Use property Data to retreat data 
        /// </summary> 
        private static void QuickLoad()
        {
            Debug.Log("QuickLoad");
            if (PlayerPrefs.HasKey(typeof(T).Name))
            {
                Debug.Log("QuickLoad Has Key");
                data = LoadDataFromPlayerPref(typeof(T).Name);
            }

            if (data == null)
            {
                data = new T();
                QuickSave();
            }
        }
        /// <summary>
        /// You must have disk access right to use this function
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="extension"></param>
        /// <returns></returns> 
        public static T LoadDataFromFile(string filePath, string extension)
        {
            string jsonStr = FileExtension.ReadFile(filePath, extension);
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }
        /// <summary>
        /// Load the data from Resources/Datas
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static T LoadDataFromDefaultFolder(string fileName)
        {
            if (FileExtension.IsExitFileInDefaultFolder(fileName))
            {
                string jsonStr = Resources.Load<TextAsset>(Path.Combine("Datas", fileName)).text;
                return JsonConvert.DeserializeObject<T>(jsonStr);
            }
            Debug.LogWarning("Not exist file name " + fileName + " in default folder");
            return default;
        }
        /// <summary>
        /// Return the value base on key in PlayerPref, use this for data written on build device
        /// </summary>
        /// <param name="keyName">
        /// <returns></returns>
        public static T LoadDataFromPlayerPref(string keyName)
        {
            string jsonStr = PlayerPrefs.GetString(keyName);
            // Debug.Log(jsonStr);
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }
        #endregion

        #region SaveData
        /// <summary>
        /// Save to PlayerPrefs, this will save to PlayerPrefs by default
        /// </summary>
        public static void QuickSave()
        {
            // Save file to Resources/fileName
            SaveToPlayerPref(data);
        }
#if UNITY_EDITOR
        /// <summary>
        /// You must have disk access right to use this function
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data"></param> 
        public static void SaveToFile(string filePath, T data)
        {
            string jsonString = JsonConvert.SerializeObject(data);
            FileExtension.WriteToFile(filePath, jsonString);
        }
        /// <summary>
        /// Save to Assets/Resources/Datas folder, do not use this function to save data on device
        /// </summary>
        /// <param name="data"></param>
        public static void SaveToDefaultFolder(T data, string fileName, string extension)
        {
            string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
            string path = Path.Combine(Application.dataPath, "Resources");
            string fleName = fileName;
            string finalPath = Path.Combine(path, "Datas", fleName + extension);
            Debug.Log(path);
            FileExtension.WriteToFile(finalPath, jsonString);
        }
#endif
        /// <summary>
        /// Save the data to PlayerPref, this data can be written on build device
        /// </summary>
        /// <param name="data"> the data want to save </param>
        public static void SaveToPlayerPref(T data)
        {
            string jsonStr = JsonUtility.ToJson(data);
            Debug.Log(jsonStr);
            PlayerPrefs.SetString(typeof(T).Name, jsonStr);
            PlayerPrefs.Save();
        }
        #endregion

        /// <summary>
        /// Only work if data is saved with QuickSave()  
        /// </summary>
        public static void QuickClear()
        {
            QuickLoad();
            data = default;
            QuickSave();
        }
    }
}
