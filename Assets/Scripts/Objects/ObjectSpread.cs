using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectSpread : MonoBehaviour
{
    private ObjectSpawner _objectSpawner;
    
    [SerializeField, Tooltip("Wait time for respawn (in seconds)")]
    private int spreadTime; // in seconds

    private void Awake()
    {
        Transform gridObject = transform.parent.parent;
        _objectSpawner = gridObject.GetComponentInChildren<ObjectSpawner>();
    }

    private void Start()
    {
        StartCoroutine(WaitForSpread());
    }

    IEnumerator WaitForSpread()
    {
        WaitForSeconds wait = new WaitForSeconds(spreadTime);
        while (true)
        {
            yield return wait;
            
            Spread();
        }
    }

    private void Spread()
    {
        Grid currentGrid = _objectSpawner.GetCurrentGrid();
        Vector3Int currentCell = currentGrid.WorldToCell(transform.position);
        int currentBiome = _objectSpawner.GetCurrentBiome(currentCell.x, currentCell.y);
        
        System.Random psuedoRandom = new System.Random();
        
        for (int neighbourX = currentCell.x - 1; neighbourX <= currentCell.x + 1; neighbourX++)
            for (int neighbourY = currentCell.y - 1; neighbourY <= currentCell.y + 1; neighbourY++)
            {
                if (psuedoRandom.Next(0, 100) < 80) continue;
                if (neighbourX >= 0 && neighbourX < _objectSpawner.MapObjects.GetLength(0) && neighbourY >= 0 && neighbourY < _objectSpawner.MapObjects.GetLength(1))
                    if (neighbourX != currentCell.x || neighbourY != currentCell.y)
                        if (_objectSpawner.GetAvailable(neighbourX, neighbourY, currentBiome))
                        {
                            _objectSpawner.SpawnObject(_objectSpawner._objectPrefabs[currentBiome], neighbourX, neighbourY);
                            return;
                        }
            }
                
    }
}
