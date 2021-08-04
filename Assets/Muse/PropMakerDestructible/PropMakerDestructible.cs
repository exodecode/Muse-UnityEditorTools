using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Muse
{
    public class PropMakerDestructible : MonoBehaviour
    {
        public GameObject model;
        public GameObject baseGameObject;

        public int intactPropIndex = 0;
        public string suffix;

        public void CreateDestructiblePropPrefabsFromModelAsset()
        {
            var meshFilters = model.GetComponentsInChildren<MeshFilter>();
            var meshes = meshFilters.Select(meshFilter => meshFilter.sharedMesh);
            var intactMesh = meshes.Skip(intactPropIndex).First();
            var cellMeshes = meshes.Where(mesh => mesh != intactMesh);

            var material = meshFilters[intactPropIndex].GetComponent<Renderer>().sharedMaterial;

            var intact = CreateGameObjectWithMesh(intactMesh, material);
            var cells = cellMeshes.Select(cellMesh => CreateGameObjectWithMesh(cellMesh, material)).ToList();

            cells.ForEach(cell =>
            {
                var mc = cell.AddComponent<MeshCollider>();
                mc.convex = true;
                cell.AddComponent<Rigidbody>();
            });

            var assetPath = AssetDatabase.GetAssetPath(model);

            var intactVariantGroup =
                CreatePrefabVariantFromGameObjectAndBase(
                    new GameObject[1] { intact },
                    baseGameObject,
                    assetPath,
                    intact.name,
                    suffix);

            var destroyedVariantGroup =
                CreatePrefabVariantFromGameObjectAndBase(
                    cells.ToArray(),
                    baseGameObject,
                    assetPath,
                    intact.name + "Destroyed",
                    suffix);

            var intactVariant = intactVariantGroup.variant;
            var destroyedVariant = destroyedVariantGroup.variant;

            var helper = new GameObject("Helper").AddComponent<GameObjectToolHelper>();
            helper.DestroyImmediateGameObject(intactVariantGroup.extra);
            helper.DestroyImmediateGameObject(destroyedVariantGroup.extra);
            helper.Finish();

            AssetDatabase.Refresh();
        }

        static (GameObject variant, GameObject extra) CreatePrefabVariantFromGameObjectAndBase(
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

            var pathWithName = assetPath.Substring(0, assetPath.LastIndexOf('/') + 1) + name + "_" + suffix + ".prefab";
            var variant = PrefabUtility.SaveAsPrefabAsset(basePrefab, pathWithName);

            return (variant, basePrefab);
        }

        static GameObject CreateGameObjectWithMesh(Mesh mesh, Material material)
        {
            var g = new GameObject(mesh.name);
            var mf = g.AddComponent<MeshFilter>();
            var mr = g.AddComponent<MeshRenderer>();
            mr.sharedMaterial = material;
            mf.sharedMesh = mesh;
            return g;
        }
    }

    [CustomEditor(typeof(PropMakerDestructible))]
    public class PropMakerDestructibleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var a = target as PropMakerDestructible;

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Destructible Prop Prefabs From Model Asset"))
                a.CreateDestructiblePropPrefabsFromModelAsset();
        }
    }
}