using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Muse
{
    public static class SceneUtils
    {
        public static void UnloadAdditiveScenes(bool remove, List<SceneAsset> additiveScenes)
        {
            var paths = GetSceneAssetPaths(additiveScenes);
            var scenes = GetScenesFromPaths(paths);
            scenes.ForEach(scene => EditorSceneManager.CloseScene(scene, remove));
        }

        public static void LoadScenes(SceneAsset activeScene, List<SceneAsset> additiveScenes)
        {
            var activeScenePath = AssetDatabase.GetAssetPath(activeScene);
            EditorSceneManager.OpenScene(activeScenePath, OpenSceneMode.Single);

            GetSceneAssetPaths(additiveScenes)
                .ForEach(path => EditorSceneManager.OpenScene(path, OpenSceneMode.Additive));
        }

        public static void SetExpandedForAllScenes(bool expand, List<SceneAsset> additiveScenes)
        {
            var paths = GetSceneAssetPaths(additiveScenes);
            var scenes = GetScenesFromPaths(paths);
            scenes.ForEach(scene => SetScenesExpanded(scene, expand));
        }

        public static List<Scene> GetScenesFromPaths(List<string> paths) =>
            paths.Select(path => SceneManager.GetSceneByPath(path)).ToList();

        public static List<string> GetSceneAssetPaths(List<SceneAsset> sceneAssets) =>
            sceneAssets.Select(sceneAsset => AssetDatabase.GetAssetPath(sceneAsset)).ToList();

        public static void SetScenesExpanded(Scene scene, bool expand)
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
}