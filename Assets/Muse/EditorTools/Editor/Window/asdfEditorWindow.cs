using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace Muse
{
    using static ShortcutKeys;
    using static EditorUtils;

    public class asdfEditorWindow : EditorWindow
    {
        public int mapRadius;
        public MappablesData mappablesData;
        public GameObject[] selectedPrefabs;

        [MenuItem("Tools/Muse/Object Mapper" + SHORTCUT_WINDOW_OBJECTMAPPER)]
        static void ShowWindow() => GetWindow<asdfEditorWindow>("Object Mapper");

        public void GenerateMapFromSelected()
        {
            mappablesData = GetAllInstances<MappablesData>().Where(d => d != null).First();

            var selectedGameObjects =
                GetSelectedTransforms()
                .Where(a => !EditorUtility.IsPersistent(a))
                .Select(t => t.gameObject)
                .ToArray();

            var validPrefabs = mappablesData.mappables.Select(m => m.prefab).ToList();
            var selectedPrefabs = GetGameObjectsWithPrefabs(selectedGameObjects).ToArray();

            var validSelections = new List<GameObject>();
            for (int i = 0; i < selectedPrefabs.Length; i++)
            {
                var sel = selectedPrefabs[i];
                if (validPrefabs.Contains(sel))
                    validSelections.Add(sel);
            }

            var tex2D = DrawMap(validPrefabs.ToArray());
            Texture2DToPNG(tex2D);
        }

        public static void Texture2DToPNG(Texture2D texture)
        {
            // Texture2D tex = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);

            byte[] bytes = texture.EncodeToPNG();
            var dirPath = Application.dataPath + "/../SaveImages/";

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            File.WriteAllBytes(dirPath + "Image" + ".png", bytes);
        }

        public static (int x, int z, float a) WorldPositionToPixelCoordsAndAlpha(Vector3 pos, int mapRadius)
        {
            var x = Mathf.RoundToInt(Mathf.Clamp(pos.x, -mapRadius, mapRadius)) + mapRadius;
            var a = Mathf.Clamp(pos.y, -50, 50);
            var y = Mathf.RoundToInt(Mathf.Clamp(pos.z, -mapRadius, mapRadius)) + mapRadius;

            return (x, y, a);
        }

        public static Vector3 PixelCoordsAndAlphaToWorldPos((int x, int y, float a) pixelCoordsAndAlpha, int mapRadius) =>
             new Vector3(
                 pixelCoordsAndAlpha.x - mapRadius,
                 HeightFromAlpha(pixelCoordsAndAlpha.a),
                 pixelCoordsAndAlpha.y - mapRadius);

        public static Texture2D DrawMap(GameObject[] validGameObjects)
        {
            // var mapRadius = CalculateMapRadius(validGameObjects.Select(go => go.transform.position).ToArray());
            var mapRadius = 64;

            var width = mapRadius * 2;
            var height = mapRadius * 2;
            var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

            for (int w = 0; w < width; w++)
                for (int h = 0; h < height; h++)
                    tex.SetPixel(w, h, Color.clear);

            tex.Apply();

            for (int i = 0; i < validGameObjects.Length; i++)
            {
                var go = validGameObjects[i];
                var pos = go.transform.position;
                var coordsAndAlpha = WorldPositionToPixelCoordsAndAlpha(pos, mapRadius);

                var x = coordsAndAlpha.x;
                var y = coordsAndAlpha.z;
                var a = coordsAndAlpha.a;

                Debug.Log("X: " + x + ", Y: " + y + ", Alpha: " + a);

                var color = new Color(1, 1, 1, a);

                tex.SetPixel(x, y, color);
            }

            tex.Apply();

            return tex;
            /*
                #1##
                #01# <-- origin is at 0
                ####
                ####

                2345
                8901
                4567
                0123
            */
        }

        public static float HeightFromAlpha(float a) => (Mathf.Clamp(a, 0, 1) * 100) - 50;
        public static float HeightToAlpha(float height) => (Mathf.Clamp(height, -50, 50) + 50) / 100;

        public static GameObject[] GetCorrespondingObjectsFromSources(GameObject[] gameObjects) =>
            gameObjects.Select(g => PrefabUtility.GetCorrespondingObjectFromSource(g)).Distinct().ToArray();

        public static GameObject[] GetGameObjectsWithPrefabs(GameObject[] gameObjects) =>
            gameObjects.Where(g => PrefabUtility.GetCorrespondingObjectFromSource(g) != null).ToArray();

        public static int CalculateMapRadius(Vector3[] positions)
        {
            var list = positions.ToList();
            var xs = list.Select(v => Mathf.Abs(v.x));
            var ys = list.Select(v => Mathf.Abs(v.x));

            var gx = (int)xs.OrderBy(a => a).Last();
            var gy = (int)ys.OrderBy(a => a).Last();

            var px = RoundToNextPowerOfTwo(gx);
            var py = RoundToNextPowerOfTwo(gy);

            return px > py ? px : py;
        }

        public static int RoundToNextPowerOfTwo(int val) =>
            (int)Mathf.Pow(2, Mathf.Ceil(Mathf.Log(val) / Mathf.Log(2)));

        // public readonly string[] acceptedFileTypes = new string[] { ".fbx", ".obj", ".prefab", ".unity", ".asset" };
        Vector2 scrollPos;
        // string[] assetPaths;

        SerializedObject so;
        // SerializedProperty propValidPathsThatAlreadyExist;
        // SerializedProperty propValidNewDir;
        // SerializedProperty propSimplifiedValidNewDir;

        // public string[] validPathsThatAlreadyExist;
        // public string[] validNewDirectories;
        // public string simplifiedValidNewDirectories;

        // [MenuItem("Tools/Muse/Folder Maker" + SHORTCUT_WINDOW_FOLDER)]
        // private static void ShowWindow() => GetWindow<FolderMakerEditorWindow>("Folder Maker");

        void OnSelect()
        {
            selectedPrefabs =
               GetCorrespondingObjectsFromSources(
                   GetSelectedTransforms()
                   .Select(t => t.gameObject)
                   .ToArray());

            // assetPaths = GetAssetPathsFromSelections(Selection.objects);

            // var fullAssetPaths =
            //     assetPaths
            //     .Select(assetPath => GetFullAssetPath(assetPath))
            //     .ToArray();

            // validNewDirectories =
            //    fullAssetPaths
            //    .Where(p => !string.IsNullOrEmpty(p) && p.Contains('.'))
            //    .Where(path => acceptedFileTypes.Contains(GetFileNameAndType(path).fileType))
            //    .Select(path => (parent: GetParentDirectory(path), newDirectory: GetNewDirectoryPath(path)))
            //    .Distinct()
            //    .Where(pair => GetDirectoryName(pair.parent) != GetDirectoryName(pair.newDirectory))
            //    .Select(p => p.newDirectory)
            //    .ToArray();

            // propValidNewDir.arraySize = validNewDirectories.Length;

            // for (int i = 0; i < propValidNewDir.arraySize; i++)
            //     propValidNewDir.GetArrayElementAtIndex(i).stringValue = validNewDirectories[i];

            // simplifiedValidNewDirectories =
            //     validNewDirectories
            //     .Select(path => path.Substring(Application.dataPath.LastIndexOf('/') + 1))
            //     .ToArray()
            // //     .Flatten("\n");

            // propSimplifiedValidNewDir.stringValue = simplifiedValidNewDirectories;

            // validPathsThatAlreadyExist = GetValidDirectoriesThatExistFromAssetPaths(assetPaths);
            // propValidPathsThatAlreadyExist.arraySize = validPathsThatAlreadyExist.Length;

            // for (int i = 0; i < propValidPathsThatAlreadyExist.arraySize; i++)
            //     propValidPathsThatAlreadyExist.GetArrayElementAtIndex(i).stringValue =
            //     validPathsThatAlreadyExist[i];
        }

        void OnEnable()
        {
            OnSelect();
            so = new SerializedObject(this);
            // propValidPathsThatAlreadyExist = so.FindProperty("validPathsThatAlreadyExist");

            // propValidNewDir = so.FindProperty("validNewDirectories");

            // propSimplifiedValidNewDir = so.FindProperty("simplifiedValidNewDirectories");

            Selection.selectionChanged += Repaint;
            Selection.selectionChanged += OnSelect;
        }

        void OnDisable()
        {
            Selection.selectionChanged -= Repaint;
            Selection.selectionChanged -= OnSelect;
        }

        void OnGUI()
        {
            // EditorGUILayout.PropertyField(propValidPathsThatAlreadyExist);
            // EditorGUILayout.PropertyField(propValidNewDir);
            // EditorGUILayout.PropertyField(propSimplifiedValidNewDir);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(Selection.objects.Length == 0 || selectedPrefabs.Length == 0))
            {
                var buttonStyle = new GUIStyle("Button");
                buttonStyle.fontSize = 14;

                if (GUILayout.Button("Map Selected GameObjects", buttonStyle))
                {
                    OnSelect();
                    GenerateMapFromSelected();
                }


                var scrollStyle = new GUIStyle("HelpBox");
                scrollStyle.fontSize = 14;
                scrollStyle.alignment = TextAnchor.MiddleLeft;
                scrollStyle.wordWrap = false;

                GUILayout.FlexibleSpace();

                using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos, scrollStyle, GUILayout.Height(400)))
                {
                    var names = selectedPrefabs.Where(a => a != null).Select(p => p.name);

                    GUILayout.Label("Prefabs", new GUIStyle("Box"));
                    scrollPos = scrollView.scrollPosition;

                    var pathStyle = new GUIStyle("WhiteLabel");
                    pathStyle.fontSize = 14;
                    GUILayout.Label(string.Join("\n", names), pathStyle);
                }
            }
        }

        static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            var paths = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid));
            var assets = paths.Select(path => AssetDatabase.LoadAssetAtPath<T>(path));

            return assets.ToArray();
        }
    }
}