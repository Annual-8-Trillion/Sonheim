using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    private TileMapGenerator _tileMapGenerator;
    
    public GameObject[] _objectPrefabs;
    [SerializeField] Transform _objectParents;
    [SerializeField, Range(0, 100)] private int _density; 

    public GameObject[,] MapObjects;
    private void Awake()
    {
        _tileMapGenerator = GetComponent<TileMapGenerator>();
    }

    private void Start()
    {
        SpawnInitialObjects();
        SpawnMonsterSpawners();
    }

    void SpawnInitialObjects()
    {
        Vector2Int size = _tileMapGenerator.size;
        int[,] mapBiomes = _tileMapGenerator.MapBiomes;
        int[,] mapHeights = _tileMapGenerator.MapHeights;
        
        MapObjects = new GameObject[size.x, size.y];
        int[,] objectStatus = new int[size.x, size.y];
        
        System.Random psuedoRandom = new System.Random(_tileMapGenerator.seed.GetHashCode());
        
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                if (mapHeights[x, y] == 0)
                    objectStatus[x, y] = psuedoRandom.Next(0, 100) > _density ? 0 : 1;
        
        
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                if (objectStatus[x, y] == 1)
                {
                    SpawnObject(_objectPrefabs[mapBiomes[x, y]], x, y);
                }
                else MapObjects[x, y] = null;
        
    }

    void SpawnMonsterSpawners()
    {
        Vector2Int size = _tileMapGenerator.size;
        System.Random psuedoRandom = new System.Random(_tileMapGenerator.seed.GetHashCode());
        
        int gridX, gridY;
        do
        {
            gridX = psuedoRandom.Next(1, size.x - 1);
            gridY = psuedoRandom.Next(1, size.y - 1);
        } while (!GetAvailable(gridX, gridY, 0));
        
        BindToCell(GameObject.Find("SkeletonSpawner").transform, gridX, gridY);
        
        do
        {
            gridX = psuedoRandom.Next(1, size.x - 1);
            gridY = psuedoRandom.Next(1, size.y - 1);
        } while (!GetAvailable(gridX, gridY, 1));
        
        BindToCell(GameObject.Find("CactusSpawner").transform, gridX, gridY);
        
        do
        {
            gridX = psuedoRandom.Next(1, size.x - 1);
            gridY = psuedoRandom.Next(1, size.y - 1);
        } while (!GetAvailable(gridX, gridY, 1));
        
        BindToCell(GameObject.Find("SpiderSpawner").transform, gridX, gridY);
    }

    public void SpawnObject(GameObject prefab, int gridX, int gridY)
    {
        Grid grid = _tileMapGenerator.MapGrid;
        
        Vector3 pos = grid.CellToWorld(new Vector3Int(gridX, gridY, 0));
        GameObject generated = Instantiate(prefab, pos, Quaternion.identity, _objectParents);
        MapObjects[gridX, gridY] = generated;
    }

    public void BindToCell(Transform target, int gridX, int gridY)
    {
        target.position = _tileMapGenerator.MapGrid.CellToWorld(new Vector3Int(gridX, gridY, 0));
        MapObjects[gridX, gridY] = target.gameObject;
        target.parent = _objectParents;
    }

    public Grid GetCurrentGrid()
    {
        return _tileMapGenerator.MapGrid;
    }

    public int GetCurrentBiome(int gridX, int gridY)
    {
        return _tileMapGenerator.MapBiomes[gridX, gridY];
    }

    public bool GetAvailable(int gridX, int gridY)
    {
        if (_tileMapGenerator.MapHeights[gridX, gridY] != 0) return false;
        if (MapObjects[gridX, gridY] != null) return false;
        return true;
    }

    public bool GetAvailable(int gridX, int gridY, int biome)
    {
        return GetAvailable(gridX, gridY) && _tileMapGenerator.MapBiomes[gridX, gridY] == biome;
    }
}