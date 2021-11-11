using UnityEngine;

[CreateAssetMenu(fileName = "MappablesData", menuName = "Mappable Data")]
public class MappablesData : ScriptableObject
{
    public Mappable[] mappables;

    void OnValidate()
    {
        for (int i = 0; i < mappables.Length; i++)
        {
            var m = mappables[i];

            if (m.prefab != null)
                m.name = m.prefab.name;
            else
                m.name = "NULL";
        }
    }

    [System.Serializable]
    public class Mappable
    {
        [HideInInspector] public string name;
        public GameObject prefab;
        public Color color;
    }
}