using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace Muse
{
    using static ShortcutKeys;
    using static EditorUtils;

    public class PixelMapGeneratorWindow : EditorWindow
    {
        public int mapRadius;
        public PixelMapSettings pixelMapSettings;
        public GameObject[] selectedPrefabs;

        [MenuItem("Tools/Muse/Object Mapper" + SHORTCUT_WINDOW_OBJECTMAPPER)]
        static void ShowWindow() => GetWindow<PixelMapGeneratorWindow>("Object Mapper");

        public void GenerateMapFromSelected()
        {
            pixelMapSettings = GetAllInstances<PixelMapSettings>().Where(d => d != null).First();

            var selectedGameObjects =
                GetSelectedTransforms()
                .Where(a => !EditorUtility.IsPersistent(a))
                .Select(t => t.gameObject)
                .ToArray();

            var validPrefabs = pixelMapSettings.prefabsToMap.Select(m => m.prefab).ToList();
            var selectedPrefabs = GetGameObjectsWithPrefabs(selectedGameObjects).ToArray();

            var validSelections = new List<(GameObject, PixelMapSettings.PixelMappedPrefab)>();
            for (int i = 0; i < selectedPrefabs.Length; i++)
            {
                var sel = selectedPrefabs[i];
                var p = PrefabUtility.GetCorrespondingObjectFromSource(sel);
                if (validPrefabs.Contains(p))
                    validSelections.Add((sel, pixelMapSettings.GetMappableWithPrefab(p)));
            }

            var tex2D = DrawMap(validSelections);
            // Texture2DToPNG(tex2D);
            Texture2DToTGA(tex2D);
        }

        public static void Texture2DToTGA(Texture2D texture)
        {
            byte[] bytes = texture.EncodeToTGA();
            var dirPath = Application.dataPath + "/../SaveImages/";

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            File.WriteAllBytes(dirPath + "Image" + ".tga", bytes);
        }

        // public static void Texture2DToPNG(Texture2D texture)
        // {
        //     byte[] bytes = texture.EncodeToPNG();
        //     var dirPath = Application.dataPath + "/../SaveImages/";

        //     if (!Directory.Exists(dirPath))
        //         Directory.CreateDirectory(dirPath);

        //     File.WriteAllBytes(dirPath + "Image" + ".png", bytes);
        // }

        public static (int x, int z, float a) WorldPositionToPixelCoordsAndAlpha(Vector3 pos, int mapRadius)
        {
            var x = Mathf.RoundToInt(Mathf.Clamp(pos.x, -mapRadius, mapRadius)) + mapRadius;
            var a = Mathf.RoundToInt(Mathf.Clamp(pos.y, -50, 50) + 50) * 0.01f;
            var y = Mathf.RoundToInt(Mathf.Clamp(pos.z, -mapRadius, mapRadius)) + mapRadius;

            return (x, y, a);
        }

        public static Texture2D DrawMap(List<(GameObject prefab, PixelMapSettings.PixelMappedPrefab mappable)> validData)
        {
            var validGameObjects = validData.Select(v => v.prefab).ToArray();

            var mappables = validData.Select(v => v.mappable).ToArray();

            var mapRadius = CalculateMapRadius(validGameObjects.Select(go => go.transform.position).ToArray());

            var width = mapRadius * 2;
            var height = mapRadius * 2;

            var tex = new Texture2D(width, height, TextureFormat.RGBA64, false);

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

                var color = mappables[i].color;
                color.a = a;

                tex.SetPixel(x, y, color);
            }

            tex.Apply();

            return tex;
        }

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

        Vector2 scrollPos;

        void OnSelect()
        {
            selectedPrefabs =
               GetCorrespondingObjectsFromSources(
                   GetSelectedTransforms()
                   .Select(t => t.gameObject)
                   .ToArray());
        }

        void OnEnable()
        {
            OnSelect();

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

                    GUILayout.Label(" Prefabs ", new GUIStyle("Box"));
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