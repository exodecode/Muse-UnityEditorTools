using System.Linq;
using UnityEngine;

namespace Muse
{
    public class ObjectPooler : MonoBehaviour
    {
        public GameObject[] prefabsToPool;

        [Range(1, 128)]
        public int poolSize = 1;

        Pool[] pools;
        int[] activeIndexes;
        int length;

        public GameObject DrawFromPools(int prefabIndex)
        {
            return DrawFromPool(pools[prefabIndex], ref activeIndexes[prefabIndex]);
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

            pools = new Pool[length];
            activeIndexes = new int[length];

            for (int i = 0; i < length; i++)
                pools[i] = PoolPrefab(prefabsToPool[i], poolSize, transform);
        }

        static Pool PoolPrefab(GameObject prefabToPool, int poolSize, Transform parent)
        {
            GameObject CreateGO(int index)
            {
                var go = Instantiate(prefabToPool);
                go.name = go.name + "_" + (index < 10 ? ("0" + index.ToString()) : index.ToString());
                go.SetActive(false);
                go.transform.SetParent(parent);
                return go;
            }

            var gameObjects = Enumerable.Range(0, poolSize).Select(i => CreateGO(i)).ToArray();

            return new Pool(gameObjects);
        }

        static GameObject DrawFromPool(Pool pool, ref int activeIndex)
        {
            if (activeIndex >= pool.Length)
                activeIndex = 0;

            var g = pool[activeIndex++];
            g.SetActive(true);

            return g;
        }
    }

    struct Pool
    {
        public readonly GameObject[] gameObjects;
        public readonly int Length;

        public Pool(GameObject[] gameObjects)
        {
            this.gameObjects = gameObjects;
            this.Length = gameObjects.Length;
        }

        public void DisableAll()
        {
            for (int i = 0; i < gameObjects.Length; i++)
                gameObjects[i].SetActive(false);
        }

        public GameObject this[int index] => gameObjects[index];
    }
}