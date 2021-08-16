using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Muse
{
    public static class EditorUtils
    {
        public static Transform[] GetSelectedTransforms() =>
            Selection.gameObjects.Select(g => g.transform).Where(t => t != null).ToArray();
    }
}