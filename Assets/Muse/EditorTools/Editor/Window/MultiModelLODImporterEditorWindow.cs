using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Muse
{
    using static FilePathUtils;
    using static ShortcutKeys;

    public class MultiModelLODImporterEditorWindow : EditorWindow
    {
        public GameObject basePrefab;
        public string namePrefix;
        public string nameSuffix;

        public readonly string[] acceptedFileTypes = new string[] { ".fbx", ".obj" };
        string[] validSelections;

        Vector2 scrollPos;
        string list;

        [MenuItem("Tools/Muse/Multiple Model LOD Import" + SHORTCUT_WINDOW_LOD)]
        static void ShowWindow() => GetWindow<MultiModelLODImporterEditorWindow>("Multiple Model LOD Import");

        void OnEnable()
        {
            Selection.selectionChanged += Repaint;
            Selection.selectionChanged += OnSelect;
        }

        void OnDisable()
        {
            Selection.selectionChanged -= Repaint;
            Selection.selectionChanged -= OnSelect;
        }

        void OnSelect()
        {
            validSelections =
                GetAssetPathsFromSelections(Selection.objects)
                .Where(sel => acceptedFileTypes.Contains(GetFileNameAndType(sel).fileType))
                .ToArray();

            list = validSelections.Flatten("\n");
        }

        void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                basePrefab = EditorGUILayout.ObjectField("Base Prefab", basePrefab, typeof(GameObject), false) as GameObject;
                namePrefix = EditorGUILayout.TextField("Name Prefix", namePrefix);
                nameSuffix = EditorGUILayout.TextField("Name Suffix", nameSuffix);
            }

            using (new EditorGUI.DisabledScope(validSelections == null || validSelections.Length == 0))
            {
                var buttonStyle = new GUIStyle("Button");
                buttonStyle.fontSize = 14;

                if (GUILayout.Button("Create LOD Prefabs", buttonStyle))
                {
                    for (int i = 0; i < validSelections.Length; i++)
                    {
                        var a = AssetDatabase.LoadAssetAtPath<GameObject>(validSelections[i]);
                        var go = Instantiate(a);

                        var children = Enumerable.Range(0, go.transform.childCount).Select(j => go.transform.GetChild(j));
                        var childrenWithLOD = children.Where(child => child.name.Contains("_LOD"));
                        var sortedGroups = childrenWithLOD.GroupBy(c => c.name.Substring(0, c.name.Length - 1)).ToList();

                        sortedGroups.ForEach(group =>
                        {
                            var g =
                                basePrefab == null ?
                                new GameObject() :
                                PrefabUtility.InstantiatePrefab(basePrefab) as GameObject;

                            g.name = group.Key.Substring(0, group.Key.Length - 4);

                            var values = group.ToArray();
                            var lodGroup = g.AddComponent<LODGroup>();

                            for (int j = 0; j < values.Length; j++)
                            {
                                var t = values[j];
                                t.SetParent(g.transform);

                                t.localPosition = Vector3.zero;
                                t.localRotation = Quaternion.identity;
                                t.localScale = Vector3.one;
                            }

                            var lods = new LOD[values.Length];

                            for (int j = 0; j < values.Length; j++)
                            {
                                var rends = values[j].GetComponents<Renderer>();
                                lods[j] = new LOD(1f / (j + 1), rends);
                            }

                            lodGroup.SetLODs(lods);
                            lodGroup.RecalculateBounds();

                            var path = "Assets/";
                            var pathWithName = path.Substring(0, path.LastIndexOf('/') + 1) + namePrefix + g.name + nameSuffix + ".prefab";

                            g.transform.position = Vector3.zero;
                            g.transform.rotation = Quaternion.identity;

                            var obj = PrefabUtility.SaveAsPrefabAsset(g, pathWithName);

                            DestroyImmediate(g);
                        });

                        DestroyImmediate(go);
                    }

                    OnSelect();
                    AssetDatabase.Refresh();
                }

                var scrollStyle = new GUIStyle("HelpBox");
                scrollStyle.fontSize = 14;
                scrollStyle.alignment = TextAnchor.MiddleLeft;
                scrollStyle.wordWrap = false;

                GUILayout.FlexibleSpace();

                using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos, scrollStyle, GUILayout.Height(400)))
                {
                    GUILayout.Label("Valid Selections", new GUIStyle("Box"));
                    scrollPos = scrollView.scrollPosition;

                    var pathStyle = new GUIStyle("WhiteLabel");
                    pathStyle.fontSize = 14;
                    GUILayout.Label(list, pathStyle);
                }
            }
        }
    }
}