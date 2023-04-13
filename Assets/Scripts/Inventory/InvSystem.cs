using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class InvSystem 
{

    [SerializeField] private List<InvItemSlot> _inventorySlots;
    //private int _invSize = 10;

    public List<InvItemSlot> inventorySlots => _inventorySlots;
    public int invSize => inventorySlots.Count;

    public UnityAction<InvItemSlot> OnInvSlotChange;

    public InvSystem(int size){
        _inventorySlots = new List<InvItemSlot>(size);
        OnEnable();
        for(int i = 0; i < size; i++)
            _inventorySlots.Add(new InvItemSlot());
    }


    private void OnEnable() {
        PickupActivator.OnItemPickup += AddItem;
    }

    private void OnDisable() {
        PickupActivator.OnItemPickup -= AddItem;
    }

    public bool AddItem(InvItemData itemData){
        if(ContainsItem(itemData, out List<InvItemSlot> invSlot)){
            foreach(var slot in invSlot){
                if(slot.CheckStackSize(1)){
                    slot.IncrementStack();
                    OnInvSlotChange?.Invoke(slot);
                    return true;
                }
            }
        }
        if(HasFreeSlot(out InvItemSlot freeSlot)){
            freeSlot.UpdateInvSlot(itemData, 1);
            OnInvSlotChange?.Invoke(freeSlot);
            return true;
        }
        return false;
    }

    public void RemoveItem(){

    }

    public bool ContainsItem(InvItemData itemToAdd, out List<InvItemSlot> invSlot){
        invSlot = _inventorySlots.Where(i => i.itemData == itemToAdd).ToList();
        return invSlot.Count >= 1 ? true: false;
    }

    public bool HasFreeSlot(out InvItemSlot freeSlot){
        freeSlot = _inventorySlots.FirstOrDefault(i=> i.itemData == null);
        return freeSlot == null ? false:true;
    }
}
