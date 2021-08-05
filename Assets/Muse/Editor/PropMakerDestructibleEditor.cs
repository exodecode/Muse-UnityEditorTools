using UnityEditor;
using UnityEngine;

namespace Muse
{
    using static EditorUtils;

    [CustomEditor(typeof(PropMakerDestructible))]
    public class PropMakerDestructibleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var propMakerDestructible = target as PropMakerDestructible;
            var model = propMakerDestructible.model;
            var basePrefab = propMakerDestructible.destroyedBase;
            var nameSuffix = propMakerDestructible.destroyedSuffix;

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Destructible Prop Prefab From Model Asset"))
                CreateDestructiblePropPrefabFromModelAsset(model, basePrefab, nameSuffix);
        }
    }
}