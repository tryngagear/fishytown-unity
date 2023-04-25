using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    public string InteractionPrompt => _prompt;
    
    public static AddItemHandler OnItemPickup;
    public delegate bool AddItemHandler(InvItemData itemData);
    public InvItemData pickUpData;

    public bool Interact(InteractionController interactor){
        if (OnItemPickup?.Invoke(pickUpData)==true) Destroy(gameObject);
        return true;
    }
}
