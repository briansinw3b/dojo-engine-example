using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using NOOD.UI;

namespace NOOD.NoodCustomEditor
{
#if UNITY_EDITOR
    public class ClassCreatorEditor : EditorWindow
    {
        const string PATH_TO_TEMPLATE = "Scripts/Editor/Template/";
        const string PREFAB_QUEUE = "prefabInQueue";

        Vector2 scrollPosition = Vector2.zero;

        int selectedIndex;
        int viewTypeIndex;

        string assetDirectory = "";
        string nameSpace = "";
        string baseName = "";
        bool isUI = false;
        TemplateUsingParams usingParams = new TemplateUsingParams();

        string viewAssetPath => assetDirectory + usingParams.ClassName + ".cs";


        public static ClassCreatorEditor CreateWindow(string currentDirectory, bool isUI)
        {
            string[] directories = currentDirectory.Replace("Scripts/", "").Split('/');

            EditorPrefs.DeleteKey(PREFAB_QUEUE);

            ClassCreatorEditor window = GetWindowWithRect<ClassCreatorEditor>(new Rect(0, 0, 450, 500));
            window.assetDirectory = "Scripts/Game/";
            window.isUI = isUI;
            window.nameSpace = string.Join(".", directories);
            window.Show();
            return window;
        }

        private void OnGUI()
        {
            assetDirectory = "Scripts/Game/" + usingParams.ClassPath;
            bool anyErrors = false;

            GUIStyle bold = new GUIStyle(EditorStyles.label);
            bold.fontStyle = FontStyle.Bold;

            GUIStyle s = new GUIStyle(EditorStyles.label);
            s.normal.textColor = Color.red;
            s.alignment = TextAnchor.MiddleRight;

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Namespace");
            nameSpace = EditorGUILayout.TextField(nameSpace);
            EditorGUILayout.EndHorizontal();

            if (nameSpace.StartsWith("Game") == false)
            {
                EditorGUILayout.LabelField("Only allow creating classes in 'Scripts/Game' folder", s);
                anyErrors = true;
            }
            else
            {
                EditorGUILayout.LabelField("");
            }
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Class Name");
            baseName = EditorGUILayout.TextField(Regex.Replace(baseName, @"\s+", ""));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (baseName.Length == 0)
            {
                EditorGUILayout.LabelField("Base Name can't be blank or contains a special character.", s);
                anyErrors = true;
            }
            else
            {
                baseName = Regex.Replace(baseName, @"[^0-9a-zA-Z_]+", "");

                var firstChar = baseName.Substring(0, 1);
                if(int.TryParse(firstChar, out int firstNumber))
                {
                    EditorGUILayout.LabelField("Class Name can't accept '" + firstNumber + "' as the prefix.", s);
                    anyErrors = true;
                }
                else if (baseName.ToLower().EndsWith("view"))
                {
                    EditorGUILayout.LabelField("Base Name can't accept 'view' at the suffix.", s);
                    anyErrors = true;
                }
                else
                {
                    EditorGUILayout.LabelField("");
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            {
                usingParams.nameSpace = nameSpace + ("");
                usingParams.baseName = baseName;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{usingParams.ClassName}", bold);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.LabelField(viewAssetPath);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Is UI Window?");
                usingParams.isUI = EditorGUILayout.Toggle(usingParams.isUI);
                EditorGUILayout.EndHorizontal();

                if (usingParams.isUI == true)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Attach Root Canvas to prefab");
                    usingParams.hasRootCanvas = EditorGUILayout.Toggle(usingParams.hasRootCanvas);
                    EditorGUILayout.EndHorizontal();
                }


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Create a prefab");
                usingParams.hasPrefab = EditorGUILayout.Toggle(usingParams.hasPrefab);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.LabelField(usingParams.hasPrefab ? GetPrefabPath() : "");
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(20);

            EditorGUILayout.EndScrollView();

            var waitForCompiling = !string.IsNullOrEmpty(EditorPrefs.GetString(PREFAB_QUEUE));

            if (anyErrors || waitForCompiling)
            {
                EditorGUI.BeginDisabledGroup(true);
                GUILayout.Button("Can't Create Now");
                EditorGUI.EndDisabledGroup();

                if(waitForCompiling)
                {
                    EditorUtility.DisplayProgressBar("Attaching Script to Prefab Progress", "Waiting for scripts to compiling...", UnityEngine.Random.Range(0f, 0.8f));
                }
            }
            else
            {
                if (GUILayout.Button("Create"))
                {
                    EditorPrefs.DeleteKey(PREFAB_QUEUE);
                    CreateClass();
                }
            }
        }

        private void WriteFile(string assetPath, string text, bool warnIfExists = true)
        {
            string relativePath = "Assets/" + assetPath;
            string absolutePath = Path.Combine(Application.dataPath, assetPath);
            CreateFolderIfNeed(assetPath);
            if (File.Exists(absolutePath) == false)
            {
                File.WriteAllText(absolutePath, text);
                AssetDatabase.ImportAsset(relativePath);
            }
            else if (warnIfExists)
            {
                Debug.LogWarning("File already exists: " + assetPath);
            }
        }

        private void CopyFile(string from, string to)
        {
            var sourcePath = Path.Combine(Application.dataPath, from);
            var template = File.ReadAllText(sourcePath);

            WriteFile(to, template, false);
        }

        private void CreateClass()
        {
            // CopyFile(PATH_TO_TEMPLATE + "GameDefinition.asmdef.txt", "Scripts/Game.asmdef");
            //CopyFile(PATH_TO_TEMPLATE + "PlayModeTestDefinition.asmdef.txt", "Scripts/Tests/PlayMode/PlayModeTest.asmdef");
            //CopyFile(PATH_TO_TEMPLATE + "EditModeTestDefinition.asmdef.txt", "Scripts/Tests/EditMode/EditModeTest.asmdef");

            string text = ViewTemplate.Create(usingParams);
            WriteFile(viewAssetPath, text);
        

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

            if (usingParams.hasPrefab)
            {
                CreatePrefab();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
                UILoader.SetUIPath(nameSpace+"."+usingParams.ClassName, GetPrefabPath());
            }else
            {
                EditorUtility.DisplayDialog("Information", "Creating Classes Successful.", "OK");
            }
        }

        public string GetPrefabPath()
        {
            return "Resources/" + usingParams.PrefabPath + ".prefab";
        }        

        private void CreatePrefab()
        {
            EditorPrefs.SetString("UsingParams", JsonUtility.ToJson(usingParams));

            CreateFolderIfNeed(GetPrefabPath());           

            GameObject gameObject = EditorUtility.CreateGameObjectWithHideFlags(name, HideFlags.HideInHierarchy);
            if (usingParams.isUI == true)
            {
                gameObject.AddComponent<RectTransform>();
                gameObject.AddComponent<CanvasRenderer>();
                gameObject.AddComponent<GraphicRaycaster>();
                CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
                Canvas canvas = gameObject.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080, 1920);
            }

            string typeFullName = nameSpace + (".") + usingParams.ClassName;           

            var prefabSavePath = "Assets/" + GetPrefabPath();

            EditorPrefs.SetString(PREFAB_QUEUE, prefabSavePath + "," + typeFullName);

            PrefabUtility.SaveAsPrefabAsset(gameObject, prefabSavePath);
            DestroyImmediate(gameObject);
        }

        private void CreateFolderIfNeed(string to)
        {
            to = to.Replace("Assets/", "");
            List<string> targetSegments = to.Split('/').ToList();
            targetSegments.RemoveAt(targetSegments.Count - 1);
            string path = Application.dataPath;
            foreach (string segment in targetSegments)
            {
                path += "/" + segment;
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
            }
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void AttachScriptWhenReady()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += AttachScriptWhenReady;

                // TODO: catch exception here to close progress bar
                return;
            }

            EditorApplication.delayCall += AttachScriptToPrefab;
        }

        private static void AttachScriptToPrefab()
        {            
            EditorUtility.ClearProgressBar();

            var usingParams = JsonUtility.FromJson<TemplateUsingParams>(EditorPrefs.GetString("UsingParams", ""));
            EditorPrefs.SetString("UsingParams", "");

            var prefabInQueue = EditorPrefs.GetString(PREFAB_QUEUE);
            if (string.IsNullOrEmpty(prefabInQueue)) return;

            var keys = prefabInQueue.Split(',');
            var prefabPath = keys[0];
            var scriptToAttach = keys[1];

            if (!string.IsNullOrEmpty(prefabPath) && !string.IsNullOrEmpty(scriptToAttach))
            {
                var prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;

                Assembly assembly = Assembly.Load("Game");

                Type t = assembly.GetType(scriptToAttach);

                if (t != null)
                {
                    prefab.AddComponent(t);
                }

                if(usingParams != null)
                {
                    if (usingParams.isUI == true)
                    {
                        var canvas = prefab.GetComponent<Canvas>();
                        if(canvas == null) 
                        {
                            canvas = prefab.GetComponent<Canvas>();
                        }
                        var scaler = prefab.GetComponent<CanvasScaler>();
                        if(scaler == null)
                        {
                            scaler = prefab.AddComponent<CanvasScaler>();
                        }
                        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                        scaler.referenceResolution = new Vector2(1920, 1080);

                        prefab.AddComponent<GraphicRaycaster>();
                        
                    }

                    PrefabUtility.SavePrefabAsset(prefab);

                    // refresh RectTransform after saving prefab
                    {
                        var rectTransform = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(RectTransform)) as RectTransform;
                        if(rectTransform != null)
                        {
                            rectTransform.anchorMin = Vector2.zero;
                            rectTransform.anchorMax = Vector2.one;
                            rectTransform.sizeDelta = Vector2.zero;
                            rectTransform.localScale = Vector3.one;

                            rectTransform.gameObject.layer = LayerMask.NameToLayer("UI");

                            PrefabUtility.SavePrefabAsset(prefab);
                        }
                    }

                }

                EditorPrefs.DeleteKey(PREFAB_QUEUE);

                EditorUtility.DisplayDialog("Information", "Creating Classes Successful.", "OK");
            }
        }
    }
#endif
}
