using UnityEditor;
using UnityEngine;

public static class PlacementTools
{
    [MenuItem("Tools/PropPlacement/Set Random Y Rotation")]
    public static void SetRandomYRotation()
    {
        var gameObjects = Selection.gameObjects;
        for (int i = 0; i < gameObjects.Length; i++)
            gameObjects[i].transform.rotation =
                Quaternion.Euler(0, Random.Range(0f, 360f), 0);
    }
}