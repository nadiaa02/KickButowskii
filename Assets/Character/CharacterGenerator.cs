using UnityEngine;

public class CharacterGenerator : MonoBehaviour
{
    public GameObject pixelTilePrefab;
    public int width = 12;
    public int height = 16;
    public float tileSize = 0.1f; 

    private CharacterSpec currentSpec;
    
    int headWidth = 8;
    int headHeight = 7;
    int headStartX;
    int headStartY;
    int headEndY;

    int bodyStartY;
    int bodyEndY;
    int waistY;
    int legsBottomY;


    
    int leftHandCenterX;
    int rightHandCenterX;
    int handsY;


    void InitLayout()
    {
        headStartX = (width - headWidth) / 2;
        headStartY = 14;
        headEndY = headStartY + headHeight - 1;

        int neckBottomY = headStartY - 2;  

        bodyStartY = neckBottomY - 2;      
        bodyEndY = bodyStartY + 4;

        waistY = bodyStartY - 1;
    }






    void Start()
    {
        GenerateNewCharacter();
    }

    public void GenerateNewCharacter()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        InitLayout();

        currentSpec = GenerateSpec();
        DrawCharacter(currentSpec);

    }

    CharacterSpec GenerateSpec()
    {
        var spec = new CharacterSpec();

        spec.skinPalette = (SkinPalette)Random.Range(0, 3);
        switch (spec.skinPalette)
        {
            case SkinPalette.Palette1:
                spec.skinColor = new Color(1f, 0.87f, 0.77f); 
                break;
            case SkinPalette.Palette2:
                spec.skinColor = new Color(0.87f, 0.70f, 0.52f);
                break;
            case SkinPalette.Palette3:
                spec.skinColor = new Color(0.60f, 0.45f, 0.30f);
                break;
        }

        
        spec.bodyType = (BodyType)Random.Range(0, 3);

        float r1 = Random.value;
        if (r1 < 0.6f) spec.hairType = HairType.Short;
        else if (r1 < 0.85f) spec.hairType = HairType.Long;
        else spec.hairType = HairType.Fringe;



        int hairColorIndex = Random.Range(0, 4); 
        switch (hairColorIndex)
        {
            case 0: spec.hairColor = new Color(0.8f, 0.4f, 0.1f); break; 
            case 1: spec.hairColor = new Color(0.95f, 0.9f, 0.5f); break; 
            case 2: spec.hairColor = new Color(0.4f, 0.25f, 0.1f); break; 
            case 3: spec.hairColor = new Color(0.1f, 0.1f, 0.1f); break; 
        }

        spec.upperClothesType = (UpperClothesType)Random.Range(0, 3);

        spec.lowerClothesType = (LowerClothesType)Random.Range(0, 3);

        spec.eyeColor = (EyeColor)Random.Range(0, 3);

        int secIndex = Random.Range(0, 3);
        switch (secIndex)
        {
            case 0: spec.clothesSecondaryColor = new Color(0.65f, 0.75f, 0.85f); break; 
            case 1: spec.clothesSecondaryColor = new Color(0.70f, 0.80f, 0.70f); break; 
            case 2: spec.clothesSecondaryColor = new Color(0.75f, 0.70f, 0.60f); break; 
        }

        float r = Random.Range(0.4f, 0.8f);
        float g = Random.Range(0.4f, 0.8f);
        float b = Random.Range(0.4f, 0.8f);
        float gray = (r + g + b) / 3f;
        spec.clothesMainColor = Color.Lerp(new Color(gray, gray, gray), new Color(r, g, b), 0.5f);

        return spec;
    }
    void DrawCharacter(CharacterSpec spec)
    {
        Color?[,] grid = new Color?[width, height];

        DrawHeadAndNeck(grid, spec);
        DrawHair(grid, spec);
        DrawEyes(grid, spec);
        DrawUpperClothes(grid, spec);
        DrawArmShadows(grid, spec);   

        DrawLowerClothes(grid, spec);
        DrawCenterLowerShadow(grid, spec);
        DrawHands(grid, spec);
        DrawSkate(grid);

        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y].HasValue)
                {
                    CreateTile(x, y, grid[x, y].Value);
                }
            }
        }
    }

    void CreateTile(int x, int y, Color color)
    {
        var tile = Instantiate(pixelTilePrefab, transform);

        int mirroredX = width - 1 - x;

        tile.transform.localPosition = new Vector3(
            (mirroredX - width / 2f) * tileSize,
            y * tileSize,
            0f);

        var sr = tile.GetComponent<SpriteRenderer>();
        sr.color = color;
    }


    void DrawHeadAndNeck(Color?[,] grid, CharacterSpec spec)
    {
        for (int x = headStartX; x < headStartX + headWidth; x++)
        {
            for (int y = headStartY; y <= headEndY; y++)
            {
                bool topCorner = (y == headEndY &&
                                 (x == headStartX || x == headStartX + headWidth - 1));
                if (!topCorner && InBounds(x, y))
                    grid[x, y] = spec.skinColor;
            }
        }

        int neckWidth = 2;
        int neckCenterX = width / 2;
        int neckFirstY = headStartY - 1; 

        for (int i = 0; i < 2; i++)
        {
            int ny = neckFirstY - i;
            for (int x = neckCenterX - neckWidth / 2; x <= neckCenterX + neckWidth / 2; x++)
            {
                if (InBounds(x, ny))
                    grid[x, ny] = spec.skinColor;
            }
        }
    }




    void DrawHair(Color?[,] grid, CharacterSpec spec)
    {
        int topExtra = 2; 

        for (int x = headStartX; x < headStartX + headWidth; x++)
        {
            for (int y = headEndY - 1; y <= headEndY + topExtra; y++)
            {
                if (!InBounds(x, y)) continue;

                bool draw = false;

                switch (spec.hairType)
                {
                    case HairType.Short:
                        if (y == headEndY || y == headEndY + 1)
                            draw = true;
                        break;

                    case HairType.Long:
                        if (y >= headEndY && y <= headEndY + topExtra)
                        {
                            draw = true; 
                        }
                        else if ((x == headStartX || x == headStartX + headWidth - 1) &&
                                 y >= headEndY - 2 && y <= headEndY)
                        {
                            draw = true; 
                        }
                        break;

                    case HairType.Fringe:
                        if (y == headEndY - 1 &&
                            x > headStartX + 1 && x < headStartX + headWidth - 2)
                            draw = true;
                        break;
                }

                if (draw)
                    grid[x, y] = spec.hairColor;
            }
        }
    }




    void DrawEyes(Color?[,] grid, CharacterSpec spec)
    {
        int eyeY = headStartY + headHeight / 2 + 1;


        int leftEyeX = headStartX + 2;
        int rightEyeX = headStartX + headWidth - 3;

        Color eyeColor = spec.eyeColor switch
        {
            EyeColor.Green => new Color(0.3f, 0.8f, 0.3f),
            EyeColor.Blue => new Color(0.3f, 0.5f, 0.9f),
            _ => new Color(0.4f, 0.3f, 0.2f)
        };

        if (InBounds(leftEyeX, eyeY)) grid[leftEyeX, eyeY] = eyeColor;
        if (InBounds(rightEyeX, eyeY)) grid[rightEyeX, eyeY] = eyeColor;

        
        int mouthY = eyeY - 2;
        int mouthCenterX = width / 2 - 1;
        Color mouthColor = new Color(0.45f, 0.35f, 0.3f);

        for (int x = mouthCenterX; x <= mouthCenterX + 1; x++)
        {
            if (InBounds(x, mouthY))
                grid[x, mouthY] = mouthColor;
        }

    }





    void DrawUpperClothes(Color?[,] grid, CharacterSpec spec)
    {
        int baseWidth = spec.bodyType switch
        {
            BodyType.Slim => 6,
            BodyType.Normal => 8,
            BodyType.Big => 10,
            _ => 8
        };

        int bodyWidth = Mathf.Max(baseWidth, headWidth);

        int bodyStartX = headStartX + (headWidth - bodyWidth) / 2;

        for (int x = bodyStartX; x < bodyStartX + bodyWidth; x++)
        {
            for (int y = bodyStartY; y <= bodyEndY; y++)
            {
                if (InBounds(x, y))
                    grid[x, y] = spec.clothesMainColor;
            }
        }

        leftHandCenterX = bodyStartX;
        rightHandCenterX = bodyStartX + bodyWidth - 1;
    }








    void DrawLowerClothes(Color?[,] grid, CharacterSpec spec)
    {
        int centerX = width / 2;
        int topY = waistY;
        int bottomY = waistY - 4;
        if (bottomY < 0) bottomY = 0;

        int halfWidth = spec.bodyType switch
        {
            BodyType.Slim => 2,
            BodyType.Normal => 3,
            BodyType.Big => 4,
            _ => 3
        };

        for (int x = centerX - halfWidth; x <= centerX + halfWidth; x++)
        {
            for (int y = bottomY; y <= topY; y++)
            {
                if (!InBounds(x, y)) continue;

                bool draw = true;

                switch (spec.lowerClothesType)
                {
                    case LowerClothesType.Pants:
                        draw = true;
                        break;
                    case LowerClothesType.Shorts:
                        if (y < topY - 2) draw = false;
                        break;
                    case LowerClothesType.Skirt:
                        if (y < topY - 1)
                        {
                            int extra = topY - y;
                            if (Mathf.Abs(x - centerX) > halfWidth + extra)
                                draw = false;
                        }
                        break;
                }

                if (draw)
                    grid[x, y] = spec.clothesSecondaryColor;
            }
        }

        legsBottomY = -1;
        for (int y = bottomY; y <= topY; y++)
        {
            if (grid[centerX, y].HasValue &&
                grid[centerX, y].Value == spec.clothesSecondaryColor)
            {
                legsBottomY = y;
                break;              
            }
        }

        if (legsBottomY == -1)
            legsBottomY = bottomY;
    }




    void DrawHands(Color?[,] grid, CharacterSpec spec)
    {
        Color handColor = spec.skinColor;

        int handHeight = 1;
        int yStart = bodyStartY - handHeight; 

        for (int x = leftHandCenterX; x <= leftHandCenterX; x++)
        {
            for (int y = yStart; y < yStart + handHeight; y++)
            {
                if (InBounds(x, y))
                    grid[x, y] = handColor;
            }
        }

        for (int x = rightHandCenterX; x <= rightHandCenterX; x++)
        {
            for (int y = yStart; y < yStart + handHeight; y++)
            {
                if (InBounds(x, y))
                    grid[x, y] = handColor;
            }
        }
    }
    void DrawArmShadows(Color?[,] grid, CharacterSpec spec)
    {
        Color baseColor = spec.clothesMainColor;
        float shadowFactor = 0.75f; 
        Color shadowColor = new Color(
            baseColor.r * shadowFactor,
            baseColor.g * shadowFactor,
            baseColor.b * shadowFactor
        );

        int leftX = leftHandCenterX + 1;
        int rightX = rightHandCenterX - 1;

        int startY = bodyStartY;     
        int endY = bodyEndY - 1;      

        for (int y = startY; y <= endY; y++)
        {
            if (InBounds(leftX, y))
                grid[leftX, y] = shadowColor;

            if (InBounds(rightX, y))
                grid[rightX, y] = shadowColor;
        }
    }
    void DrawCenterLowerShadow(Color?[,] grid, CharacterSpec spec)
    {
        Color baseColor = spec.clothesSecondaryColor;
        float shadowFactor = 0.75f;
        Color shadowColor = new Color(
            baseColor.r * shadowFactor,
            baseColor.g * shadowFactor,
            baseColor.b * shadowFactor
        );

        int centerX = width / 2;

        if (legsBottomY < 0) return; 

        int startY = legsBottomY;   
        int endY = waistY - 1;    

        for (int y = startY; y <= endY; y++)
        {
            if (InBounds(centerX, y))
                grid[centerX, y] = shadowColor;
        }
    }
    void DrawSkate(Color?[,] grid)
    {
        if (legsBottomY < 0) return;

        Color deckColor = new Color(0.25f, 0.25f, 0.25f);
        Color legColor = new Color(0.6f, 0.0f, 0.2f);

        int centerX = width / 2;

        int deckY = legsBottomY - 1;

        int deckHalfWidth = 4;   

        for (int x = centerX - deckHalfWidth; x <= centerX + deckHalfWidth; x++)
        {
            if (InBounds(x, deckY))
                grid[x, deckY] = deckColor;
        }

        int legHeight = 2;       
        int legOffset = 2;       

        int leftLegX = centerX - legOffset;
        int rightLegX = centerX + legOffset;

        for (int y = deckY - 1; y >= deckY - legHeight; y--)
        {
            if (InBounds(leftLegX, y))
                grid[leftLegX, y] = legColor;
            if (InBounds(rightLegX, y))
                grid[rightLegX, y] = legColor;
        }
    }









    bool InBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

}