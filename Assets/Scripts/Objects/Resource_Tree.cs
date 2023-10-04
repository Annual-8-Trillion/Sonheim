using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Resource_Tree : MonoBehaviour
{
    public ItemData dropItem;
    [SerializeField] private int dropNumber;
    
    private int _destroyCount;
    private MeshCollider _collider;

    [HideInInspector]
    public int DestroyCount
    {
        get { return _destroyCount; }
        
        set
        {
            _destroyCount = value;
            if (_destroyCount <= 0)
            {
                Vector3 dropPosition = gameObject.transform.position;
                dropPosition.y += 2;
                for (int i = 0; i < dropNumber; i++)
                    ItemManager._instance.DropNewItem(dropPosition, dropItem);
                Destroy(gameObject);
            }
        }
    }

    private void Awake()
    {
        _destroyCount = 10;
        _collider = GetComponent<MeshCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon")
        {
            Debug.Log("tree");
            Weapon weapon = other.GetComponent<Weapon>();
            if (weapon.type == Weapon.Type.Axe)
                DestroyCount -= 2;
            else
                DestroyCount -= 1;
        }
    }
}
