using System;
using UnityEngine;

public class PlayerObjectInstaller : MonoBehaviour
{
    [SerializeField] private ObjectSpawner objectSpawner;
    
    private Grid currentGrid;
    private PlayerController controller;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    private void Start()
    {
        currentGrid = objectSpawner.GetCurrentGrid();
    }

    public bool InstallObject(GameObject prefab)
    {
        Vector3Int installedCell = currentGrid.WorldToCell(transform.position + (controller.direction * currentGrid.cellSize.x));
        
        if (objectSpawner.GetAvailable(installedCell.x, installedCell.y) == false) return false;
        objectSpawner.SpawnObject(prefab, installedCell.x, installedCell.y);
        
        return true;
    }
}