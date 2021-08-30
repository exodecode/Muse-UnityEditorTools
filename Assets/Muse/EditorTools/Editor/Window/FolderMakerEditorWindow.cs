using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace Muse
{
    using static ShortcutKeys;
    using static FilePathUtils;

    public class FolderMakerEditorWindow : EditorWindow
    {
        public readonly string[] acceptedFileTypes = new string[] { ".fbx", ".obj", ".prefab", ".unity", ".asset" };
        Vector2 scrollPos;
        string[] assetPaths;

        SerializedObject so;
        SerializedProperty propValidPathsThatAlreadyExist;
        SerializedProperty propValidNewDir;
        SerializedProperty propSimplifiedValidNewDir;

        public string[] validPathsThatAlreadyExist;
        public string[] validNewDirectories;
        public string simplifiedValidNewDirectories;

        [MenuItem("Tools/Muse/Folder Maker" + SHORTCUT_WINDOW_FOLDER)]
        private static void ShowWindow() => GetWindow<FolderMakerEditorWindow>("Folder Maker");

        void OnSelect()
        {
            assetPaths = GetAssetPathsFromSelections(Selection.objects);

            var fullAssetPaths =
                assetPaths
                .Select(assetPath => GetFullAssetPath(assetPath))
                .ToArray();

            validNewDirectories =
               fullAssetPaths
               .Where(p => !string.IsNullOrEmpty(p) && p.Contains('.'))
               .Where(path => acceptedFileTypes.Contains(GetFileNameAndType(path).fileType))
               .Select(path => (parent: GetParentDirectory(path), newDirectory: GetNewDirectoryPath(path)))
               .Distinct()
               .Where(pair => GetDirectoryName(pair.parent) != GetDirectoryName(pair.newDirectory))
               .Select(p => p.newDirectory)
               .ToArray();

            propValidNewDir.arraySize = validNewDirectories.Length;

            for (int i = 0; i < propValidNewDir.arraySize; i++)
                propValidNewDir.GetArrayElementAtIndex(i).stringValue = validNewDirectories[i];

            simplifiedValidNewDirectories =
                validNewDirectories
                .Select(path => path.Substring(Application.dataPath.LastIndexOf('/') + 1))
                .ToArray()
                .Flatten("\n");

            propSimplifiedValidNewDir.stringValue = simplifiedValidNewDirectories;

            validPathsThatAlreadyExist = GetValidDirectoriesThatExistFromAssetPaths(assetPaths);
            propValidPathsThatAlreadyExist.arraySize = validPathsThatAlreadyExist.Length;

            for (int i = 0; i < propValidPathsThatAlreadyExist.arraySize; i++)
                propValidPathsThatAlreadyExist.GetArrayElementAtIndex(i).stringValue =
                validPathsThatAlreadyExist[i];
        }

        void OnEnable()
        {
            so = new SerializedObject(this);

            propValidPathsThatAlreadyExist = so.FindProperty("validPathsThatAlreadyExist");

            propValidNewDir = so.FindProperty("validNewDirectories");

            propSimplifiedValidNewDir = so.FindProperty("simplifiedValidNewDirectories");

            Selection.selectionChanged += Repaint;
            Selection.selectionChanged += OnSelect;
        }

        void OnDisable()
        {
            Selection.selectionChanged -= Repaint;
            Selection.selectionChanged -= OnSelect;
        }

        void OnGUI()
        {
            EditorGUILayout.PropertyField(propValidPathsThatAlreadyExist);
            EditorGUILayout.PropertyField(propValidNewDir);
            EditorGUILayout.PropertyField(propSimplifiedValidNewDir);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(Selection.objects.Length == 0))
            {
                var buttonStyle = new GUIStyle("Button");
                buttonStyle.fontSize = 14;

                if (GUILayout.Button("Create Folders for Selected Assets", buttonStyle))
                {
                    for (int i = 0; i < propValidNewDir.arraySize; i++)
                        Directory.CreateDirectory(propValidNewDir.GetArrayElementAtIndex(i).stringValue);

                    AssetDatabase.Refresh();
                    OnSelect();
                }

                using (new EditorGUI.DisabledScope(propValidPathsThatAlreadyExist.arraySize == 0))
                {
                    if (GUILayout.Button("Move Selected Assets to Directories with the Same Name", buttonStyle))
                    {
                        var l = propValidPathsThatAlreadyExist.arraySize;

                        for (int i = 0; i < assetPaths.Length; i++)
                        {
                            var assetPath = assetPaths[i];
                            var fileNameAndFileType = GetFileNameAndType(assetPath);

                            for (int j = 0; j < l; j++)
                            {
                                var dir = propValidPathsThatAlreadyExist.GetArrayElementAtIndex(j).stringValue;
                                var newDirName = dir.TrimEnd('/').Split('/').Last();
                                var newPath = dir + fileNameAndFileType.fileName + fileNameAndFileType.fileType;

                                if (fileNameAndFileType.fileName == newDirName)
                                    AssetDatabase.MoveAsset(assetPath, newPath);
                            }
                        }

                        AssetDatabase.Refresh();
                        OnSelect();
                    }
                }

                var scrollStyle = new GUIStyle("HelpBox");
                scrollStyle.fontSize = 14;
                scrollStyle.alignment = TextAnchor.MiddleLeft;
                scrollStyle.wordWrap = false;

                GUILayout.FlexibleSpace();

                using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos, scrollStyle, GUILayout.Height(400)))
                {
                    GUILayout.Label("Directories to Create", new GUIStyle("Box"));
                    scrollPos = scrollView.scrollPosition;

                    var pathStyle = new GUIStyle("WhiteLabel");
                    pathStyle.fontSize = 14;
                    GUILayout.Label(simplifiedValidNewDirectories, pathStyle);
                }
            }
        }
    }
}