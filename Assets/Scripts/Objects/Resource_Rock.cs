using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource_Rock : MonoBehaviour
{
    public ItemData dropItem;
    
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
                for (int i = 0; i < 3; i++)
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
            Debug.Log("ROCK");
            Weapon weapon = other.GetComponent<Weapon>();
            if (weapon.type == Weapon.Type.Pickaxe)
                DestroyCount -= 2;
            else
                DestroyCount -= 1;
        }
    }
}
