using System;
using UnityEngine;
using UnityEngine.Serialization;

public class TileMapGenerator : MonoBehaviour
{
    public Grid MapGrid;
    
    [SerializeField] GameObject[] tilePrefabs;
    [SerializeField] Transform _tileParents;
    
    public string seed;
    public bool useRandomSeed;
    
    public Vector2Int size;
    [SerializeField, Range(0, 100)]
    private int normalBlockPercent;
    [SerializeField, Range(0, 100)]
    private int normalBiomePercent;

    [SerializeField] private int smoothingFactor;
    [SerializeField] private float height;
    
    public int[,] MapHeights;
    public int[,] MapBiomes;
    
    void Awake()
    {
        GenerateMap();
        DrawMapTiles();
    }

    private void Start()
    {
        Transform playerTransform = GameManager.Instance.Player.transform;
        playerTransform.position = MapGrid.CellToWorld(new Vector3Int(size.x / 2, size.y / 2)) + Vector3.up * 3f;
    }

    void GenerateMap()
    {
        MapHeights = new int[size.x, size.y];
        MapBiomes = new int[size.x, size.y];
        RandomFillMap();

        for (int i = 0; i < smoothingFactor; i++)
        {
            SmoothMap(MapHeights);
            SmoothMap(MapBiomes);
        }
    }

    void RandomFillMap()
    {
        if (useRandomSeed)
            seed = DateTime.Now.ToBinary().ToString();

        System.Random psuedoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                if (x == 0 || x == size.x - 1 || y == 0 || y == size.y - 1)
                    MapHeights[x, y] = 1;
                else MapHeights[x, y] = psuedoRandom.Next(0, 100) < normalBlockPercent ? 0 : 1;
                MapBiomes[x, y] = psuedoRandom.Next(0, 100) < normalBiomePercent ? 0 : 1;
            }
    }

    void SmoothMap(int[,] map)
    {
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                int neighbourDifferentTiles = GetSurroundingTiles(map, x, y);
                
                if (neighbourDifferentTiles > 4)
                    map[x, y] = 1;
                else if (neighbourDifferentTiles < 4)
                    map[x, y] = 0;
            }
    }

    int GetSurroundingTiles(int[,] map, int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
                if (neighbourX >= 0 && neighbourX < size.x && neighbourY >= 0 && neighbourY < size.y)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                        wallCount += map[neighbourX, neighbourY];
                }
                else wallCount++;

        return wallCount;
    }

    void DrawMapTiles()
    {
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                Vector3 pos = MapGrid.CellToWorld(new Vector3Int(x, y, 0));
                GameObject generated = Instantiate(tilePrefabs[MapBiomes[x, y]], pos ,Quaternion.identity, _tileParents);
                float newHeight = Convert.ToSingle(MapHeights[x, y]) * height;
                generated.GetComponentInChildren<HexRenderer>().height = newHeight;
                generated.GetComponentInChildren<BoxCollider>().size += Vector3.up * newHeight;
                generated.SetActive(true);
            }
                
    }
}



