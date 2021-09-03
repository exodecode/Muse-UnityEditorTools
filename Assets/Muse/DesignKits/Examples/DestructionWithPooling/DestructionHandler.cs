using System;
using UnityEngine;

public class DestructionHandler : MonoBehaviour
{
    [Serializable]
    public class CollisionHandlerGroup
    {
        public CollisionHandler[] collisionHandlers;
    }

    public CollisionHandlerGroup[] collisionHandlerGroups;
    public ObjectPooler objectPooler;

    private void Awake()
    {
        for (int i = 0; i < collisionHandlerGroups.Length; i++)
        {
            var group = collisionHandlerGroups[i];
            var handlers = group.collisionHandlers;
            for (int j = 0; j < handlers.Length; j++)
            {
                var handler = handlers[j];
                var index = i;
                Action<Collision> callback = (c) =>
                {
                    var pos = handler.transform.position;
                    var rot = handler.transform.rotation;
                    handler.gameObject.SetActive(false);
                    var g = objectPooler.DrawFromPool(index);

                    var rbs = g.GetComponentsInChildren<Rigidbody>();
                    for (int k = 0; k < rbs.Length; k++)
                    {
                        var rb = rbs[k];
                        rb.transform.localPosition = Vector3.zero;
                        rb.transform.localRotation = Quaternion.identity;
                    }

                    g.transform.position = pos;
                    g.transform.rotation = rot;
                };
                // Debug.Log(i);
                handler.Init(callback);
            }
        }
    }
}
