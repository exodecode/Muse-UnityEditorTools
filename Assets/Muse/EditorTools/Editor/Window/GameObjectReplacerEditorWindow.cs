using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Muse
{
    using static EditorUtils;
    using static ShortcutKeys;

    public class GameObjectReplacerEditorWindow : EditorWindow
    {
        [SerializeField] GameObject replacement;
        Vector2 scrollPos;

        [MenuItem("Tools/Muse/GameObject Replacer" + SHORTCUT_WINDOW_REPLACER)]
        static void ShowWindow() => GetWindow<GameObjectReplacerEditorWindow>("GameObject Replacer");

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

            var scrollStyle = new GUIStyle("HelpBox");
            scrollStyle.fontSize = 14;
            scrollStyle.alignment = TextAnchor.MiddleLeft;
            scrollStyle.wordWrap = false;

            GUILayout.FlexibleSpace();

            var textStyle = new GUIStyle("Box");
            textStyle.fontSize = 14;

            GUILayout.Label("Selected GameObjects", textStyle);

            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos, scrollStyle, GUILayout.Height(300)))
            {
                scrollPos = scrollView.scrollPosition;

                textStyle = new GUIStyle("WhiteLabel");
                textStyle.fontSize = 14;

                GUILayout.Label(FlattenStringArray(Selection.gameObjects.Select(g => g.name).ToArray()), textStyle);
            }
        }

        static void ReplaceSelected(Transform[] transforms, GameObject prefabReplacement)
        {
            var selectedGameObjects = Selection.gameObjects;
            var length = selectedGameObjects.Length;

            for (int i = 0; i < length; i++)
            {
                var go = selectedGameObjects[i];
                var position = go.transform.position;
                var rotation = go.transform.rotation;
                var replacement = PrefabUtility.InstantiatePrefab(prefabReplacement) as GameObject;

                replacement.transform.position = position;
                replacement.transform.rotation = rotation;

                replacement.transform.SetParent(go.transform);
                replacement.transform.SetParent(go.transform.parent);

                Undo.RegisterCreatedObjectUndo(replacement, "Create replacement");
                Undo.DestroyObjectImmediate(go);
            }
        }

        static string FlattenStringArray(string[] array) =>
            string.Join("\n", array);
    }
}