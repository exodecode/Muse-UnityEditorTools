using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Collections.Generic;

public class FolderMakerEditorWindow : EditorWindow
{
    public List<string> acceptedFileTypes = new List<string>() { ".fbx", ".obj", ".prefab", ".unity", ".asset" };
    Vector2 scrollPos;

    [MenuItem("Tools/Muse/Folder Maker")]
    private static void ShowWindow() => GetWindow<FolderMakerEditorWindow>("Folder Maker");

    void OnEnable() => Selection.selectionChanged += Repaint;
    void OnDisable() => Selection.selectionChanged -= Repaint;

    void OnGUI()
    {
        EditorGUILayout.Space();

        var persistantSelections =
            GetPersistantObjects(Selection.objects);

        var assetPaths =
            persistantSelections
            .Select(persistantSelection => AssetDatabase.GetAssetPath(persistantSelection))
            .ToArray();

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

        using (new EditorGUI.DisabledScope(validNewDirectories.Length == 0))
        {
            var buttonStyle = new GUIStyle("Button");
            buttonStyle.fontSize = 14;

            if (GUILayout.Button("Create Folders for Selected Assets", buttonStyle))
            {
                for (int i = 0; i < validNewDirectories.Length; i++)
                    Directory.CreateDirectory(validNewDirectories[i]);

                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Move Selected Assets to Valid Directories Based on Name", buttonStyle))
            {
                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    var obj = Selection.objects[i];
                    var assetPath = AssetDatabase.GetAssetPath(obj);
                    var parentDir = TrimToLastSlash(assetPath);
                    var fileNameAndFileType = GetFileNameAndType(assetPath);

                    for (int j = 0; j < simplifiedValidNewDirectories.Length; j++)
                    {
                        var newDir = simplifiedValidNewDirectories[j];
                        var newDirName = newDir.TrimEnd('/').Split('/').Last();
                        var newPath = newDir + fileNameAndFileType.fileName + fileNameAndFileType.fileType;

                        if (fileNameAndFileType.fileName == newDirName)
                            AssetDatabase.MoveAsset(assetPath, newPath);
                    }
                }

                AssetDatabase.Refresh();
            }

            var scrollStyle = new GUIStyle("HelpBox");
            scrollStyle.fontSize = 14;
            scrollStyle.alignment = TextAnchor.MiddleLeft;
            scrollStyle.wordWrap = false;

            GUILayout.FlexibleSpace();

            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos, scrollStyle, GUILayout.Height(150)))
            {
                GUILayout.Label("Directories to Create", new GUIStyle("Box"));
                scrollPos = scrollView.scrollPosition;

                var pathStyle = new GUIStyle("WhiteLabel");
                pathStyle.fontSize = 14;
                GUILayout.Label(FlattenStringArray(simplifiedValidNewDirectories), pathStyle);
            }
        }
    }

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