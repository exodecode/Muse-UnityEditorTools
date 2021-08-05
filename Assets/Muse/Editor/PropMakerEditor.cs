using UnityEditor;
using UnityEngine;

namespace Muse
{
    [CustomEditor(typeof(PropMaker))]
    public class PropMakerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var propMaker = target as PropMaker;
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
                var gameObjects = Selection.gameObjects;
                propMaker.FoldersForSelected(gameObjects);
            }
        }
    }
}