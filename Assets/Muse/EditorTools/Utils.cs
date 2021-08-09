using UnityEngine;

namespace Muse
{
    public static class Utils
    {
        public static GameObject CreateGameObjectWithMesh(Mesh mesh, Material material)
        {
            var g = new GameObject(mesh.name);
            var mf = g.AddComponent<MeshFilter>();
            var mr = g.AddComponent<MeshRenderer>();
            mr.sharedMaterial = material;
            mf.sharedMesh = mesh;
            return g;
        }
    }
}