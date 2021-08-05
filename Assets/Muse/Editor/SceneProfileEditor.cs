using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Muse
{
    using static SceneUtils;

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
    }
}