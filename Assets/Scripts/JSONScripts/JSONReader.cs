using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JSONReader : MonoBehaviour
{
    public TextAsset JSONtesttext;
    //public DialogueObject dialogueObject;

    [System.Serializable]
    public struct DialogueList
    {
    public List<DialogueObject> dialogueObjects;
    }

    public DialogueList dialogueList = new DialogueList();
    public Dictionary<string, DialogueObject> dialogueDict = new Dictionary<string, DialogueObject>();

    private void Start(){
        dialogueList = JsonUtility.FromJson<DialogueList>(JSONtesttext.text);
        foreach(DialogueObject dialogueOBJ in dialogueList.dialogueObjects){
            dialogueDict.Add(dialogueOBJ.DUID, dialogueOBJ);
        }
    }
}
