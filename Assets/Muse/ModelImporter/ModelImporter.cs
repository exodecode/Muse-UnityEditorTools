using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Muse
{
    public class ModelImporter : MonoBehaviour
    {
        public GameObject prefabReplacement;

        public void ReplaceSelected()
        {
            var selectedGameObjects = Selection.gameObjects;
            var length = selectedGameObjects.Length;
            var replacementsParent = new GameObject("[Replacements]");

            for (int i = 0; i < length; i++)
            {
                var go = selectedGameObjects[i];
                go.transform.SetParent(transform);

                var position = go.transform.position;
                var rotation = go.transform.rotation;
                var replacement = PrefabUtility.InstantiatePrefab(prefabReplacement) as GameObject;

                replacement.transform.position = position;
                replacement.transform.rotation = rotation;

                replacement
                    .transform
                    .SetParent(replacementsParent.transform);
            }
        }
    }

    [CustomEditor(typeof(ModelImporter))]
    public class ModelImporterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var a = target as ModelImporter;

            EditorGUILayout.Space();

            if (GUILayout.Button("Replace Selected GameObjects"))
                a.ReplaceSelected();
        }
    }
}