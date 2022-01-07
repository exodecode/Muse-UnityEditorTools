using UnityEngine;

public class PlayerControllerTopDown : MonoBehaviour
{
    const string VERTICAL = "Vertical";
    const string HORIZONTAL = "Horizontal";

    public float moveSpeed = 10;
    public bool topDown = true;

    Camera cam;
    Rigidbody rb;

    public float smoothSpeed;
    Vector3 cameraOffset;

    void Start()
    {
        cameraOffset = cam.transform.position - transform.position;
    }

    void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
    }

    Vector3 MoveDirection(bool topDown) => topDown ? new Vector3(Input.GetAxis(HORIZONTAL), 0, Input.GetAxis(VERTICAL)) : new Vector3(Input.GetAxis(HORIZONTAL), Input.GetAxis(VERTICAL), 0);

    void Update()
    {
    }

    void LateUpdate()
    {
        //camera follow code
    }

    void LookTowardMouseCursor()
    {

    }

    void SmoothFollow()
    {
        var targetPos = transform.position + cameraOffset;
        var smoothFollow = Vector3.Lerp(cam.transform.position, targetPos, smoothSpeed);
        cam.transform.position = smoothFollow;
        //cam.transform.LookAt(transform);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + (MoveDirection(topDown) * moveSpeed * Time.fixedDeltaTime));
        SmoothFollow();
    }
}
