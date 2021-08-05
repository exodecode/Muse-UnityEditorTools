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

        public static (GameObject variant, GameObject extra) CreatePrefabVariantFromGameObjectAndBase(
            GameObject[] go,
            GameObject baseGameObject,
            string assetPath,
            string name,
            string suffix)
        {
            var basePrefab = PrefabUtility.InstantiatePrefab(baseGameObject) as GameObject;

            basePrefab.name = name;

            for (int i = 0; i < go.Length; i++)
            {
                var g = go[i];
                g.transform.SetParent(basePrefab.transform);
                g.layer = basePrefab.layer;
            }

            var pathWithName = assetPath.Substring(0, assetPath.LastIndexOf('/') + 1) + name + ".prefab";
            var variant = PrefabUtility.SaveAsPrefabAsset(basePrefab, pathWithName);

            return (variant, basePrefab);
        }

        public static void CreateDestructiblePropPrefabFromModelAsset(GameObject model, GameObject baseDestructiblePropPrefab, string nameSuffix = "")
        {
            var meshFilters = model.GetComponentsInChildren<MeshFilter>();
            var meshes = meshFilters.Select(meshFilter => meshFilter.sharedMesh);

            var material = meshFilters[0].GetComponent<Renderer>().sharedMaterial;

            var cells = meshes.Select(mesh => CreateGameObjectWithMesh(mesh, material)).ToList();

            cells.ForEach(cell =>
            {
                var mc = cell.AddComponent<MeshCollider>();
                mc.convex = true;
                cell.AddComponent<Rigidbody>();
            });

            var assetPath = AssetDatabase.GetAssetPath(model);

            var destroyedVariantGroup =
                CreatePrefabVariantFromGameObjectAndBase(
                    cells.ToArray(),
                    baseDestructiblePropPrefab,
                    assetPath,
                    model.name,
                    nameSuffix);

            var destroyedVariant = destroyedVariantGroup.variant;
            var helper = new GameObject("Helper").AddComponent<GameObjectToolHelper>();

            helper.DestroyImmediateGameObject(destroyedVariantGroup.extra);
            helper.Finish();

            AssetDatabase.Refresh();
        }

        public static void ReplaceSelected(Transform[] transforms, GameObject prefabReplacement, Transform parentTransform)
        {
            var selectedGameObjects = Selection.gameObjects;
            var length = selectedGameObjects.Length;
            var replacementsParent = new GameObject("[Replacements]");

            for (int i = 0; i < length; i++)
            {
                var go = selectedGameObjects[i];
                go.transform.SetParent(parentTransform);

                var position = go.transform.position;
                var rotation = go.transform.rotation;
                var replacement = PrefabUtility.InstantiatePrefab(prefabReplacement) as GameObject;

                replacement.transform.position = position;
                replacement.transform.rotation = rotation;

                replacement
                    .transform
                    .SetParent(replacementsParent.transform);
            }
        }
    }
}