using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;

    public bool isOpen {get; private set;}
    
    private ResponseHandler responseHandler;
    private TypeWriter typeWriterEffect;

    private void Start() {
        typeWriterEffect = GetComponent<TypeWriter>();
        responseHandler = GetComponent<ResponseHandler>();
        
        CloseDialogue();
    }

    public void ShowDialogue(Dictionary<string,DialogueObject> dialogueObject, string DUID){
        isOpen = true;
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject,DUID));
    }

    private IEnumerator StepThroughDialogue(Dictionary<string,DialogueObject> dialogueDict,string DUID){
        for(int i = 0; i < dialogueDict[DUID].Dialogue.Length; i++ ){
            string dialogue = dialogueDict[DUID].Dialogue[i];
            yield return typeWriterEffect.Run(dialogue, textLabel);
            if (i == dialogueDict[DUID].Dialogue.Length - 1 && dialogueDict[DUID].HasResponses) break;

            yield return new WaitUntil(() => Keyboard.current.eKey.wasPressedThisFrame);
        }

        if(dialogueDict[DUID].HasResponses){
            responseHandler.ShowResponses(dialogueDict[DUID].Response,dialogueDict);
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
