using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PropMaker : MonoBehaviour
{
    public GameObject baseGameObject;

    public void Spawn(GameObject[] children)
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
            var pathWithName = path.Substring(0, path.LastIndexOf('/') + 1) + child.name + ".prefab";
            var obj = PrefabUtility.SaveAsPrefabAsset(basePrefab, pathWithName);

            helper.DestroyImmediateGameObject(basePrefab);
        }

        helper.Finish();
    }
}

[CustomEditor(typeof(PropMaker))]
public class PropMakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var a = target as PropMaker;
        if (GUILayout.Button("Make Prefab Variant"))
        {
            var gameObjects = Selection.gameObjects;
            a.Spawn(gameObjects);
        }
    }
}
