using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MappablesData", menuName = "Mappable Data")]
public class PixelMapSettings : ScriptableObject
{
    public PixelMappedPrefab[] prefabsToMap;

    public PixelMappedPrefab GetMappableWithPrefab(GameObject prefab) =>
        prefabsToMap.Where(m => m.prefab == prefab).First();

    void OnValidate()
    {
        for (int i = 0; i < prefabsToMap.Length; i++)
        {
            var m = prefabsToMap[i];

            if (m.prefab != null)
                m.name = m.prefab.name;
            else
                m.name = "NULL";
        }
    }

    [System.Serializable]
    public class PixelMappedPrefab
    {
        [HideInInspector] public string name;
        public GameObject prefab;
        public Color color;
    }
}