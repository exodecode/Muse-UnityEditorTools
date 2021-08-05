using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Muse
{
    using static EditorUtils;

    [CustomEditor(typeof(SceneProfile))]
    public class SceneProfileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var sceneProfile = target as SceneProfile;
            var activeScene = sceneProfile.activeScene;
            var additiveScenes = sceneProfile.additiveScenes;

            EditorGUILayout.Space();

            if (GUILayout.Button("Load Scenes"))
                LoadScenes(activeScene, additiveScenes);

            if (GUILayout.Button("Unload Additive Scenes"))
                UnloadAdditiveScenes(false, additiveScenes);

            if (GUILayout.Button("Remove Additive Scenes"))
                UnloadAdditiveScenes(true, additiveScenes);

            EditorGUILayout.Space();

            if (GUILayout.Button("Collapse All Scenes"))
                SetExpandedForAllScenes(false, additiveScenes);
        }

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
    }
}