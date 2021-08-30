using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Muse
{
    public static class FilePathUtils
    {
        public static string GetDirectoryName(string directoryPath) => directoryPath.TrimEnd('/').Split('/').Last();

        public static Object[] GetPersistantObjects(Object[] objects) =>
            objects.Where(obj => EditorUtility.IsPersistent(obj)).ToArray();

        public static string GetFullAssetPath(string assetPath)
        {
            var dataPath = Application.dataPath;
            var dataPathDir = dataPath.TrimAfterLastChar('/');

            return $"{dataPathDir}{assetPath}";
        }

        public static string GetNewDirectoryPath(string fullPath)
        {
            var nameAndType = GetFileNameAndType(fullPath);
            var dir = fullPath.TrimAfterLastChar('/');

            return $"{dir}{nameAndType.fileName}/";
        }

        public static string GetParentDirectory(string filePath) => filePath.TrimAfterLastChar('/');

        public static (string fileName, string fileType) GetFileNameAndType(string path)
        {
            if (!path.Contains('.') || !path.Contains('/'))
                return ("NONE", "NONE");
            else
            {
                var fileNameWithType = path.TrimBeforeLastChar('/', true);
                var fileType = fileNameWithType.Substring(fileNameWithType.LastIndexOf("."));
                var fileName = fileNameWithType.Substring(0, fileNameWithType.LastIndexOf("."));

                return (fileName, fileType);
            }
        }

        public static string[] GetAssetPathsFromSelections(Object[] selections)
        {
            var a = selections
                .Where(selection => EditorUtility.IsPersistent(selection))
                .Select(persistant => AssetDatabase.GetAssetPath(persistant))
                .Where(p => !string.IsNullOrEmpty(p) && p.Contains('.'))
                .Distinct();

            return a.Any() ? a.ToArray() : new string[] { "NONE" };
        }

        public static string[] GetValidDirectoriesThatExistFromAssetPaths(string[] assetPaths) =>
            assetPaths
            .Select(fullPath => GetNewDirectoryPath(fullPath))
            .Where(dirPath => Directory.Exists(dirPath))
            .Distinct()
            .ToArray();
    }
}