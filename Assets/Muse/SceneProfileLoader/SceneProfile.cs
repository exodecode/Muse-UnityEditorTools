using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Muse
{
    [CreateAssetMenu(fileName = "SceneProfile", menuName = "EditorTools/SceneProfile", order = 0)]
    public class SceneProfile : ScriptableObject
    {
        public SceneAsset activeScene;
        public List<SceneAsset> additiveScenes = new List<SceneAsset>();

        public List<string> GetSceneAssetPaths(List<SceneAsset> sceneAssets) =>
            sceneAssets.Select(sceneAsset => AssetDatabase.GetAssetPath(sceneAsset)).ToList();

        public void LoadScenes()
        {
            var activeScenePath = AssetDatabase.GetAssetPath(activeScene);
            EditorSceneManager.OpenScene(activeScenePath, OpenSceneMode.Single);

            GetSceneAssetPaths(additiveScenes)
                .ForEach(path => EditorSceneManager.OpenScene(path, OpenSceneMode.Additive));
        }

        List<Scene> GetScenesFromPaths(List<string> paths) =>
            paths.Select(path => SceneManager.GetSceneByPath(path)).ToList();

        public void UnloadAdditiveScenes(bool remove)
        {
            var paths = GetSceneAssetPaths(additiveScenes);
            var scenes = GetScenesFromPaths(paths);
            scenes.ForEach(scene => EditorSceneManager.CloseScene(scene, remove));
        }

        public void SetExpandedForAllScenes(bool expand)
        {
            var paths = GetSceneAssetPaths(additiveScenes);
            var scenes = GetScenesFromPaths(paths);
            scenes.ForEach(scene => SetExpanded(scene, expand));
        }

        private static void SetExpanded(Scene scene, bool expand)
        {
            foreach (var window in Resources.FindObjectsOfTypeAll<SearchableEditorWindow>())
            {
                if (window.GetType().Name != "SceneHierarchyWindow")
                    continue;

                var method = window.GetType().GetMethod("SetExpandedRecursive",
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance, null,
                    new[] { typeof(int), typeof(bool) }, null);

                if (method == null)
                {
                    Debug.LogError(
                        "Could not find method 'UnityEditor.SceneHierarchyWindow.SetExpandedRecursive(int, bool)'.");
                    return;
                }

                var field = scene.GetType().GetField("m_Handle",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (field == null)
                {
                    Debug.LogError("Could not find field 'int UnityEngine.SceneManagement.Scene.m_Handle'.");
                    return;
                }

                var sceneHandle = field.GetValue(scene);
                method.Invoke(window, new[] { sceneHandle, expand });
            }
        }
    }

    [CustomEditor(typeof(SceneProfile))]
    public class SceneProfileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var sceneProfile = target as SceneProfile;

            EditorGUILayout.Space();

            if (GUILayout.Button("Load Scenes"))
                sceneProfile.LoadScenes();

            if (GUILayout.Button("Unload Additive Scenes"))
                sceneProfile.UnloadAdditiveScenes(false);

            if (GUILayout.Button("Remove Additive Scenes"))
                sceneProfile.UnloadAdditiveScenes(true);

            EditorGUILayout.Space();

            if (GUILayout.Button("Collapse All Scenes"))
                sceneProfile.SetExpandedForAllScenes(false);

            // if (GUILayout.Button("Expand All Scenes"))
            //     sceneProfile.SetExpandedForAllScenes(true);
        }
    }
}