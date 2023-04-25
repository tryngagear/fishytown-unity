using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LegacyDialogueObject", menuName = "ScriptableObjects/DialogueObject", order = 0)]
public class LegacyDialogueObject : ScriptableObject {
    [SerializeField] [TextArea] private string[] dialogue;
    [SerializeField] private Response[] responses;

    public string[] Dialogue => dialogue;

    public bool HasResponses =>  responses != null && responses.Length > 0;

    public Response[] Response => responses;
}

