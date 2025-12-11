using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Grid / Level size")]
    [SerializeField] private int levelWidth = 120;   
    [SerializeField] private int minHeight = 0;      
    [SerializeField] private int maxHeight = 4;      
    [SerializeField] private float tileSize = 1f;

    [Header("Jump constraints")]
    [SerializeField] private int maxStepUp = 2;      
    [SerializeField] private int maxGap = 3;         

    [Header("Prefabs")]
    [SerializeField] private GameObject roofTilePrefab;
    [SerializeField] private GameObject blockObstaclePrefab;
    [SerializeField] private GameObject anthena1Prefab;
    [SerializeField] private GameObject anthena2Prefab;

    [Header("Obstacles placement")]
    [SerializeField] private int segmentSize = 10;   
    [SerializeField] private float obstacleChancePerSegment = 0.7f;
    [SerializeField] private float blockVsAntennaChance = 0.5f; 

    private int[] heights;

    private const int GAP = int.MinValue;

    private void Start()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        heights = new int[levelWidth];

        GenerateHeightsDrunkard();
        BuildRoofs();
        PlaceObstaclesBySegments();
    }

    
    private void GenerateHeightsDrunkard()
    {
        int currentHeight = (minHeight + maxHeight) / 2;

        
        for (int x = 0; x < levelWidth; x++)
        {
            int step = Random.Range(-1, 2); 

           
            if (Random.value < 0.5f)
                step = 0;

            int newHeight = currentHeight + step;
            newHeight = Mathf.Clamp(newHeight, minHeight, maxHeight);

            
            if (x > 0 && newHeight - heights[x - 1] > maxStepUp)
            {
                newHeight = heights[x - 1] + maxStepUp;
            }

            heights[x] = newHeight;
            currentHeight = newHeight;
        }

        
        for (int x = 5; x < levelWidth - 5;) 
        {
            if (Random.value < 0.15f) 
            {
                int gapSize = Random.Range(1, maxGap + 1);
                for (int i = 0; i < gapSize && x + i < levelWidth; i++)
                {
                    heights[x + i] = GAP;
                }
                x += gapSize;
            }
            else
            {
                x++;
            }
        }
    }

    
    private void BuildRoofs()
    {
        for (int x = 0; x < levelWidth; x++)
        {
            if (heights[x] == GAP) continue;

            int h = heights[x];

            Vector3 worldPos = transform.position +
                               new Vector3(x * tileSize, h * tileSize, 0f);

            Instantiate(roofTilePrefab, worldPos, Quaternion.identity, transform);
        }
    }

    
    private void PlaceObstaclesBySegments()
    {
        for (int startX = 0; startX < levelWidth; startX += segmentSize)
        {
            int endX = Mathf.Min(startX + segmentSize, levelWidth);

            if (Random.value > obstacleChancePerSegment)
                continue;

            List<int> candidateXs = new List<int>();
            for (int x = startX; x < endX; x++)
            {
                if (x < 0 || x >= levelWidth) continue;
                if (heights[x] == GAP) continue;

                int h = heights[x];

                int leftH = (x > 0) ? heights[x - 1] : h;
                int rightH = (x < levelWidth - 1) ? heights[x + 1] : h;

                if (leftH <= h && rightH <= h)
                    candidateXs.Add(x);
            }

            if (candidateXs.Count == 0)
                continue;

            int chosenX = candidateXs[Random.Range(0, candidateXs.Count)];
            int heightAtX = heights[chosenX];

            Vector3 roofPos = transform.position +
                              new Vector3(chosenX * tileSize, heightAtX * tileSize, 0f);

            Collider2D roofCol = Physics2D.OverlapPoint(roofPos);
            float topY = roofPos.y;
            if (roofCol != null)
                topY = roofCol.bounds.max.y;          

            Vector3 basePos = new Vector3(roofPos.x, topY + 0.05f, 0f);

            if (Random.value < blockVsAntennaChance)
            {
                Instantiate(blockObstaclePrefab, basePos, Quaternion.identity, transform);
            }
            else
            {
                GameObject antennaPrefab =
                    (Random.value < 0.5f) ? anthena1Prefab : anthena2Prefab;

             
                Vector3 antennaPos = basePos + Vector3.up * 0.9f;
                Instantiate(antennaPrefab, antennaPos, Quaternion.identity, transform);
            }
        }
    }



}
