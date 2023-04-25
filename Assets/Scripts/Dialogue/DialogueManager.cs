using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;

    public bool isOpen {get; private set;}
    
    private ResponseHandler _responseHandler;
    private TypeWriter _typeWriterEffect;

    private InputManager _inputManager;

    private void Start() {
        _typeWriterEffect = GetComponent<TypeWriter>();
        _responseHandler = GetComponent<ResponseHandler>();
        _inputManager = InputManager.Instance; 
        CloseDialogue();
    }

    private void OnEnable() {
        ResponseHandler.OnShowDialogue += ShowDialogue; 
        //DialogueActivator.OnShowDialogue += ShowDialogue;
    }

    public void ShowDialogue(Dictionary<string,DialogueObject> dialogueObject, string DUID){
        isOpen = true;
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject,DUID));
    }

    private IEnumerator StepThroughDialogue(Dictionary<string,DialogueObject> dialogueDict,string DUID){
        for(int i = 0; i < dialogueDict[DUID].Dialogue.Length; i++ ){
            string dialogue = dialogueDict[DUID].Dialogue[i];
            yield return _typeWriterEffect.Run(dialogue, textLabel);
            if (i == dialogueDict[DUID].Dialogue.Length - 1 && dialogueDict[DUID].HasResponses) break;

            yield return new WaitUntil(() => _inputManager.GetPlayerInteract());
        }

        if(dialogueDict[DUID].HasResponses){
            _responseHandler.ShowResponses(dialogueDict[DUID].Response,dialogueDict);
        }else{
            CloseDialogue();
        }
    }

    private void CloseDialogue(){
        isOpen = false;
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
    }    
}
