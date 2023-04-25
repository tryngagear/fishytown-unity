using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InvHolder : MonoBehaviour
{
    [SerializeField] private int _inventorySize;
    [SerializeField] protected InvSystem _invSystem;

    public InvSystem invSystem => _invSystem;

    public static UnityAction<InvSystem> onDynamicInventoryDisplayRequested;

    private void Awake() {
        _invSystem = new InvSystem(_inventorySize);
    }
}
