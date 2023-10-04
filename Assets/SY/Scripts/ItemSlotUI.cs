using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{

    public int myindex;
    public Button button;
    private Outline outline;

    public Image icon;
    public TextMeshProUGUI quantityText;
    public ItemSlot curSlot;
    public Image background;

    private void Awake()
    {
        background = GetComponent<Image>();
        outline = GetComponent<Outline>();
    }


    public void Set(ItemSlot slot)
    {
        if (slot == null || slot.item == null)
        {
            Clear();
            return;
        }
        curSlot = slot;
        icon.gameObject.SetActive(true);
        icon.sprite = slot.item.Icon;
        quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : string.Empty;

        if (curSlot.isEquipped) { background.color = Color.yellow; }
        else { background.color = Color.white; }
        if (outline != null) { outline.enabled = curSlot.isEquipped; }
    }

    public void Clear()
    {
        curSlot = null;
        icon.gameObject.SetActive(false);
        quantityText.text = string.Empty;

        background.color = Color.white;
        if (outline != null) { outline.enabled = false; }
    }

    public void OnButtonClick()
    {
        if (Inventory.instance.IsOpen())
        {
            Inventory.instance.SelectItem(myindex);
        }
        if (CraftPanelUI.instance.IsOpen())
        {
            CraftPanelUI.instance.SelectItem(curSlot);
        }
    }

    public void SetButton()
    {
        if (curSlot != null)
        {
            if (curSlot.item != null)
            {
                button.enabled = true;
                return;
            }
        }
        button.enabled = false;
    }
}
