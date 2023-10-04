using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPlayerTest : MonoBehaviour
{
    [SerializeField] ItemData[] items;

    public void OnClickAddItemsButton()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].Type == ItemType.Resource)
            {
                Inventory.instance.AddItem(items[i]);
            }
        }
        CraftPanelUI.instance.UpdateResources();
        CraftPanelUI.instance.ResetCurRecipes();
    }
    public void OnClickAddRandomItemButton()
    {
        int index = Random.Range(0, items.Length);
        Inventory.instance.AddItem(items[index]);
        CraftPanelUI.instance.UpdateResources();
        CraftPanelUI.instance.ResetCurRecipes();
    }
}
