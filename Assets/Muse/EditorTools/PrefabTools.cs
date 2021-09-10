using UnityEditor;
using UnityEngine;

namespace Muse
{
    public class PrefabTools : MonoBehaviour
    {
        // public string[] prefabDirectories;

        public void GrabAllPrefabsFromDirectories()
        {
            // var selectedGameObjects = Selection.gameObjects;
            // var length = selectedGameObjects.Length;

            // for (int i = 0; i < length; i++)
            // {
            //     var selection = selectedGameObjects[i];
            //     var prefabParent = PrefabUtility.GetCorrespondingObjectFromSource(selection);

            //     if (prefabParent != null)
            //     {
            //         var path = AssetDatabase.GetAssetPath(prefabParent);
            //         var dir = path.Substring(0, path.LastIndexOf('/') + 1);
            //         bool inDirectory = false;

            //         for (int j = 0; j < prefabDirectories.Length; j++)
            //         {
            //             if (dir == prefabDirectories[j])
            //             {
            //                 inDirectory = true;
            //                 break;
            //             }
            //         }

            //         if (inDirectory)
            //             selection.transform.SetParent(transform);
            //     }
            // }
        }
    }
}
