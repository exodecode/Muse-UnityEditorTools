// using Muse;
using UnityEngine;

namespace Muse
{
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
                objectPooler.CleanupAllPools();
            else if (Input.GetKeyDown(KeyCode.A))
                objectPooler.CleanupPool(0);
        }

        void Spawn(int index)
        {
            var g = objectPooler.DrawFromPool(index);
            g.transform.position = Vector3.zero;
            g.transform.rotation = Quaternion.identity;
            g.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}