using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseInvData : MonoBehaviour
{
    public InvItemSlot invSlot;
    public Image _itemSprite;
    public TextMeshProUGUI _itemCount;

    private void Awake() {
        _itemSprite.color = Color.clear;
        _itemCount.text = "";
    }    
   public void UpdateUISlot(InvItemSlot slot){
        if(slot.itemData != null){
            _itemSprite.sprite = slot.itemData.sprite;
            _itemSprite.color = Color.white;
            if(slot.stackSize > 1) _itemCount.text = slot.stackSize.ToString();
            else _itemCount.text = "";
        }
        else
            ClearSlot();
    }

    public void ClearSlot(){
        invSlot.ClearSlot();
        _itemSprite.sprite = null;
        _itemSprite.color = Color.clear;
        _itemCount.text = "";
    }

    public void UpdateMouseSlot(InvItemSlot tempSlot){
        invSlot.AssignSlot(tempSlot);
        UpdateUISlot(tempSlot);
    }

    public void UpdateMouseSlot(){
        UpdateUISlot(invSlot);
    }
}
