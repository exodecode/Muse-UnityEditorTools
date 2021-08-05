using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Muse
{
    [CreateAssetMenu(fileName = "SceneProfile", menuName = "EditorTools/SceneProfile", order = 0)]
    public class SceneProfile : ScriptableObject
    {
        public SceneAsset activeScene;
        public List<SceneAsset> additiveScenes = new List<SceneAsset>();
    }
}