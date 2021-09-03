using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Muse
{
    public class GetSurroundingElementsTester : MonoBehaviour
    {
        public int length;
        public int width;

        public int index;

        GameObject[] cubes;

        void Start()
        {
            cubes = Enumerable.Range(0, length).Select(i =>
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                var collider = cube.GetComponent<Collider>();
                if (collider != null)
                    Destroy(collider);

                cube.name = i.ToString();
                return cube;
            })
            .ToArray();

            SetPositions();
        }

        void SetPositions()
        {
            for (int i = 0; i < length; i++)
            {
                var t = cubes[i].transform;
                var x = ((i / width) + i % width) - (i / width);
                var y = (i / width);
                var pos = new Vector3(x, y, 0);

                t.position = pos;
            }
        }

        void SetColors()
        {
            var baseMat = new Material(Shader.Find("Standard"));
            baseMat.SetColor("_Color", Color.black);

            var highlightedMat = new Material(Shader.Find("Standard"));
            highlightedMat.SetColor("_Color", Color.green);

            for (int i = 0; i < length; i++)
                cubes[i].GetComponent<Renderer>().material = baseMat;

            var surrounding = cubes.GetSurroundingElements(index, width);

            for (int i = 0; i < surrounding.Length; i++)
                surrounding[i].GetComponent<Renderer>().material = highlightedMat;
        }

        float t;

        private void Update()
        {
            t += Time.deltaTime;

            // if (Input.GetKeyDown(KeyCode.Space))
            if (t > 0.3f)
            {
                // SetPositions();
                SetColors();

                index++;

                if (index > length - 1)
                    index = 0;

                t = 0;
            }
        }
    }
}