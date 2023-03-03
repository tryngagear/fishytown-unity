using UnityEngine;

[System.Serializable]
public class DialogueObject 
{  
    [SerializeField] [TextArea] private string[] dialogue;
    public string[] Dialogue => dialogue;
    [SerializeField] private string duid;
    public string DUID => duid;
    public bool HasResponses;
    [SerializeField] private Response[] responses;
    public Response[] Response => responses;
}
