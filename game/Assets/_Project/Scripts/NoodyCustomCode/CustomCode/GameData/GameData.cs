using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using NOOD.Extension;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NOOD
{
    [CreateAssetMenu(fileName = "GameData", menuName = "NOOD/GameData", order = 1)]
    public class GameData : SerializedScriptableObject
    {
        [DictionaryDrawerSettings(ValueLabel = "Object and Path")]
        public Dictionary<string, PrefabPathClass> prefabPathDictionary = new Dictionary<string, PrefabPathClass>();

        #region Instance
        private static GameData _instance;
        public static GameData Instance 
        {
            get
            {
                if(_instance == null)
                {
                    GameData[] gameDataArray = Resources.LoadAll<GameData>("");
                    if(gameDataArray != null && gameDataArray.Count() > 0)
                    {
                        _instance = gameDataArray[0];
                        return _instance;
                    }
                    else
                    {
                        Debug.LogError("GameData didn't exist, please create one in Resources folder using Create -> GameData");
                        return null;
                    }
                }
                else
                {
                    return _instance;
                }
            }
        }

        #endregion

        #region Prefabs functions
        public GameObject InstantiatePrefab(PrefabEnum prefabEnum)
        {
            return Instantiate(prefabPathDictionary[prefabEnum.ToString()].gameObject);
        }
        public string GetPrefabPath(PrefabEnum prefabEnum)
        {
            return prefabPathDictionary[prefabEnum.ToString()].pathInResources;
        }
        #endregion

#if UNITY_EDITOR
        private int _dictionaryCountOld;

        private void Awake() 
        {
            _dictionaryCountOld = prefabPathDictionary.Count;     
        }

        #region Update Asset Path
        [ShowIf("CheckNeedRefresh")]
        [GUIColor("red")]
        [Button("Refresh Data", ButtonSizes.Large)]
        // [OnStateUpdate("CheckNeedRefresh")]
        private void UpdateAssetPath()
        {
            foreach(PrefabPathClass prefabPathClass in prefabPathDictionary.Values)
            {
                string path = AssetDatabase.GetAssetPath(prefabPathClass.gameObject);
                string resourcesPath = path.Split("Resources/").Last();
                resourcesPath = resourcesPath.Replace(".prefab", "");
                prefabPathClass.pathInResources = resourcesPath;
            }

            string folderPath = RootPathExtension<GameData>.FolderPath(".cs");
            EnumCreator.WriteToEnum<PrefabEnum>(folderPath, "PrefabEnum", prefabPathDictionary.Keys.ToList(), "NOOD");
            _dictionaryCountOld = prefabPathDictionary.Count;
        }
        private bool CheckNeedRefresh()
        {
            return prefabPathDictionary.Values.Any(x => string.IsNullOrEmpty(x.pathInResources)) || _dictionaryCountOld != prefabPathDictionary.Count;
        }
        #endregion
#endif
    }

    [System.Serializable]
    [InlineProperty()]
    public class PrefabPathClass
    {
        [AssetsOnly]
        public GameObject gameObject;
        [ReadOnly]
        public string pathInResources;
    }

    public enum PlateType
    {
        DirtyPlate,
        CleanPlate
    }
}
