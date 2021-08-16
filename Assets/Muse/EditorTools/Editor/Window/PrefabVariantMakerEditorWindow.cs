using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace Muse
{
    using static ShortcutKeys;

    public class PrefabVariantMakerEditorWindow : EditorWindow
    {
        public const string SHORTCUT_KEY_PREFIX = "&#"; //Alt Shift

        public GameObject basePrefab;
        public string nameSuffix;
        public bool clearChildTransforms;
        public List<string> acceptedModelFileTypes = new List<string>() { ".fbx", ".obj" };

        [MenuItem("Tools/Muse/Prefab Variant Maker" + SHORTCUT_WINDOW_PREFABVARIANT)]
        static void ShowWindow() => GetWindow<PrefabVariantMakerEditorWindow>("Prefab Variant Maker");

        void OnEnable() => Selection.selectionChanged += Repaint;
        void OnDisable() => Selection.selectionChanged -= Repaint;

        void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                basePrefab = EditorGUILayout.ObjectField("Base Prefab", basePrefab, typeof(GameObject), false) as GameObject;
                nameSuffix = EditorGUILayout.TextField("Name Suffix", nameSuffix);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0 || basePrefab == null))
            {
                var persistantSelections =
                    Selection.gameObjects
                    .Where(s => EditorUtility.IsPersistent(s))
                    .ToArray();

                var modelSelections =
                    persistantSelections
                    .Select(selection => (selection, path: AssetDatabase.GetAssetPath(selection)))
                    .Where(pair =>
                    {
                        var path = pair.path;
                        var fileType = path.Substring(path.LastIndexOf("."));
                        return acceptedModelFileTypes.Contains(fileType);
                    })
                    .Select(p => p.selection)
                    .ToArray();

                using (new EditorGUI.DisabledScope(persistantSelections.Length > 0))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Create Prefabs Variants From Selected GameObjects"))
                            CreateVariantsFromSelectedGameObjects(
                                Selection.gameObjects,
                                basePrefab,
                                nameSuffix,
                                clearChildTransforms);

                        clearChildTransforms = EditorGUILayout.Toggle("Clear Child Transforms", clearChildTransforms);
                    }
                }

                EditorGUILayout.Space();

                using (new EditorGUI.DisabledScope(modelSelections.Length == 0))
                {
                    if (GUILayout.Button("Create Prefabs Variants From Selected Model Assets"))
                        CreateVariantsFromSelectedModels(
                            modelSelections,
                            basePrefab,
                            nameSuffix);
                }
            }
        }

        public static void CreateVariantsFromSelectedModels(GameObject[] children, GameObject basePrefabAsset, string suffix)
        {
            for (int i = 0; i < children.Length; i++)
            {
                var child = children[i];

                var basePrefabGameObject = PrefabUtility.InstantiatePrefab(basePrefabAsset) as GameObject;
                var prop = PrefabUtility.InstantiatePrefab(child) as GameObject;

                basePrefabGameObject.name = child.name;

                prop.transform.SetParent(basePrefabGameObject.transform);

                var path = AssetDatabase.GetAssetPath(child);
                var pathWithName = path.Substring(0, path.LastIndexOf('/') + 1) + child.name + suffix + ".prefab";
                var obj = PrefabUtility.SaveAsPrefabAsset(basePrefabGameObject, pathWithName);

                DestroyImmediate(basePrefabGameObject);
            }
        }

        public void CreateVariantsFromSelectedGameObjects(GameObject[] children, GameObject basePrefabAsset, string suffix, bool zeroOutChildTransforms)
        {
            for (int i = 0; i < children.Length; i++)
            {
                var child = children[i];
                var basePrefabGameObject = PrefabUtility.InstantiatePrefab(basePrefabAsset) as GameObject;

                if (zeroOutChildTransforms)
                {
                    basePrefabGameObject.transform.position = child.transform.position;
                    basePrefabGameObject.transform.rotation = child.transform.rotation;
                }

                var copy = Instantiate<GameObject>(child, basePrefabGameObject.transform, false);
                copy.name = child.name;

                if (zeroOutChildTransforms)
                {
                    copy.transform.localPosition = Vector3.zero;
                    copy.transform.localRotation = Quaternion.identity;
                }

                basePrefabGameObject.name = copy.name;

                var path = "Assets/";
                var pathWithName = path.Substring(0, path.LastIndexOf('/') + 1) + child.name + suffix + ".prefab";

                basePrefabGameObject.transform.position = Vector3.zero;
                basePrefabGameObject.transform.rotation = Quaternion.identity;

                var obj = PrefabUtility.SaveAsPrefabAsset(basePrefabGameObject, pathWithName);

                var variant = PrefabUtility.InstantiatePrefab(obj) as GameObject;

                variant.transform.position = child.transform.position;
                variant.transform.rotation = child.transform.rotation;

                DestroyImmediate(basePrefabGameObject);
            }
        }
    }
}