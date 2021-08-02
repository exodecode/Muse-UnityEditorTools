using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Muse
{
    public static class EditorUtil
    {
        public static Transform[] GetSelectedTransforms() =>
            Selection.gameObjects.Select(g => g.transform).ToArray();
    }
}