using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Muse
{
    public class PrefabTools : MonoBehaviour
    {
        public string[] prefabDirectories;

        public void GrabAllPrefabsFromDirectories()
        {
            var selectedGameObjects = Selection.gameObjects;
            var length = selectedGameObjects.Length;

            for (int i = 0; i < length; i++)
            {
                var selection = selectedGameObjects[i];
                var prefabParent = PrefabUtility.GetCorrespondingObjectFromSource(selection);

                if (prefabParent != null)
                {
                    var path = AssetDatabase.GetAssetPath(prefabParent);
                    var dir = path.Substring(0, path.LastIndexOf('/') + 1);
                    bool inDirectory = false;

                    for (int j = 0; j < prefabDirectories.Length; j++)
                    {
                        if (dir == prefabDirectories[j])
                        {
                            inDirectory = true;
                            break;
                        }
                    }

                    if (inDirectory)
                        selection.transform.SetParent(transform);
                }
            }
        }
    }

    [CustomEditor(typeof(PrefabTools))]
    public class PrefabToolsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var a = target as PrefabTools;
            EditorGUILayout.Space();

            if (GUILayout.Button("Grab All PrefabsFromDirectories"))
                a.GrabAllPrefabsFromDirectories();
        }
    }

}
