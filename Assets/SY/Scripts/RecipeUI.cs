using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public enum CraftState { Craftable, EnoughInInventory, NotEnough }
public class RecipeUI : MonoBehaviour
{
    public CraftRecipe curRecipe;

    public ItemSlotUI[] requiredItemSlotui;
    public ItemSlotUI resultItemSlotui;

    [Header("Recipe")]
    public Image background;
    public bool isReady;
    public int myindex;
    public Button button;

    public bool isEmpty;
    public CraftState curRecipeState;

    private void Awake()
    {
        background = GetComponent<Image>();
        button = GetComponent<Button>();
    }


    public void Set(ItemSlot[] _inventory, ItemSlot[] _craftBox, CraftRecipe newRecipe)
    {
        curRecipe = newRecipe;
        gameObject.SetActive(true);
        for (int i = 0; i < requiredItemSlotui.Length; i++)
        {
            if ( newRecipe.requiredItems[i].item != null)
            {
                requiredItemSlotui[i].Set(newRecipe.requiredItems[i]);
                requiredItemSlotui[i].gameObject.SetActive(true);
            }
            else
            {
                requiredItemSlotui[i].gameObject.SetActive(false);
            }
        }
        resultItemSlotui.Set(newRecipe.resultItem);
        SetState(_inventory, _craftBox, newRecipe);
    }
    public void Clear()
    {
        curRecipe = null;
        curRecipeState = CraftState.NotEnough;
        SetColor();
        SetButton();

        for (int i = 0; i < requiredItemSlotui.Length; i++)
        {
            requiredItemSlotui[i].Clear();
            requiredItemSlotui[i].gameObject.SetActive(false);
        }
        isEmpty = true;
        gameObject.SetActive(false);
    }

    public void OnClickButton()
    {
        CraftPanelUI.instance.OnClickRecipe(curRecipe);
    }

    public void SetState(ItemSlot[] _inventory, ItemSlot[] _craftBox, CraftRecipe _recipe)
    {
        int need;
        int total_Inventory;
        int total_CraftBox;

        curRecipeState = CraftState.Craftable;

        for (int i = 0; i < _recipe.requiredItems.Length; i++)
        {
            ItemSlot requireItem = _recipe.requiredItems[i];
            need = requireItem.quantity;
            total_Inventory = 0;
            total_CraftBox = 0;

            for (int j = 0; j < _craftBox.Length; j++)
            {
                if (requireItem.item == _craftBox[j].item)
                {
                    total_CraftBox += _craftBox[j].quantity;
                }
            }
            
            for (int j = 0; j < _inventory.Length; j++)
            {
                if (requireItem.item == _inventory[j].item)
                {
                    total_Inventory += _inventory[j].quantity;
                }
            }

            if (need > total_CraftBox)
            {
                curRecipeState = CraftState.EnoughInInventory;
            }

            if (need > total_Inventory + total_CraftBox)
            {
                curRecipeState = CraftState.NotEnough;
                break;
            }
        }
        SetColor();
        SetButton();
    }

    public void SetColor()
    {
        if (curRecipeState == CraftState.Craftable) background.color = Color.white;
        if (curRecipeState == CraftState.EnoughInInventory) background.color = Color.yellow;
        if (curRecipeState == CraftState.NotEnough) background.color = Color.grey;
    }

    public void SetButton()
    {
        if (curRecipeState == CraftState.Craftable) button.interactable = true;
        if (curRecipeState == CraftState.EnoughInInventory) button.interactable = false;
        if (curRecipeState == CraftState.NotEnough) button.interactable = false;
    }
}
