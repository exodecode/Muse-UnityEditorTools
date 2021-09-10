using Muse;
using UnityEngine;

public class ExampleObjectPooler : MonoBehaviour
{
    public ObjectPooler objectPooler;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Spawn(0);
        else if (Input.GetMouseButtonDown(1))
            Spawn(1);
        else if (Input.GetKeyDown(KeyCode.Space))
            Spawn(2);
        else if (Input.GetKeyDown(KeyCode.C))
            objectPooler.DisableAllPools();
        else if (Input.GetKeyDown(KeyCode.A))
            objectPooler.DisableAllInPoolAt(0);
    }

    void Spawn(int index)
    {
        var g = objectPooler.DrawFromPools(index);
        g.transform.position = Vector3.zero;
        g.transform.rotation = Quaternion.identity;
        g.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}