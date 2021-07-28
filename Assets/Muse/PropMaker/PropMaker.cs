using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

// [ExecuteInEditMode]
public class PropMaker : MonoBehaviour
{
    public GameObject baseGameObject;
    public string suffix;
    public bool zeroOutChildTransforms;

    public void FromSelectedModels(GameObject[] children)
    {
        var helper = new GameObject("Helper").AddComponent<GameObjectToolHelper>();

        for (int i = 0; i < children.Length; i++)
        {
            var child = children[i];

            var basePrefab = PrefabUtility.InstantiatePrefab(baseGameObject) as GameObject;
            var prop = PrefabUtility.InstantiatePrefab(child) as GameObject;

            basePrefab.name = child.name;

            prop.transform.SetParent(basePrefab.transform);

            var path = AssetDatabase.GetAssetPath(child);
            var pathWithName = path.Substring(0, path.LastIndexOf('/') + 1) + child.name + suffix + ".prefab";
            var obj = PrefabUtility.SaveAsPrefabAsset(basePrefab, pathWithName);

            helper.DestroyImmediateGameObject(basePrefab);
        }

        helper.Finish();
    }

    public void FromSelectedGameObjects(GameObject[] children)
    {
        var helper = new GameObject("Helper").AddComponent<GameObjectToolHelper>();

        for (int i = 0; i < children.Length; i++)
        {
            var child = children[i];
            var basePrefab = PrefabUtility.InstantiatePrefab(baseGameObject) as GameObject;
            if (zeroOutChildTransforms)
            {
                basePrefab.transform.position = child.transform.position;
                basePrefab.transform.rotation = child.transform.rotation;
            }

            var copy = Instantiate<GameObject>(child, basePrefab.transform, false);

            copy.name = child.name;

            if (zeroOutChildTransforms)
            {
                // basePrefab.transform.position = copy.transform.position;
                // basePrefab.transform.rotation = copy.transform.rotation;
                copy.transform.localPosition = Vector3.zero;
                copy.transform.localRotation = Quaternion.identity;
            }

            // // PrefabUtility.InstantiatePrefab(child);
            // // child.transform.SetParent(basePrefab.transform);

            // copy.transform.SetParent(basePrefab.transform);
            // copy.transform.position = child.transform.position;
            // copy.transform.rotation = child.transform.rotation;

            // // child.transform.position = Vector3.zero;
            // // child.transform.rotation = Quaternion.identity;
            // // Debug.Log(child.name);
            // // var copy = PrefabUtility.InstantiatePrefab(child) as GameObject;

            basePrefab.name = copy.name;

            var path = "Assets/";
            var pathWithName = path.Substring(0, path.LastIndexOf('/') + 1) + child.name + suffix + ".prefab";

            var obj = PrefabUtility.SaveAsPrefabAsset(basePrefab, pathWithName);

            var variant = PrefabUtility.InstantiatePrefab(obj) as GameObject;
            variant.transform.position = basePrefab.transform.position;
            variant.transform.rotation = basePrefab.transform.rotation;

            helper.DestroyImmediateGameObject(basePrefab);
        }

        helper.Finish();
    }


    public void FoldersForSelected(GameObject[] children)
    {
        for (int i = 0; i < children.Length; i++)
        {
            var child = children[i];
            var dir = child.name + '/';
            var dataPath = Application.dataPath;
            var assetPath = AssetDatabase.GetAssetPath(child);
            var nameWithFileType = assetPath.Substring(assetPath.LastIndexOf('/') + 1, assetPath.Length - (assetPath.LastIndexOf('/') + 1));
            var fullPath = dataPath + assetPath.Substring(assetPath.IndexOf('/'), assetPath.Length - assetPath.IndexOf('/'));
            var directoryPath = fullPath.Substring(0, fullPath.LastIndexOf('/') + 1);

            var parentDirecory = Path.GetDirectoryName(assetPath).Replace('\\', '/');
            var parentDirecoryName = parentDirecory.Substring(parentDirecory.LastIndexOf('/') + 1, parentDirecory.Length - (parentDirecory.LastIndexOf('/') + 1));
            var localPath = assetPath.Substring(0, assetPath.LastIndexOf('/'));

            if (parentDirecoryName != child.name)
            {
                var newDirectoryPath = directoryPath + dir;
                System.IO.Directory.CreateDirectory(newDirectoryPath);
            }
            else
                Debug.LogWarning(nameWithFileType + " is already in a folder with the same name!");
        }

        AssetDatabase.Refresh();

        for (int i = 0; i < children.Length; i++)
        {
            var child = children[i];
            var dir = child.name + '/';
            var assetPath = AssetDatabase.GetAssetPath(child);
            var nameWithFileType = assetPath.Substring(assetPath.LastIndexOf('/') + 1, assetPath.Length - (assetPath.LastIndexOf('/') + 1));
            var newAssetPath = assetPath.Substring(0, assetPath.LastIndexOf('/') + 1) + dir + nameWithFileType;

            AssetDatabase.MoveAsset(assetPath, newAssetPath);
        }

        AssetDatabase.Refresh();
    }
}

[CustomEditor(typeof(PropMaker))]
public class PropMakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var a = target as PropMaker;
        if (GUILayout.Button("Make Prefab Variant From Selected Models"))
        {
            var gameObjects = Selection.gameObjects;
            a.FromSelectedModels(gameObjects);
        }
        if (GUILayout.Button("Make Prefab Variant From Selected GameObjects"))
        {
            var gameObjects = Selection.gameObjects;
            a.FromSelectedGameObjects(gameObjects);
        }
        if (GUILayout.Button("Create Folder(s) for selected"))
        {
            var gameObjects = Selection.gameObjects;
            a.FoldersForSelected(gameObjects);
        }
    }
}
