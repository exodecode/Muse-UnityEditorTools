using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling2 : MonoBehaviour
{
    public GameObject gameObjectToPool;
    public int maxActive;

    Queue<GameObject> goQueue;

    void Awake()
    {
        goQueue = CreateQueue(gameObjectToPool, maxActive);
    }

    GameObject DrawFromPool()
    {
        var g = goQueue.Dequeue();
        ResetGameObject(g);
        g.SetActive(true);
        goQueue.Enqueue(g);
        return g;
    }

    static void DisableObject(Queue<GameObject> q, GameObject g)
    {
        ResetGameObject(g);
        g.SetActive(false);
    }

    static Queue<GameObject> CreateQueue(GameObject go, int amount)
    {
        var q = new Queue<GameObject>();

        for (int i = 0; i < amount; i++)
        {
            var g = Instantiate(go);
            g.SetActive(false);
            q.Enqueue(g);
        }

        return q;
    }

    static void ResetGameObject(GameObject g)
    {
        var t = g.transform;
        t.position = Vector3.zero;
        t.rotation = Quaternion.identity;
        t.localScale = Vector3.one;
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var g = DrawFromPool();
            g.transform.position = new Vector3(0, 10, 0);
        }
    }
#endif
}