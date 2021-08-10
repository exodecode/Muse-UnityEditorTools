// using UnityEngine;
// using UnityEditor;

// namespace Muse
// {
//     public class PropMaker : MonoBehaviour
//     {
//         public GameObject baseGameObject;
//         public string suffix;
//         public bool zeroOutChildTransforms;

//         public void FromSelectedModels(GameObject[] children)
//         {
//             var helper = new GameObject("Helper").AddComponent<GameObjectToolHelper>();

//             for (int i = 0; i < children.Length; i++)
//             {
//                 var child = children[i];

//                 var basePrefab = PrefabUtility.InstantiatePrefab(baseGameObject) as GameObject;
//                 var prop = PrefabUtility.InstantiatePrefab(child) as GameObject;

//                 basePrefab.name = child.name;

//                 prop.transform.SetParent(basePrefab.transform);

//                 var path = AssetDatabase.GetAssetPath(child);
//                 var pathWithName = path.Substring(0, path.LastIndexOf('/') + 1) + child.name + suffix + ".prefab";
//                 var obj = PrefabUtility.SaveAsPrefabAsset(basePrefab, pathWithName);

//                 helper.DestroyImmediateGameObject(basePrefab);
//             }

//             helper.Finish();
//         }

//         public void FromSelectedGameObjects(GameObject[] children)
//         {
//             var helper = new GameObject("Helper").AddComponent<GameObjectToolHelper>();

//             for (int i = 0; i < children.Length; i++)
//             {
//                 var child = children[i];
//                 var basePrefab = PrefabUtility.InstantiatePrefab(baseGameObject) as GameObject;

//                 if (zeroOutChildTransforms)
//                 {
//                     basePrefab.transform.position = child.transform.position;
//                     basePrefab.transform.rotation = child.transform.rotation;
//                 }

//                 var copy = Instantiate<GameObject>(child, basePrefab.transform, false);
//                 copy.name = child.name;

//                 if (zeroOutChildTransforms)
//                 {
//                     copy.transform.localPosition = Vector3.zero;
//                     copy.transform.localRotation = Quaternion.identity;
//                 }

//                 basePrefab.name = copy.name;

//                 var path = "Assets/";
//                 var pathWithName = path.Substring(0, path.LastIndexOf('/') + 1) + child.name + suffix + ".prefab";

//                 basePrefab.transform.position = Vector3.zero;
//                 basePrefab.transform.rotation = Quaternion.identity;

//                 var obj = PrefabUtility.SaveAsPrefabAsset(basePrefab, pathWithName);

//                 var variant = PrefabUtility.InstantiatePrefab(obj) as GameObject;

//                 variant.transform.position = child.transform.position;
//                 variant.transform.rotation = child.transform.rotation;

//                 helper.DestroyImmediateGameObject(basePrefab);
//             }

//             helper.Finish();
//         }
//     }
// }