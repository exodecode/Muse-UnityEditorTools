using System.Linq;
using UnityEditor;
using UnityEngine;

static class GameObjectTools
{
    [MenuItem("Tools/GameObject/Assign To Parent With Same Name")]
    static void MakeAndAssignToParentWithSameName()
    {
        var transforms = GetSelectedTransforms();

        var length = transforms.Length;

        for (int i = 0; i < length; i++)
        {
            var t = transforms[i];
            if (t.parent == null)
            {
                var p = new GameObject(t.name).transform;
                t.SetParent(p);
            }
        }
    }

    [MenuItem("Tools/GameObject/Place/In A Line")]
    static void PlaceObjectsInALine()
    {
        var transforms = GetSelectedTransforms();
        var length = transforms.Length;
        var sorted = transforms.OrderBy(a => a.name).ToArray();

        for (int i = 0; i < transforms.Length; i++)
            sorted[i].localPosition = new Vector3(i * 2, 0, 0);
    }

    [MenuItem("Tools/GameObject/Sort/Texture")]
    static void SortGameObjectsBasedOnHavingTexture()
    {
        var transforms = GetSelectedTransforms();

        var hasTextureParent = new GameObject("Has Texture").transform;
        var noTextureParent = new GameObject("Needs Texture").transform;

        for (int i = 0; i < transforms.Length; i++)
        {
            var t = transforms[i];
            var renderers = t.GetComponentsInChildren<MeshRenderer>();
            var hasTexture = renderers.All(r => r.sharedMaterial.GetTexture("_MainTex") != null);

            t.SetParent(hasTexture ? hasTextureParent : noTextureParent);
        }
    }

    [MenuItem("Tools/GameObject/Sort/Alpabetically")]
    static void SortGameObjectsAlpabetically()
    {
        var transforms = GetSelectedTransforms();
        var length = transforms.Length;
        var sorted = transforms.OrderBy(a => a.name).ToArray();

        for (int i = 0; i < length; i++)
            sorted[i].SetSiblingIndex(i);
    }

    [MenuItem("Tools/GameObject/Collider/Add Mesh Colliders Based On Child Meshes")]
    static void AddMeshCollidersBasedOnChildMeshes()
    {
        var transforms = GetSelectedTransforms();

        for (int i = 0; i < transforms.Length; i++)
        {
            var t = transforms[i];
            var meshFilters = t.GetComponentsInChildren<MeshFilter>();

            for (int j = 0; j < meshFilters.Length; j++)
            {
                var mc = t.gameObject.AddComponent<MeshCollider>();
                mc.sharedMesh = meshFilters[j].sharedMesh;
            }
        }
    }

    [MenuItem("Tools/GameObject/Collider/Remove All Colliders")]
    static void RemoveAllColliders()
    {
        var transforms = GetSelectedTransforms();
        var go = new GameObject("Helper");
        var helper = go.AddComponent<GameObjectToolHelper>();

        for (int i = 0; i < transforms.Length; i++)
        {
            var t = transforms[i];
            var colliders = t.GetComponentsInChildren<Collider>();
            for (int j = 0; j < colliders.Length; j++)
            {
                var collider = colliders[j];
                helper.DestroyImmediateCollider(collider);
            }
        }

        helper.Finish();
    }

    [MenuItem("Tools/GameObject/Collider/Adjust Box Collider Based Children")]
    static void AddBoxColliderBasedOnChildMeshes()
    {
        var transforms = GetSelectedTransforms();

        for (int i = 0; i < transforms.Length; i++)
        {
            var hasBounds = false;
            var t = transforms[i];
            var renderers = t.GetComponentsInChildren<MeshRenderer>();

            var bc = t.GetComponent<BoxCollider>();
            if (bc == null)
                bc = t.gameObject.AddComponent<BoxCollider>();

            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

            for (int j = 0; j < renderers.Length; j++)
            {
                var renderer = renderers[j];
                if (renderer != null)
                {
                    if (hasBounds)
                        bounds.Encapsulate(renderer.bounds);
                    else
                    {
                        bounds = renderer.bounds;
                        hasBounds = true;
                    }
                }
            }

            bc.center = bounds.center - t.position;
            bc.size = bounds.size;
        }
    }

    public static Transform[] GetSelectedTransforms()
    {
        var gameObjects = Selection.gameObjects;
        var transforms = gameObjects.Select(g => g.transform).ToArray();

        return transforms;
    }
}