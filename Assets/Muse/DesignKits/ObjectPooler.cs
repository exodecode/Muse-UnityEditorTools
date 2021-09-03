using UnityEngine;

namespace Muse
{
    public class ObjectPooler : MonoBehaviour
    {
        public GameObject[] prefabsToPool;
        [Range(1, 128)]
        public int poolSize = 1;

        GameObject[][] pools;
        int[] activeIndexes;
        int length;

        public GameObject DrawFromPool(int index)
        {
            // Debug.Log("Index: " + index);
            return DrawFromPool(pools[index], ref activeIndexes[index]);
        }

        public void CleanupPool(int index)
        {
            for (int j = 0; j < pools[index].Length; j++)
                pools[index][j].SetActive(false);
        }

        public void CleanupAllPools()
        {
            for (int i = 0; i < pools.Length; i++)
                for (int j = 0; j < pools[i].Length; j++)
                    pools[i][j].SetActive(false);
        }

        void Awake()
        {
            length = prefabsToPool.Length;
            pools = new GameObject[length][];
            activeIndexes = new int[length];

            for (int i = 0; i < length; i++)
                pools[i] = PoolPrefab(prefabsToPool[i], poolSize, transform);
        }

        static GameObject[] PoolPrefab(GameObject prefabToPool, int poolSize, Transform parent)
        {
            var pool = new GameObject[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                var go = Instantiate(prefabToPool);
                go.name = go.name + "_" + (i < 10 ? ("0" + i.ToString()) : i.ToString());
                go.SetActive(false);
                go.transform.SetParent(parent);
                pool[i] = go;
            }
            return pool;
        }

        static GameObject DrawFromPool(GameObject[] pool, ref int activeIndex)
        {
            if (activeIndex >= pool.Length)
                activeIndex = 0;

            var g = pool[activeIndex++];
            g.SetActive(true);

            return g;
        }
    }
}