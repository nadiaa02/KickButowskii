
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BackgroundGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RawImage backgroundImage;

    [Header("Texture Settings")]
    [SerializeField] private int textureWidth = 5760;
    [SerializeField] private int textureHeight = 1920;

    [Header("Time of Day")]
    [SerializeField] private bool isNightTime = true;

    [Header("Sky - Night")]
    [SerializeField] private Gradient skyGradientNight;
    [SerializeField] private float skyNoiseScale = 0.5f;
    [SerializeField] private float skyNoiseStrength = 0.3f;

    [Header("Sky - Day")]
    [SerializeField] private Gradient skyGradientDay;

    [Header("Buildings - Night")]
    [SerializeField] private int minBuildingWidth = 80;
    [SerializeField] private int maxBuildingWidth = 170;
    [SerializeField] private float minBuildingHeightPercent = 0.1f;
    [SerializeField] private float maxBuildingHeightPercent = 0.9f;
    [SerializeField] private Color buildingFillColorNight = new Color(0f, 0f, 0f);
    [SerializeField] private Color buildingOutlineColorNight = new Color(0.01568628f, 0.01568628f, 0.01568628f);
    [SerializeField] private int buildingOutlineWidth = 2;

    [Header("Buildings - Day")]
    [SerializeField] private Color buildingFillColorDay = new Color(0.3f, 0.35f, 0.4f);
    [SerializeField] private Color buildingOutlineColorDay = new Color(0.5f, 0.55f, 0.6f);

    [Header("Windows - Night")]
    [SerializeField] private bool drawWindows = true;
    [SerializeField] private int windowWidth = 8;
    [SerializeField] private int windowHeight = 12;
    [SerializeField] private int windowSpacingX = 20;
    [SerializeField] private int windowSpacingY = 30;
    [SerializeField] private int windowMarginTop = 50;
    [SerializeField] private int windowMarginBottom = 50;
    [SerializeField] private int windowMarginLeft = 35;
    [SerializeField] private int windowMarginRight = 35;
    [SerializeField] private Color windowOnColorNight = new Color(1f, 0.95f, 0.6f);
    [SerializeField] private float windowLightChanceNight = 0.4f;

    [Header("Windows - Day")]
    [SerializeField] private Color windowOnColorDay = new Color(0.7003382f, 0.7447312f, 0.8113208f);
    [SerializeField] private float windowLightChanceDay = 1f;

    private Texture2D backgroundTexture;

    void Start()
    {
        GenerateBackground();
    }

    public void GenerateBackground()
    {
        backgroundTexture = new Texture2D(textureWidth, textureHeight);

        DrawSky();
        List<Rect> buildings = GenerateBuildings();
        DrawBuildings(buildings);

        if (drawWindows)
            DrawWindows(buildings);

        backgroundTexture.Apply();
        backgroundImage.texture = backgroundTexture;
    }

    void DrawSky()
    {
        Gradient currentGradient = isNightTime ? skyGradientNight : skyGradientDay;

        for (int x = 0; x < textureWidth; x++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                float noise = Mathf.PerlinNoise(x * skyNoiseScale, y * skyNoiseScale);
                float gradientPos = (float)y / textureHeight;
                Color skyColor = currentGradient.Evaluate(gradientPos + (noise - 0.5f) * skyNoiseStrength);
                backgroundTexture.SetPixel(x, y, skyColor);
            }
        }
    }

    List<Rect> GenerateBuildings()
    {
        List<Rect> buildings = new List<Rect>();
        int currentX = 0;

        while (currentX < textureWidth)
        {
            int width = Random.Range(minBuildingWidth, maxBuildingWidth);

            float perlinValue = Mathf.PerlinNoise(currentX * 0.005f, 100f);
            float randomValue = Random.value;

            float heightPercent = Mathf.Lerp(minBuildingHeightPercent, maxBuildingHeightPercent,
                                             perlinValue * 0.5f + randomValue * 0.5f);
            int height = (int)(textureHeight * heightPercent);

            buildings.Add(new Rect(currentX, 0, width, height));
            currentX += width;
        }

        return buildings;
    }

    void DrawBuildings(List<Rect> buildings)
    {
        Color fillColor = isNightTime ? buildingFillColorNight : buildingFillColorDay;
        Color outlineColor = isNightTime ? buildingOutlineColorNight : buildingOutlineColorDay;

        foreach (Rect building in buildings)
        {
            
            for (int x = (int)building.x; x < building.x + building.width && x < textureWidth; x++)
            {
                for (int y = 0; y < building.height && y < textureHeight; y++)
                {
                    backgroundTexture.SetPixel(x, y, fillColor);
                }
            }

            
            for (int y = 0; y < building.height && y < textureHeight; y++)
            {
                for (int w = 0; w < buildingOutlineWidth; w++)
                {
                    int x = (int)building.x + w;
                    if (x < textureWidth)
                        backgroundTexture.SetPixel(x, y, outlineColor);
                }
            }

            
            for (int y = 0; y < building.height && y < textureHeight; y++)
            {
                for (int w = 0; w < buildingOutlineWidth; w++)
                {
                    int x = (int)(building.x + building.width) - w - 1;
                    if (x >= 0 && x < textureWidth)
                        backgroundTexture.SetPixel(x, y, outlineColor);
                }
            }

            
            for (int x = (int)building.x; x < building.x + building.width && x < textureWidth; x++)
            {
                for (int w = 0; w < buildingOutlineWidth; w++)
                {
                    int y = (int)building.height - w - 1;
                    if (y >= 0 && y < textureHeight)
                        backgroundTexture.SetPixel(x, y, outlineColor);
                }
            }
        }
    }

    void DrawWindows(List<Rect> buildings)
    {
        Color windowColor = isNightTime ? windowOnColorNight : windowOnColorDay;
        float lightChance = isNightTime ? windowLightChanceNight : windowLightChanceDay;

        foreach (Rect building in buildings)
        {
            int startX = (int)building.x + windowMarginLeft;
            int endX = (int)(building.x + building.width) - windowMarginRight;
            int startY = windowMarginBottom;
            int endY = (int)building.height - windowMarginTop;

            if (endX - startX < windowWidth || endY - startY < windowHeight)
                continue;

            for (int wx = startX; wx + windowWidth < endX; wx += windowSpacingX)
            {
                for (int wy = startY; wy + windowHeight < endY; wy += windowSpacingY)
                {
                    if (Random.value > lightChance)
                        continue;

                    for (int x = 0; x < windowWidth; x++)
                    {
                        for (int y = 0; y < windowHeight; y++)
                        {
                            int pixelX = wx + x;
                            int pixelY = wy + y;

                            if (pixelX < textureWidth && pixelY < textureHeight)
                            {
                                backgroundTexture.SetPixel(pixelX, pixelY, windowColor);
                            }
                        }
                    }
                }
            }
        }
    }
}
