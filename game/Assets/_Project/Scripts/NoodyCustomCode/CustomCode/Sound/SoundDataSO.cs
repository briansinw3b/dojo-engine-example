using System.Linq;
using UnityEngine;
using NOOD.SerializableDictionary;
using NOOD.Extension;
using UnityEditor;
using NOOD.Sound;
using System.IO;
using System;

namespace NOOD.Sound
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "SoundData")]
    public class SoundDataSO : ScriptableObject
    {
        [SerializeField] public SerializableDictionary<string, AudioClip> soundDic = new SerializableDictionary<string, AudioClip>();
        [SerializeField] public SerializableDictionary<string, AudioClip> musicDic = new SerializableDictionary<string, AudioClip>();

    #if UNITY_EDITOR
        public void GenerateSoundEnum()
        {
            // string folderPath = Application.dataPath + "/_Scripts/Noody/Extension/";
            // EnumCreator.WriteToEnum<SoundEnum>(folderPath, "SoundEnum", soundDic.Dictionary.Keys.ToList());
            // EnumCreator.WriteToEnum<MusicEnum>(folderPath, "MusicEnum", musicDic.Dictionary.Keys.ToList());
            GenerateSoundEnumNood();
            AssetDatabase.Refresh();
        }
        private void GenerateSoundEnumNood()
        {
            string folderPath = RootPathExtension<SoundEnum>.FolderPath(".cs");
            Debug.Log(folderPath);
            EnumCreator.WriteToEnum<SoundEnum>(folderPath, "SoundEnum", soundDic.Dictionary.Keys.ToList(), "NOOD.Sound");
            EnumCreator.WriteToEnum<MusicEnum>(folderPath, "MusicEnum", musicDic.Dictionary.Keys.ToList(), "NOOD.Sound");
        }
    #endif
    }

}
