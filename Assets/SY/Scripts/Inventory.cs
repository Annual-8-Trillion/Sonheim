using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;


[Serializable]
public class ItemSlot
{
    public ItemData item;
    [HideInInspector]
    public bool isEquipped;
    public int quantity;

    public ItemSlot()
    {
        isEquipped = false;
    }
}


public class Inventory : MonoBehaviour
{
    public ItemSlotUI[] uiSlots;
    public ItemSlot[] slots;
    public ItemSlot[] equips;

    public GameObject inventoryPanel;

    [Header("Selected Item")]
    [SerializeField] private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatNames;
    public TextMeshProUGUI selectedItemStatValues;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipButton;
    public GameObject dropButton;
    public GameObject placeButton;

    private int curEquipIndex;
    private Player player;

    private bool isExistEquipInventory;
    private EquipInventory equipInventory;

    [Header("Events")]
    public UnityEvent onOpenInventory;
    public UnityEvent onCloseInventory;

    public static Inventory instance;

    private ItemManager itemManager;
    private PlayerController controller;
    private PlayerObjectInstaller installer;

    private void Awake()
    {
        instance = this;
        player = GetComponent<Player>();
        if(TryGetComponent(out EquipInventory equipInventory))
        {
            isExistEquipInventory = true;
        }
        else
        {
            equips = new ItemSlot[3];
            for (int i = 0; i < equips.Length; i++)
            {
                equips[i] = new ItemSlot();
            }
        }

        controller = GetComponent<PlayerController>();
        installer = GetComponent<PlayerObjectInstaller>();
    }

    private void Start()
    {
        inventoryPanel.SetActive(false);
        slots = new ItemSlot[uiSlots.Length];

        for(int i= 0; i <slots.Length ; i++)
        {
            slots[i] = new ItemSlot();
            uiSlots[i].myindex = i;
            uiSlots[i].Clear();
        }

        itemManager = ItemManager._instance;

        ClearSelectedItemWindow();
    }

    public void Toggle()
    {
        if (inventoryPanel.activeInHierarchy)
        {
            inventoryPanel.SetActive(false);
            onCloseInventory?.Invoke();
            UpdateUI();
            
        }
        else
        {
            inventoryPanel.SetActive(true);
            onOpenInventory?.Invoke();
            UpdateUI();
        }
    }

    public bool IsOpen()
    {
        return inventoryPanel.activeInHierarchy;
    }

    public void AddItem(ItemData item)
    {
        if (item.IsStackable)
        {
            ItemSlot slotToStackTo = GetItemStack(item);
            if(slotToStackTo != null)
            {
                slotToStackTo.quantity++;
                UpdateUI();
                return;
            }
        }
        ItemSlot emptySlot = GetEmptySlot();
        
        if (emptySlot != null)
        {
            emptySlot.item = item;
            emptySlot.quantity = 1;
            UpdateUI();
            return;
        }
        ThrowItem(item);
    }

    public void ThrowItem(ItemData item)
    {
        Vector3 itemRespawnPosition = gameObject.transform.position + controller.direction;
        itemRespawnPosition = itemRespawnPosition + new Vector3(0, 1, -1);
        itemManager.DropNewItem(itemRespawnPosition, item);
        Debug.Log("아이템을 던졌다. :  " + item.DisplayName);
    }

    public void UpdateUI()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                uiSlots[i].Set(slots[i]);
            }
            else
            {
                uiSlots[i].Clear();
            }
        }
    }

    public ItemSlot GetItemStack(ItemData item)
    {
        for( int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item && slots[i].quantity < item.MaxStackAmount)
                return slots[i];
        }
        return null;
    }

    public ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i];
            }
        }
        return null;
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;

        selectedItem = slots[index];
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.item.DisplayName;
        selectedItemDescription.text = selectedItem.item.Description;

        selectedItemStatNames.text = string.Empty;
        selectedItemStatValues.text = string.Empty;

        if(selectedItem.item.Consumables != null) 
        {
            for (int i = 0; i < selectedItem.item.Consumables.Length; i++)
            {
                selectedItemStatNames.text += selectedItem.item.Consumables[i].Type.ToString() + "\n";
                selectedItemStatValues.text += selectedItem.item.Consumables[i].Value.ToString() + "\n";
            }
        }


        useButton.SetActive(selectedItem.item.Type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.item.Type == ItemType.Equipable && !slots[index].isEquipped);
        unequipButton.SetActive(selectedItem.item.Type == ItemType.Equipable && slots[index].isEquipped);
        placeButton.SetActive(selectedItem.item.Type == ItemType.Installable);
        dropButton.SetActive(true);
    }
    public void ClearSelectedItemWindow()
    {
        selectedItem = null;
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;

        selectedItemStatNames.text = string.Empty;
        selectedItemStatValues.text = string.Empty;

        useButton.SetActive(false);
        equipButton.SetActive(false);
        unequipButton.SetActive(false);
        dropButton.SetActive(false);
        placeButton.SetActive(false);
    }

    public void OnUseButton()
    {
        if (selectedItem.item.Type == ItemType.Consumable)
        {
            for (int i = 0; i < selectedItem.item.Consumables.Length; i++)
            {
                switch (selectedItem.item.Consumables[i].Type)
                {
                    case ConsumableType.Hunger:
                        player.AddHunger((float)selectedItem.item.Consumables[i].Value,false);
                        break;

                    case ConsumableType.Thirst:
                        player.AddThirst((float)selectedItem.item.Consumables[i].Value,false);
                        break;
                    case ConsumableType.Health:
                        player.AddHp((float)selectedItem.item.Consumables[i].Value, false);
                        break;
                    case ConsumableType.Stamina:
                        player.AddStamina((float)selectedItem.item.Consumables[i].Value, false);
                        break;
                }
            }
        }
        RemoveSelectedItem();
    }
    public void OnEquipButton()
    {
        if (!isExistEquipInventory)
        {
            EquipHere();
        }
    }
    public void OnUnequipButton()
    {
        if (!isExistEquipInventory)
        {
            UnequipHere();
        }
    }

    public void OnPlaceButton()
    {
        GameObject prefab = selectedItem.item.InstallablePrefab;

        bool isSuccess = installer.InstallObject(prefab);
        if (isSuccess) RemoveSelectedItem();
    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem.item);
        RemoveSelectedItem();
    }

    public void RemoveSelectedItem()
    {
        selectedItem.quantity--;
        if(selectedItem.quantity <= 0)
        {
            if (slots[selectedItemIndex].isEquipped)
            {
                UnequipHere();
            }
            selectedItem.item = null;
            ClearSelectedItemWindow();
        }
        UpdateUI();
    }
    public bool HasItem(ItemData item)
    {
        for(int i=0; i <slots.Length; i++)
        { if (slots[i].item == item && slots[i].quantity > 0) return true; }
        return false;
    }

    public void UnequipHere()
    {

        selectedItem.isEquipped = false;
        if (equips[0] == selectedItem)
        {
            player.UnEquipWeapon(equips[0].item.WeaponPrefab);
            equips[0] = null;
        }


        UpdateUI();
        SelectItem(selectedItemIndex);
    }

    public void EquipHere()
    {
        selectedItem.isEquipped = true;
        if (equips[0] != null) equips[0].isEquipped = false;
        equips[0] = selectedItem;

        player.EquipWeapon(selectedItem.item.WeaponPrefab);

        UpdateUI();
        SelectItem(selectedItemIndex);
    }

    public ItemSlot FindSlot(ItemData item)
    {
        for (int i=0; i < slots.Length; i++)
        {
            if( slots[i].item == item) return slots[i];
        }
        return null;
    }

    public bool RemoveItem(ItemData item)
    {
        ItemSlot temp = FindSlot(item);
        if (temp != null && temp.quantity > 0)
        {
            temp.quantity--;
            if (temp.quantity <= 0)
            {
                temp.item = null;
                ClearSelectedItemWindow();
            }

            if (inventoryPanel.activeInHierarchy) { UpdateUI(); }
            else { CraftPanelUI.instance.UpdateResourcesUI(); }            
            return true;
        }
        return false;
    }
}
