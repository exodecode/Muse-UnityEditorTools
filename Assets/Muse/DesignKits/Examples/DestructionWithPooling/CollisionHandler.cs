using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    Action<Collision> callback;

    public void Init(Action<Collision> callback) => this.callback = callback;

    private void OnCollisionEnter(Collision other) => callback.Invoke(other);
}
