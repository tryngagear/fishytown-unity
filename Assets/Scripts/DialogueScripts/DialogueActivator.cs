using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    [SerializeField] private Dictionary<string,DialogueObject> dialogue;

    public string InteractionPrompt => _prompt;

    private void Start(){
        dialogue = GetComponentInChildren<JSONReader>().dialogueDict;
    }

    public bool Interact(InteractionController interactor){
        interactor.DialogueUI.ShowDialogue(dialogue,"001");
        return true;
    }
}
