using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private RectTransform responseBox;
    [SerializeField] private RectTransform responseButtonTemplate;
    [SerializeField] private RectTransform responseContainer;

    //private DialogueManager _dialogueManager;

    public static AddDialogueHandler OnShowDialogue; 

    public delegate void AddDialogueHandler(Dictionary<string,DialogueObject> dialogueObject, string DUID);
    private List<GameObject> tempResponseButtons = new List<GameObject>();

    private void Start() {
        //_dialogueManager = DialogueManager.Instance;       
    }

    public void ShowResponses(Response[] responses, Dictionary<string,DialogueObject> dialogueDict){
        float responseBoxHeight = 0;

        foreach (Response response in responses){
            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
            responseButton.gameObject.SetActive(true);
            responseButton.GetComponent<TMP_Text>().text = response.ResponseText;
            responseButton.GetComponent<Button>().onClick.AddListener(()=>OnPickedResponse(response, dialogueDict));
            responseBoxHeight += responseButtonTemplate.sizeDelta.y;

            tempResponseButtons.Add(responseButton);

        }
        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
        responseBox.gameObject.SetActive(true);

    }
    private void OnPickedResponse(Response response, Dictionary<string,DialogueObject> dialogueDict){
        responseBox.gameObject.SetActive(false);

        foreach (GameObject button in tempResponseButtons){
            Destroy(button);
        }
        //_dialogueManager.ShowDialogue(dialogueDict,response.NextDUID);
        OnShowDialogue?.Invoke(dialogueDict, response.NextDUID);
    }
}
