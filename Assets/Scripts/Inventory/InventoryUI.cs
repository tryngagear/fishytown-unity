using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryUI : MonoBehaviour
{
    [SerializeField] MouseInvData _mouseInvItem;
    protected InvSystem _invSystem;
    protected Dictionary<InvSlotUI, InvItemSlot> _slotDict;

    public InvSystem invSystem => _invSystem;
    public Dictionary<InvSlotUI, InvItemSlot> slotDict => _slotDict;

    protected virtual void Start() {
        
    }
    public abstract void AssignSlot(InvSystem invToDisplay);

    protected virtual void UpdateSlots(InvItemSlot updatedSlot){
        foreach(var slot in _slotDict){
            if(slot.Value == updatedSlot){
                slot.Key.UpdateUISlot(updatedSlot);
            }
        }
    }

    public void SlotClicked(InvSlotUI clickedSlot){
        if(clickedSlot.AssSlot.itemData != null && _mouseInvItem.invSlot.itemData != null){ //both slots are filled
            if(clickedSlot.AssSlot.itemData == _mouseInvItem.invSlot.itemData){  //they are the same item
                if(clickedSlot.AssSlot.CheckStackSize(_mouseInvItem.invSlot.stackSize, out int leftInStack)){ //their is enough room for them to stack togther
                    clickedSlot.AssSlot.AssignSlot(_mouseInvItem.invSlot);
                    clickedSlot.UpdateUISlot();
                    _mouseInvItem.ClearSlot();
                }else{ //there is not enough room
                    if(leftInStack < 1) SwapSlots(clickedSlot); //if the inventory slot is full swap 
                    else{ //otherwise add the remaining to the stack
                        clickedSlot.AssSlot.AddToStack(leftInStack);
                        clickedSlot.UpdateUISlot();
                        _mouseInvItem.invSlot.RemoveFromStack(leftInStack);
                        _mouseInvItem.UpdateMouseSlot();
                    }
                }
            }else{ //they are not the same item so swap them
                SwapSlots(clickedSlot);
            }

            return;
        }

        if(clickedSlot.AssSlot.itemData != null && _mouseInvItem.invSlot.itemData == null){ //inventory slot is empty so place item down
            _mouseInvItem.UpdateMouseSlot(clickedSlot.AssSlot);
            clickedSlot.ClearSlot();
            return;
        }

        if(clickedSlot.AssSlot.itemData == null && _mouseInvItem.invSlot.itemData != null){ //inventory slot is not empty so pick item up
            clickedSlot.AssSlot.AssignSlot(_mouseInvItem.invSlot);
            clickedSlot.UpdateUISlot(_mouseInvItem.invSlot);
            _mouseInvItem.ClearSlot();
            return;
            }
    }

    private void SwapSlots(InvSlotUI a){ //swap between pickup slot and inventory slot
        var clonedSlot = new InvItemSlot(_mouseInvItem.invSlot.itemData, _mouseInvItem.invSlot.stackSize);
        _mouseInvItem.ClearSlot();
        _mouseInvItem.UpdateMouseSlot(a.AssSlot);

        a.ClearSlot();
        a.AssSlot.AssignSlot(clonedSlot);
        a.UpdateUISlot();
    }

}
