
using UnityEditor;
using UnityEngine;

namespace Muse
{
    public class PrefabNameReset
    {
        const string VARIANT_KEYWORD = " Variant";

        [MenuItem("Tools/Muse/Prefab/Reset Name")]
        public static void DoTheThing()
        {
            var selections = Selection.objects;
            var length = selections.Length;

            for (int i = 0; i < length; i++)
            {
                var s = selections[i];

                var prefab = PrefabUtility.GetCorrespondingObjectFromSource(s);
                Debug.Log(prefab.name);

                if (prefab != null)
                {
                    var name = prefab.name;
                    var assetPath = AssetDatabase.GetAssetPath(s);

                    AssetDatabase.RenameAsset(assetPath, name);
                    s.name = name;
                }

            }

            AssetDatabase.Refresh();
        }
    }
}