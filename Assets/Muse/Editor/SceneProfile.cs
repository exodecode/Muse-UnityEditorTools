using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Muse
{
    [CreateAssetMenu(fileName = "SceneProfile", menuName = "EditorTools/SceneProfile", order = 0)]
    public class SceneProfile : ScriptableObject
    {
        public SceneAsset activeScene;
        public List<SceneAsset> additiveScenes = new List<SceneAsset>();
    }
}