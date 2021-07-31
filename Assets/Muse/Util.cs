using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Muse
{
    public static class EditorUtil
    {
        public static Transform[] GetSelectedTransforms()
        {
            var gameObjects = Selection.gameObjects;
            var transforms = gameObjects.Select(g => g.transform).ToArray();

            return transforms;
        }
    }
}