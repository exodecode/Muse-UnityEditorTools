using UnityEngine;
using System.Linq;

namespace Muse
{
    public static class GameObjectTools
    {
        public static void MakeAndAssignToParentWithSameName(Transform[] transforms)
        {
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

        public static void SortGameObjectsBasedOnHavingTexture(Transform[] transforms)
        {
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

        public static void SortGameObjectsAlpabetically(Transform[] transforms)
        {
            var length = transforms.Length;
            var sorted = transforms.OrderBy(a => a.name).ToArray();

            for (int i = 0; i < length; i++)
                sorted[i].SetSiblingIndex(i);
        }

        public static void AddMeshCollidersBasedOnChildMeshes(Transform[] transforms)
        {
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

        public static void RemoveAllColliders(Transform[] transforms)
        {
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

        public static void AddBoxColliderBasedOnChildMeshes(Transform[] transforms)
        {
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
    }
}