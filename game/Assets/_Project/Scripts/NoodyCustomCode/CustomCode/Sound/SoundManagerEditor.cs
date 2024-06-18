using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NOOD.Sound;

namespace NOOD.NoodCustomEditor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(SoundDataSO))]
    public class SoundManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SoundDataSO soundManager = (SoundDataSO)target;
            if (GUILayout.Button("Generate Enum"))
            {
                soundManager.GenerateSoundEnum();
            }
        }
    }
#endif
}
