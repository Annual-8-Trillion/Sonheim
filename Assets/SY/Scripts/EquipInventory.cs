using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipInventory : MonoBehaviour
{
    public static EquipInventory Instance;
    Inventory inventory;

    private void Awake()
    {
        Instance = this;
        inventory = GetComponent<Inventory>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
