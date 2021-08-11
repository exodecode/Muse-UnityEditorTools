using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Muse
{
    using static Utils;

    public static class EditorUtils
    {
        public static Transform[] GetSelectedTransforms() =>
            Selection.gameObjects.Select(g => g.transform).ToArray();

        // public static void FoldersForSelected(GameObject[] children)
        // {
        //     for (int i = 0; i < children.Length; i++)
        //     {
        //         var child = children[i];
        //         var dir = child.name + '/';
        //         var dataPath = Application.dataPath;
        //         var assetPath = AssetDatabase.GetAssetPath(child);
        //         var nameWithFileType = assetPath.Substring(assetPath.LastIndexOf('/') + 1, assetPath.Length - (assetPath.LastIndexOf('/') + 1));
        //         var fullPath = dataPath + assetPath.Substring(assetPath.IndexOf('/'), assetPath.Length - assetPath.IndexOf('/'));
        //         var directoryPath = fullPath.Substring(0, fullPath.LastIndexOf('/') + 1);

        //         var parentDirecory = Path.GetDirectoryName(assetPath).Replace('\\', '/');
        //         var parentDirecoryName = parentDirecory.Substring(parentDirecory.LastIndexOf('/') + 1, parentDirecory.Length - (parentDirecory.LastIndexOf('/') + 1));
        //         var localPath = assetPath.Substring(0, assetPath.LastIndexOf('/'));

        //         if (parentDirecoryName != child.name)
        //         {
        //             var newDirectoryPath = directoryPath + dir;
        //             System.IO.Directory.CreateDirectory(newDirectoryPath);
        //         }
        //         else
        //             Debug.LogWarning(nameWithFileType + " is already in a folder with the same name!");
        //     }

        //     AssetDatabase.Refresh();

        //     for (int i = 0; i < children.Length; i++)
        //     {
        //         var child = children[i];
        //         var dir = child.name + '/';
        //         var assetPath = AssetDatabase.GetAssetPath(child);
        //         var nameWithFileType = assetPath.Substring(assetPath.LastIndexOf('/') + 1, assetPath.Length - (assetPath.LastIndexOf('/') + 1));
        //         var newAssetPath = assetPath.Substring(0, assetPath.LastIndexOf('/') + 1) + dir + nameWithFileType;

        //         AssetDatabase.MoveAsset(assetPath, newAssetPath);
        //     }

        //     AssetDatabase.Refresh();
        // }

        // public static (GameObject variant, GameObject extra) CreatePrefabVariantFromGameObjectAndBase(
        //     GameObject[] go,
        //     GameObject baseGameObject,
        //     string assetPath,
        //     string name,
        //     string suffix)
        // {
        //     var basePrefab = PrefabUtility.InstantiatePrefab(baseGameObject) as GameObject;

        //     basePrefab.name = name;

        //     for (int i = 0; i < go.Length; i++)
        //     {
        //         var g = go[i];
        //         g.transform.SetParent(basePrefab.transform);
        //         g.layer = basePrefab.layer;
        //     }

        //     var pathWithName = assetPath.Substring(0, assetPath.LastIndexOf('/') + 1) + name + suffix + ".prefab";
        //     var variant = PrefabUtility.SaveAsPrefabAsset(basePrefab, pathWithName);

        //     return (variant, basePrefab);
        // }

        // public static void CreateDestructiblePropPrefabFromModelAsset(
        //     GameObject model,
        //     GameObject baseDestructiblePropPrefab,
        //     string nameSuffix)
        // {
        //     var meshFilters = model.GetComponentsInChildren<MeshFilter>();
        //     var meshes = meshFilters.Select(meshFilter => meshFilter.sharedMesh);

        //     var material = meshFilters[0].GetComponent<Renderer>().sharedMaterial;

        //     var cells = meshes.Select(mesh => CreateGameObjectWithMesh(mesh, material)).ToList();

        //     cells.ForEach(cell =>
        //     {
        //         var mc = cell.AddComponent<MeshCollider>();
        //         mc.convex = true;
        //         cell.AddComponent<Rigidbody>();
        //     });

        //     var assetPath = AssetDatabase.GetAssetPath(model);

        //     var destroyedVariantGroup =
        //         CreatePrefabVariantFromGameObjectAndBase(
        //             cells.ToArray(),
        //             baseDestructiblePropPrefab,
        //             assetPath,
        //             model.name,
        //             nameSuffix);

        //     var destroyedVariant = destroyedVariantGroup.variant;
        //     var helper = new GameObject("Helper").AddComponent<GameObjectToolHelper>();

        //     helper.DestroyImmediateGameObject(destroyedVariantGroup.extra);
        //     helper.Finish();

        //     AssetDatabase.Refresh();
        // }
    }
}