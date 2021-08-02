using System.Linq;
using UnityEditor;
using UnityEngine;
using static Muse.EditorUtil;

namespace Muse
{
    public static class PlacementTools
    {
        [MenuItem("Tools/Muse/Placement/Set Random Y Rotation")]
        public static void SetRandomYRotation()
        {
            var gameObjects = Selection.gameObjects;
            for (int i = 0; i < gameObjects.Length; i++)
                gameObjects[i].transform.rotation =
                    Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        }

        [MenuItem("Tools/Muse/Placement/Place In A Line")]
        static void PlaceObjectsInALine()
        {
            var transforms = GetSelectedTransforms();
            var length = transforms.Length;
            var sorted = transforms.OrderBy(a => a.name).ToArray();

            for (int i = 0; i < transforms.Length; i++)
                sorted[i].localPosition = new Vector3(i * 2, 0, 0);
        }
    }
}