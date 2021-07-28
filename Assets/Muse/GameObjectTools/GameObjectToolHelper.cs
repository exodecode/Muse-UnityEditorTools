using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectToolHelper : MonoBehaviour
{
    public void DestroyImmediateCollider(Collider col)
    {
        DestroyImmediate(col);
    }
    public void DestroyImmediateGameObject(GameObject g)
    {
        DestroyImmediate(g);
    }

    public void Finish()
    {
        DestroyImmediate(gameObject);
    }
}
