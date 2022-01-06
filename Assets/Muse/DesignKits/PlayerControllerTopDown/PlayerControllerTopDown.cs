using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerTopDown : MonoBehaviour
{
    const string VERTICAL = "Vertical";
    const string HORIZONTAL = "Horizontal";

    public float moveSpeed;
    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    Vector3 MoveDirection(bool topDown)
    {
        if (topDown)
            return new Vector3(Input.GetAxis(HORIZONTAL), 0, Input.GetAxis(HORIZONTAL));
        else
            return new Vector3(Input.GetAxis(HORIZONTAL), Input.GetAxis(HORIZONTAL), 0);
    }

    void Update()
    {

    }
}
