using UnityEditor;
using UnityEngine;

namespace Muse
{
    [CustomEditor(typeof(PrefabTools))]
    public class PrefabToolsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var prefabTools = target as PrefabTools;
            EditorGUILayout.Space();

            if (GUILayout.Button("Grab All PrefabsFromDirectories"))
                prefabTools.GrabAllPrefabsFromDirectories();
        }
    }
}