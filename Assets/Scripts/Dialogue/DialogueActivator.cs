using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    [SerializeField] private Dictionary<string,DialogueObject> dialogue;
    private DialogueManager _dialogueManager;
    public string InteractionPrompt => _prompt;

    private void Start(){
        _dialogueManager = DialogueManager.Instance;
        dialogue = GetComponentInChildren<JSONReader>().dialogueDict;
    }

    public bool Interact(InteractionController interactor){
        _dialogueManager.ShowDialogue(dialogue,"001");
        return true;
    }
}
