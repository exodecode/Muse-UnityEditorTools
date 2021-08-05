using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace Muse
{
    [CreateAssetMenu(fileName = "SceneProfile", menuName = "EditorTools/SceneProfile", order = 0)]
    public class SceneProfile : ScriptableObject
    {
        public SceneAsset activeScene;
        public List<SceneAsset> additiveScenes = new List<SceneAsset>();
    }
}