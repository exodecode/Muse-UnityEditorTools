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
            var meshRenderersAndTransform = transforms.Select(t => (meshRenderers: t.GetComponentsInChildren<MeshRenderer>(), transform: t));

            var sorted =
                meshRenderersAndTransform
                .OrderByDescending(pair => pair.meshRenderers.All(mr => mr.sharedMaterial.GetTexture("_MainTex") != null))
                .Select(p => p.transform)
                .ToArray();

            for (int i = 0; i < sorted.Length; i++)
                sorted[i].SetSiblingIndex(i);
        }

        public static void SortGameObjectsAlpabetically(Transform[] transforms)
        {
            var length = transforms.Length;
            var sorted = transforms.OrderBy(a => a.name).ToArray();

            for (int i = 0; i < length; i++)
                sorted[i].SetSiblingIndex(i);
        }
    }
}