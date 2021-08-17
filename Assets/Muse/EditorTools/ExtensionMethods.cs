using UnityEngine;

namespace Muse
{
    public static class ExtensionMethods
    {
        public static Vector3 Round(this Vector3 v)
        {
            v.x = Mathf.Round(v.x);
            v.y = Mathf.Round(v.y);
            v.z = Mathf.Round(v.z);
            return v;
        }

        public static Vector3 Round(this Vector3 v, float size) => (v / size).Round() * size;
        public static float Round(this float v, float size) => Mathf.Round(v / size) * size;
    }
}