using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class CraftPanelUI : MonoBehaviour
{
    public static CraftPanelUI instance;

    [HideInInspector] public Inventory userInventory;
    [HideInInspector] public ItemSlot[] resources;
    [HideInInspector] public ItemSlot[] craftBox;

    [Header("Insert your recipes here")]
    public CraftRecipe[] craftRecipes;

    [Header("ResourcesPanel - SelectedItemInfo")]
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public GameObject addButton;
    public GameObject removeButton;
    [Header("ResourcesPanel - ViewPort Slots")]
    public ItemSlotUI[] resourcesUI;
    [Header("CraftPanel - CraftBox")]
    public Image craftBoxBackground;
    public ItemSlotUI[] craftBoxUI;
    public ItemSlotUI craftBoxResultUI;
    [Header("CraftPanel - ResultItemInfo")]
    public ItemSlotUI craftBoxResultUI2;
    public TextMeshProUGUI craftResultItemName;
    public TextMeshProUGUI craftResultItemDescription;
    public TextMeshProUGUI craftResultItemStatNames;
    public TextMeshProUGUI craftResultItemStatValues;
    [Header("CraftPanel - Recipes")]
    public RecipeUI[] curRecipesUI;
    public GameObject craftButton;
    [Header("Events")]
    public UnityEvent onOpenInventory;
    public UnityEvent onCloseInventory;

    [HideInInspector] public Dictionary<ItemData, List<CraftRecipe>> recipeDictionary;
    public List<CraftRecipe> curRecipesList;
    [HideInInspector] public CraftRecipe selectedRecipe;
    [HideInInspector] public CraftState craftBoxState;
    [HideInInspector] public ItemSlot selectedItem;

    private void Awake()
    {
        instance = this;
        craftBox = new ItemSlot[craftBoxUI.Length];
        resources = new ItemSlot[resourcesUI.Length];
        for (int i = 0; i < craftBoxUI.Length; i++)
        {
            craftBox[i] = new ItemSlot();
            craftBoxUI[i].myindex = i + resourcesUI.Length;
        }
        for(int i = 0; i < resourcesUI.Length; i++)
        {
            resourcesUI[i].myindex = i;
        }
        recipeDictionary = new Dictionary<ItemData, List<CraftRecipe>>();
        MakeDictionaries();
    }

    private void Start()
    {
        userInventory = Inventory.instance;
        gameObject.SetActive(false);
        ClearSelectedItemWindow();
        // ClearResultItemWindow();
    }

    public void Toggle()
    {
        ClearResultItem();
        for (int i = 0; i < craftBoxUI.Length; i++)
        {
            if (craftBox[i].item != null)
            {
                craftBoxUI[i].Set(craftBox[i]);
            }
            else
            {
                craftBoxUI[i].Clear();
            }
        }

        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            onCloseInventory?.Invoke();
        }
        else
        {
            gameObject.SetActive(true);
            onOpenInventory?.Invoke();
        }
        UpdateResources();
        ResetCurRecipes();
    }

    public bool HasItem(ItemData item)
    {
        for (int i = 0; i < craftBox.Length; i++)
        { if (craftBox[i].item == item && craftBox[i].quantity > 0) return true; }
        return false;
    }
    public bool IsOpen()
    {
        return gameObject.activeInHierarchy;
    }

    public void SelectItem(ItemSlot item)
    {
        if (item == null || item.item == null) 
        {
            ClearSelectedItemWindow();
            return; 
        }

        selectedItem = item;
        selectedItemName.text = selectedItem.item.DisplayName;
        selectedItemDescription.text = selectedItem.item.Description;
        ResetButtons();

    }

    public bool AddItemToCraftBox(ItemData item)
    {
        if (item.IsStackable)
        {
            ItemSlot slotToStackTo = GetItemStackInCraftBox(item);
            if (slotToStackTo != null)
            {
                slotToStackTo.quantity++;
                UpdateResourcesUI();
                UpdateCraftBoxUI();
                return true;
            }
        }
        ItemSlot emptySlot = GetEmptySlotInCraftBox();

        if (emptySlot != null)
        {
            emptySlot.item = item;
            emptySlot.quantity = 1;
            UpdateResourcesUI();
            UpdateCraftBoxUI();
            return true;
        }
        return false;
    }

    public void UpdateResources()
    {
        int index = 0;
        for (int i=0; i< resources.Length; i++)
        {
            resources[i] = null;
        }
        for (int i=0; i< userInventory.slots.Length; i++)
        {
            if(userInventory.slots[i].item != null && userInventory.slots[i].item.Type == ItemType.Resource)
            {
                resources[index] = userInventory.slots[i];
                index++;
            }
        }
        UpdateResourcesUI();
    }
    public void UpdateResourcesUI()
    {
        for (int i = 0; i < resourcesUI.Length; i++)
        {
            if (resources[i] != null && resources[i].item != null)
            {
                resourcesUI[i].Set(resources[i]);
                resourcesUI[i].gameObject.SetActive(true);
                continue;
            }
            resourcesUI[i].Clear();
            resourcesUI[i].gameObject.SetActive(false);
        }
    }
    public void UpdateCraftBoxUI()
    {
        for (int i = 0; i < craftBoxUI.Length; i++)
        {
            if (craftBox[i].item != null)
            {
                craftBoxUI[i].Set(craftBox[i]);
                craftBoxUI[i].SetButton();
            }
            else
            {
                craftBoxUI[i].Clear();
                craftBoxUI[i].SetButton();
            }
        }
    }
    public ItemSlot GetItemStackInCraftBox(ItemData item)
    {
        for (int i = 0; i < craftBoxUI.Length; i++)
        {
            if (craftBox[i] != null)
            {
                if (craftBox[i].item == item && craftBox[i].quantity < item.MaxStackAmount)
                    return craftBox[i];
            }
        }
        return null;
    }

    public ItemSlot GetEmptySlotInCraftBox()
    {
        for (int i = 0; i < craftBoxUI.Length; i++)
        {
            if (craftBox[i].item == null)
            {
                return craftBox[i];
            }
        }
        return null;
    }

    public void ClearSelectedItemWindow()
    {
        selectedItem = null;
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;

        ResetButtons();
    }
    public void ResetButtons()
    {
        addButton.SetActive(false);
        removeButton.SetActive(false);
        if (selectedItem == null || selectedItem.item == null || selectedItem.quantity < 1) return;
        if (Inventory.instance.HasItem(selectedItem.item)) addButton.SetActive(true);
        if (instance.HasItem(selectedItem.item)) removeButton.SetActive(true);
    }

    public bool HasItems(ItemData item, int quantity)
    {
        return false;
    }

    public void OnClickAddButton()
    {
        ItemData temp = selectedItem.item;
        if (Inventory.instance.RemoveItem(temp))
        {
            AddItemToCraftBox(temp);
            UpdateResources();
            UpdateCraftBoxUI();
            selectedItem = FindSlotInCraftBox(temp);
            ResetButtons();
            ResetCurRecipes();
            CheckCraftable();
        }
    }

    public void OnClickRemoveButton()
    {
        ItemData temp = selectedItem.item;
        if (RemoveItemInCraftBox(temp))
        {
            Inventory.instance.AddItem(temp);
            UpdateResources();
            UpdateCraftBoxUI();
            selectedItem = FindSlotInResources(temp);
            ResetButtons();
            ResetCurRecipes();
            CheckCraftable();
        }
    }
    public void OnClickRecipe(CraftRecipe _recipe)
    {
        selectedRecipe = _recipe;
        SetResultItem(_recipe.resultItem);
    }

    public void OnClickMakeButton()
    {
        for (int i=0; i < selectedRecipe.requiredItems.Length; i++)
        {
            for (int j=0; j < selectedRecipe.requiredItems[i].quantity; j++)
            {
                RemoveItemInCraftBox(selectedRecipe.requiredItems[i].item);
            }
        }
        for (int i=0; i < selectedRecipe.resultItem.quantity; i++)
        {
            Inventory.instance.AddItem(selectedRecipe.resultItem.item);
        }
        UpdateCraftBoxUI();
        ResetButtons();
        ResetCurRecipes();
        CheckCraftable();
    }

    public void CheckCraftable()
    {
        if (selectedRecipe == null) return;
        craftButton.SetActive(true);
        int need;
        int total;
        
        for (int i = 0; i < selectedRecipe.requiredItems.Length; i++)
        {
            ItemSlot requireItem = selectedRecipe.requiredItems[i];
            need = requireItem.quantity;
            total = 0;

            for (int j = 0; j < craftBox.Length; j++)
            {
                if (requireItem.item == craftBox[j].item)
                {
                    total += craftBox[j].quantity;
                }
            }

            if (need > total)
            {
                craftButton.SetActive(false);
                return;
            }
        }
    }

    public void MakeDictionaries()
    {
        int index = 0;
        for (int i=0; i < craftRecipes.Length; i++)
        {
            for (int j=0; j < craftRecipes[i].requiredItems.Length; j++)
            {
                if (craftRecipes[i].requiredItems[j].item != null)
                {
                    ItemData itemData = craftRecipes[i].requiredItems[j].item;
                    if (recipeDictionary.ContainsKey(itemData))
                    {
                        if (recipeDictionary[itemData].Contains(craftRecipes[i]))
                        {
                            continue;
                        }
                        else
                        {
                            index++;
                            recipeDictionary[itemData].Add(craftRecipes[i]);
                        }
                    }
                    else
                    {
                        index++;
                        List<CraftRecipe> newList = new List<CraftRecipe>();
                        newList.Add(craftRecipes[i]);
                        recipeDictionary.Add(itemData, newList);
                    }
                }
            }
        }
    }

    public void SetCraftBoxState(ItemSlot resultItem)
    {
        if ( craftBoxState == CraftState.NotEnough )
        {
            craftBoxBackground.color = Color.white;
            craftBoxResultUI.Set(resultItem);
        }
    }

    public void ResetCurRecipesUI()
    {
        CraftRecipe[] temp = curRecipesList.ToArray();
        for (int i=0; i < curRecipesUI.Length; i++)
        {
            if (temp.Length > i && temp[i] != null)
            {
                curRecipesUI[i].Set(userInventory.slots, craftBox, temp[i]);
            }
            else
            {
                curRecipesUI[i].Clear();
            }
        }
        ArrangeRecipes();
    }

    public void ResetCurRecipes()
    {
        int index = 0;
        curRecipesList.Clear();
        for (int i = 0; i < craftBox.Length; i++)
        {
            if (craftBox[i].item != null)
            {
                if (recipeDictionary.ContainsKey(craftBox[i].item))
                {
                    curRecipesList = index ==0? recipeDictionary[craftBox[i].item].ToList() : curRecipesList.Intersect(recipeDictionary[craftBox[i].item]).ToList();
                    index++;
                }
            }
        }
        ResetCurRecipesUI();
    }
    public void ArrangeRecipes()
    {
        for (int i= 0; i < curRecipesUI.Length; i++)
        {
            if (curRecipesUI[i].curRecipeState == CraftState.Craftable) curRecipesUI[i].gameObject.transform.SetAsFirstSibling();
            if (curRecipesUI[i].curRecipeState == CraftState.NotEnough) curRecipesUI[i].gameObject.transform.SetAsLastSibling();
        }
    }

    public void SetResultItem(ItemSlot itemslot)
    {
        craftBoxResultUI.Set(itemslot);
        craftBoxResultUI2.Set(itemslot);
        craftBoxResultUI2.gameObject.SetActive(true);

        craftResultItemName.text = itemslot.item.DisplayName;
        craftResultItemDescription.text = itemslot.item.Description;
        craftResultItemStatNames.text = string.Empty;
        craftResultItemStatValues.text = string.Empty;

        if (itemslot.item.Consumables != null) 
        {
            for(int i= 0; i < itemslot.item.Consumables.Length; i++)
            {
                craftResultItemStatNames.text += itemslot.item.Consumables[i].Type.ToString() + "\n";
                craftResultItemStatValues.text += itemslot.item.Consumables[i].Value.ToString() + "\n";
            }
        }

        craftButton.SetActive(true);
    }

    public void ClearResultItem()
    {
        craftBoxResultUI.Clear();
        craftBoxResultUI2.Clear();
        craftBoxResultUI2.gameObject.SetActive(false);

        craftResultItemName.text = string.Empty;
        craftResultItemDescription.text = string.Empty;
        craftResultItemStatNames.text = string.Empty;
        craftResultItemStatValues.text = string.Empty;

        craftButton.SetActive(false);
    }
    public bool RemoveItemInCraftBox(ItemData item)
    {
        ItemSlot temp = FindSlotInCraftBox(item);
        if (temp != null && temp.quantity > 0) 
        {
            temp.quantity--;
            if (temp.quantity <= 0)
            {
                temp.item = null;
            }
            UpdateCraftBoxUI();
            return true;
        }
        return false;
    }

    public ItemSlot FindSlotInCraftBox(ItemData item)
    {
        for (int i = 0; i < craftBox.Length; i++)
        {
            if (craftBox[i].item == item && craftBox[i].quantity > 0) return craftBox[i];
        }
        return null;
    }

    public ItemSlot FindSlotInResources(ItemData item)
    {
        for (int i=0; i < resources.Length; i++)
        {
            if (resources[i].item == item && resources[i].quantity > 0) return resources[i];
        }
        return null;
    }


}
