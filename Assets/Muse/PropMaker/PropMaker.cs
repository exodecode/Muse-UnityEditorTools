using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PropMaker : MonoBehaviour
{
    public GameObject baseGameObject;

    public void Spawn(GameObject[] children)
    {
        for (int i = 0; i < children.Length; i++)
        {
            var child = children[i];

            var prop = PrefabUtility.InstantiatePrefab(baseGameObject) as GameObject;
            prop.transform.position = child.transform.position;
            prop.transform.rotation = child.transform.rotation;

            prop.name = child.name;
            child.transform.SetParent(prop.transform);
        }
    }
}

[CustomEditor(typeof(PropMaker))]
public class PropMakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var a = target as PropMaker;
        if (GUILayout.Button("Assign to Dynamic prefabs"))
        {
            var gameObjects = Selection.gameObjects;
            a.Spawn(gameObjects);
        }
    }
}
