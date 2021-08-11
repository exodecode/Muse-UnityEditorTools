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