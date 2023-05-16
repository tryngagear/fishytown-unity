using UnityEngine;

[System.Serializable]
public class BaseGridUnit 
{   
    public string BGUID;
    private Mesh mesh; 
    public int rotationID;
    public Sockets sockets;
    public NeighborList neighborList;
}
