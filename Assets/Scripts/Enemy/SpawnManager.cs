using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab1;
    public GameObject enemyPrefab2;
    public GameObject enemyPrefab3;
    public GameObject enemyPrefab4;

    public float spawnInterval = 5f;

    private Transform skeletonSpawner;
    private Transform spiderSpawner;
    private Transform cactusSpawner;
    private Transform chickenSpawner;


    private int enemyCount = 0;

    void Start()
    {
        skeletonSpawner = GameObject.Find("SkeletonSpawner").transform;
        cactusSpawner = GameObject.Find("CactusSpawner").transform;
        spiderSpawner = GameObject.Find("SpiderSpawner").transform;
        chickenSpawner = GameObject.Find("ChickenSpawner").transform;


        StartCoroutine(SpawnNPC());
    }

    IEnumerator SpawnNPC()
    {
        while (enemyCount < 5)
        {
            yield return new WaitForSeconds(spawnInterval);
            
            Vector3 spawnPosition1 = skeletonSpawner.position;
            GameObject npcInstance1 = Instantiate(enemyPrefab1, spawnPosition1 + Vector3.up, Quaternion.identity);

            Vector3 spawnPosition2 = cactusSpawner.position;
            GameObject npcInstance2 = Instantiate(enemyPrefab2, spawnPosition2 + Vector3.up, Quaternion.identity);

            Vector3 spawnPosition3 = spiderSpawner.position;
            GameObject npcInstance3 = Instantiate(enemyPrefab3, spawnPosition3 + Vector3.up, Quaternion.identity);

            Vector3 spawnPosition4 = chickenSpawner.position;
            GameObject npcInstance4 = Instantiate(enemyPrefab4, spawnPosition4 + Vector3.up, Quaternion.identity);


            enemyCount++;
        }
    }
}