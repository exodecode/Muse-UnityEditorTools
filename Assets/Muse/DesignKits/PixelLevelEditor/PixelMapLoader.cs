using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PixelMapLoader : MonoBehaviour
{
    public Texture2D map;
    public PixelMapSettings pixelMapSettings;

    List<Color> colors;
    List<GameObject> prefabs;
    int width;
    int height;

    void Awake()
    {
        width = map.width;
        height = map.height;

        colors = pixelMapSettings.prefabsToMap.Select(m => new Color(m.color.r, m.color.g, m.color.b, 1)).ToList();
        prefabs = pixelMapSettings.prefabsToMap.Select(m => m.prefab).ToList();
    }

    void Start()
    {
        LoadMap();
    }

    public void LoadMap()
    {
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                var col = map.GetPixel(w, h);
                var colNA = new Color(col.r, col.g, col.b, 1);

                if (col.a > 0)
                {
                    for (int i = 0; i < colors.Count; i++)
                    {
                        var color = colors[i];
                        // Debug.Log("\n" + "Color: " + ColorLogMsg(color) + "\n" + "ColNA: " + ColorLogMsg(colNA));

                        if (color.Equals(colNA))
                        {
                            var p = prefabs[i];
                            var g = Instantiate(p);
                            g.transform.position = PixelCoordsAndAlphaToWorldPos((w, h, col.a), map.width / 2);
                            g.transform.SetParent(transform);
                        }
                    }
                }
            }
        }
    }

    public string ColorLogMsg(Color col) => $"R: {col.r}, G: {col.g}, B: {col.b}, A: {col.a}";

    public static Vector3 PixelCoordsAndAlphaToWorldPos((int x, int y, float a) pixelCoordsAndAlpha, int mapRadius) =>
         new Vector3(
             pixelCoordsAndAlpha.x - mapRadius,
             HeightFromAlpha(pixelCoordsAndAlpha.a),
             pixelCoordsAndAlpha.y - mapRadius);

    public static float HeightFromAlpha(float a) => (Mathf.Clamp(a, 0, 1) * 100) - 50;
}