using UnityEngine;
using UnityEditor;

namespace Muse
{
    using static EditorUtils;

    public class GameObjectSwapperEditorWindow : EditorWindow
    {
        [SerializeField] GameObject replacement;

        [MenuItem("Tools/Muse/GameObject Swapper")]
        static void ShowWindow() => GetWindow<GameObjectSwapperEditorWindow>("GameObject Swapper");

        void OnEnable() => Selection.selectionChanged += Repaint;
        void OnDisable() => Selection.selectionChanged -= Repaint;

        void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope())
                replacement =
                    EditorGUILayout.ObjectField("Replacement", replacement, typeof(GameObject), false) as GameObject;

            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0 || replacement == null))
            {
                if (GUILayout.Button("Replace Selected GameObjects"))
                {
                    ReplaceSelected(GetSelectedTransforms(), replacement);
                }
            }
        }

        static void ReplaceSelected(Transform[] transforms, GameObject prefabReplacement)
        {
            var selectedGameObjects = Selection.gameObjects;
            var length = selectedGameObjects.Length;
            var replacementsParent = new GameObject("[" + prefabReplacement.name + "]");

            for (int i = 0; i < length; i++)
            {
                var go = selectedGameObjects[i];
                var position = go.transform.position;
                var rotation = go.transform.rotation;
                var replacement = PrefabUtility.InstantiatePrefab(prefabReplacement) as GameObject;

                replacement.transform.position = position;
                replacement.transform.rotation = rotation;

                replacement
                    .transform
                    .SetParent(replacementsParent.transform);

                Undo.RegisterCreatedObjectUndo(replacement, "Create replacement");
                Undo.DestroyObjectImmediate(go);
            }

            Undo.RegisterCreatedObjectUndo(replacementsParent, "Create replacement parent");
        }
    }
}