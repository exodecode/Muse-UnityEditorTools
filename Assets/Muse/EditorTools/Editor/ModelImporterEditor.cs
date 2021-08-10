// using UnityEditor;
// using UnityEngine;

// namespace Muse
// {
//     using static EditorUtils;

//     [CustomEditor(typeof(ModelImporter))]
//     public class ModelImporterEditor : Editor
//     {
//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI();
//             var modelImporter = target as ModelImporter;
//             var prefabReplacement = modelImporter.prefabReplacement;

//             EditorGUILayout.Space();

//             // if (GUILayout.Button("Replace Selected GameObjects"))
//             // ReplaceSelected(GetSelectedTransforms(), prefabReplacement, modelImporter.transform);
//         }
//     }
// }