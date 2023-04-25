using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/************************************
The individual item slots in the inventory
************************************/
[Serializable]
public class InvItemSlot
{
    
    [SerializeField] private InvItemData _itemData;
    [SerializeField] private int _stackSize;
    [SerializeField] private bool _placeable;

    public InvItemData itemData => _itemData;
    public int stackSize => _stackSize;
    public bool placeable => _placeable;

    public InvItemSlot(InvItemData item, int amount){
        _itemData = item;
	    _stackSize = amount;
    }

    public InvItemSlot(){
    	ClearSlot();
    }

    public void ClearSlot(){
    	_itemData = null;
    	_stackSize = -1;
    }

    public void UpdateInvSlot(InvItemData data, int amount){
        _itemData = data;
        _stackSize = amount;
    }
    public bool CheckStackSize(int amountToAdd, out int sizeRemaining){
        sizeRemaining = _itemData.maxSize - _stackSize;
   	    return CheckStackSize(amountToAdd);
    }

    public bool CheckStackSize(int amountToAdd){
   	    if(_itemData.maxSize >= (amountToAdd+stackSize))    return true;
        return false;
    }
	public void AddToStack(int amount){
    	_stackSize += amount;
    }

    public void RemoveFromStack(int amount){
    	_stackSize -= amount;
    }

    public void AssignSlot(InvItemSlot inSlot){
        if(_itemData == inSlot.itemData) AddToStack(inSlot.stackSize);
        else
        {
            _itemData = inSlot.itemData;
            _stackSize = 0;
            AddToStack(inSlot.stackSize);
        }
    }
    public void IncrementStack(){
        _stackSize++;
    }

    public void DecrementStack(){
        _stackSize--;
    }
}
