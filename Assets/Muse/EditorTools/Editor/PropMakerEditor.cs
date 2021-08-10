using UnityEditor;
using UnityEngine;

namespace Muse
{
    using static EditorUtils;

    [CustomEditor(typeof(PropMaker))]
    public class PropMakerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var propMaker = target as PropMaker;

            EditorGUILayout.Space();

            if (GUILayout.Button("Make Prefab Variant From Selected Models"))
            {
                var gameObjects = Selection.gameObjects;
                propMaker.FromSelectedModels(gameObjects);
            }
            if (GUILayout.Button("Make Prefab Variant From Selected GameObjects"))
            {
                var gameObjects = Selection.gameObjects;
                propMaker.FromSelectedGameObjects(gameObjects);
            }
            if (GUILayout.Button("Create Folder(s) for selected"))
            {
                FoldersForSelected(Selection.gameObjects);
            }
        }

        public static void ReplaceSelected(Transform[] transforms, GameObject prefabReplacement, Transform parentTransform)
        {
            var selectedGameObjects = Selection.gameObjects;
            var length = selectedGameObjects.Length;
            var replacementsParent = new GameObject("[Replacements]");

            for (int i = 0; i < length; i++)
            {
                var go = selectedGameObjects[i];
                go.transform.SetParent(parentTransform);

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

}