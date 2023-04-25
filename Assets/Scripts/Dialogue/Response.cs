using UnityEngine;

[System.Serializable]
public class Response
{
    [SerializeField] private string responseText;
    [SerializeField] private string nextDUID;
    public string ResponseText => responseText;

    public string NextDUID => nextDUID;
}
