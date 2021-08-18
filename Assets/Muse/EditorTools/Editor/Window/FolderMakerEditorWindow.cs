using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace Muse
{
    using static ShortcutKeys;

    public class FolderMakerEditorWindow : EditorWindow
    {
        public List<string> acceptedFileTypes = new List<string>() { ".fbx", ".obj", ".prefab", ".unity", ".asset" };
        Vector2 scrollPos;
        public string[] validPathsThatAlreadyExist;

        SerializedObject so;
        SerializedProperty propValidPathsThatAlreadyExist;

        [MenuItem("Tools/Muse/Folder Maker" + SHORTCUT_WINDOW_FOLDER)]
        private static void ShowWindow() => GetWindow<FolderMakerEditorWindow>("Folder Maker");

        void OnEnable()
        {
            so = new SerializedObject(this);
            propValidPathsThatAlreadyExist = so.FindProperty("validPathsThatAlreadyExist");

            Selection.selectionChanged += Repaint;
        }
        void OnDisable() => Selection.selectionChanged -= Repaint;

        void OnGUI()
        {
            EditorGUILayout.PropertyField(propValidPathsThatAlreadyExist);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            var assetPaths = GetAssetPathsFromSelections(Selection.objects);

            var fullAssetPaths =
                assetPaths
                .Select(assetPath => GetFullAssetPath(assetPath))
                .ToArray();

            var validNewDirectories =
                fullAssetPaths
                .Where(p => !string.IsNullOrEmpty(p) && p.Contains('.'))
                .Where(path => acceptedFileTypes.Contains(GetFileNameAndType(path).fileType))
                .Select(path => (parent: GetParentDirectory(path), newDirectory: GetNewDirectoryPath(path)))
                .Distinct()
                .Where(pair => GetDirectoryName(pair.parent) != GetDirectoryName(pair.newDirectory))
                .Select(p => p.newDirectory)
                .ToArray();

            var simplifiedValidNewDirectories =
                validNewDirectories
                .Select(path => path.Substring(Application.dataPath.LastIndexOf('/') + 1))
                .ToArray();


            var buttonStyle = new GUIStyle("Button");
            buttonStyle.fontSize = 14;

            using (new EditorGUI.DisabledScope(Selection.objects.Length == 0))
            {
                if (GUILayout.Button("Create Folders for Selected Assets", buttonStyle))
                {
                    for (int i = 0; i < validNewDirectories.Length; i++)
                        Directory.CreateDirectory(validNewDirectories[i]);

                    AssetDatabase.Refresh();
                }

                using (new EditorGUI.DisabledScope(propValidPathsThatAlreadyExist.arraySize == 0))
                {
                    if (GUILayout.Button("Move Selected Assets to Directories with the Same Name", buttonStyle))
                    {
                        var l = propValidPathsThatAlreadyExist.arraySize;

                        for (int i = 0; i < assetPaths.Length; i++)
                        {
                            var assetPath = assetPaths[i];
                            var parentDir = TrimToLastSlash(assetPath);
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
                    }
                }

                if (GUILayout.Button("Search For Matching Directories", buttonStyle))
                {
                    var v = GetValidDirectoriesThatExistFromAssetPaths(assetPaths);
                    propValidPathsThatAlreadyExist.arraySize = v.Length;

                    for (int i = 0; i < v.Length; i++)
                        propValidPathsThatAlreadyExist.GetArrayElementAtIndex(i).stringValue = v[i];
                }
            }

            var scrollStyle = new GUIStyle("HelpBox");
            scrollStyle.fontSize = 14;
            scrollStyle.alignment = TextAnchor.MiddleLeft;
            scrollStyle.wordWrap = false;

            GUILayout.FlexibleSpace();

            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos, scrollStyle))
            {
                GUILayout.Label("Directories to Create", new GUIStyle("Box"));
                scrollPos = scrollView.scrollPosition;

                var pathStyle = new GUIStyle("WhiteLabel");
                pathStyle.fontSize = 14;
                GUILayout.Label(FlattenStringArray(simplifiedValidNewDirectories), pathStyle);
            }
        }

        static string[] GetAssetPathsFromSelections(Object[] selections) =>
            selections
                .Where(selection => EditorUtility.IsPersistent(selection))
                .Select(persistant => AssetDatabase.GetAssetPath(persistant))
                .Where(p => !string.IsNullOrEmpty(p) && p.Contains('.'))
                .ToArray();

        static string[] GetValidDirectoriesThatExistFromAssetPaths(string[] assetPaths) =>
            assetPaths
                .Select(fullPath => GetNewDirectoryPath(fullPath))
                .Where(dirPath => Directory.Exists(dirPath))
                .Distinct()
                .ToArray();

        static string GetDirectoryName(string directoryPath) => directoryPath.TrimEnd('/').Split('/').Last();

        static IEnumerable<Object> GetPersistantObjects(Object[] objects) =>
            objects.Where(obj => EditorUtility.IsPersistent(obj));

        static string GetFullAssetPath(string assetPath)
        {
            var dataPath = Application.dataPath;
            var dataPathDir = TrimToLastSlash(dataPath);

            return $"{dataPathDir}{assetPath}";
        }

        static string GetNewDirectoryPath(string fullPath)
        {
            var nameAndType = GetFileNameAndType(fullPath);
            var dir = TrimToLastSlash(fullPath);

            return $"{dir}{nameAndType.fileName}/";
        }

        static string GetParentDirectory(string filePath)
        {
            var parentDir = TrimToLastSlash(filePath);
            return parentDir;
        }

        static (string fileName, string fileType) GetFileNameAndType(string path)
        {
            var fileNameWithType = path.Substring(path.LastIndexOf("/") + 1);
            var fileType = fileNameWithType.Substring(fileNameWithType.LastIndexOf("."));
            var fileName = fileNameWithType.Substring(0, fileNameWithType.LastIndexOf("."));

            return (fileName, fileType);
        }

        static string FlattenStringArray(string[] array) =>
            string.Join("\n", array);

        static string TrimToLastSlash(string s) => s.Substring(0, s.LastIndexOf('/') + 1);
    }
}