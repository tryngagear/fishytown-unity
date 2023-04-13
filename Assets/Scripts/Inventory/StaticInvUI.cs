using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticInvUI : InventoryUI 
{
    [SerializeField] private InvHolder _invHolder;
    [SerializeField] private InvSlotUI[] _slotsUI;

    protected override void Start() {
        base.Start();
        if(_invHolder != null){
            _invSystem = _invHolder.invSystem;
            _invSystem.OnInvSlotChange += UpdateSlots;
        }

        AssignSlot(_invSystem);
    }

    public override void AssignSlot(InvSystem invToDisplay){
        _slotDict = new Dictionary<InvSlotUI, InvItemSlot>();

        if(_slotsUI.Length != _invSystem.invSize) Debug.Log($"Inventory out of sync on {this.gameObject}");

        for(int i = 0; i < _invSystem.invSize; i++){
            _slotDict.Add(_slotsUI[i], _invSystem.inventorySlots[i]);
            _slotsUI[i].Init(_invSystem.inventorySlots[i]);
        }
    }
}

