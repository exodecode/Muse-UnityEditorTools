using System.Linq;
using UnityEngine;

namespace Muse
{
    public static class PlacementTools
    {
        public static void SetRandomYRotation(Transform[] transforms)
        {
            for (int i = 0; i < transforms.Length; i++)
                transforms[i].rotation =
                    Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        }

        public static void PlaceObjectsInALine(Transform[] transforms)
        {
            var length = transforms.Length;
            var sorted = transforms.OrderBy(a => a.name).ToArray();

            for (int i = 0; i < transforms.Length; i++)
                sorted[i].localPosition = new Vector3(i * 2, 0, 0);
        }
    }
}