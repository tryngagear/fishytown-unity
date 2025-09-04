using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "BaseGridUnit", menuName = "ScriptableObjects/BaseGridUnit")]
public class BaseGridUnit : ScriptableObject 
{   
    //the actual tile
    public GameObject refObj;
    public string BGUID;
    //The key for the asset path of its fbx
    public string meshRef; 
    //valid rotations: 0,90,180,270
    public int[] rotationList;
    public float weight;
    public Sockets sockets;
}

[System.Serializable]
public class BaseGridUnitVirtual
{   
    public string BGUID;
    public string meshRef; 
    public int rotationID;
    public float weight;
    public Sockets sockets;
    public BaseGridUnitVirtual(){}
    public BaseGridUnitVirtual(BaseGridUnit BGUI, int newRotID, Sockets newSockets){
        BGUID           = BGUI.BGUID;
        meshRef         = BGUI.meshRef;
        rotationID      = newRotID;
        weight          = BGUI.weight; 
        sockets         = newSockets;
    }
    public BaseGridUnitVirtual(string newBGUID, string newMeshRef, int newRotID, float newWeight,Sockets newSockets){
        BGUID           = newBGUID;
        meshRef         = newMeshRef;
        rotationID      = newRotID;
        weight          = newWeight; 
        sockets         = newSockets;
    }
}

[System.Serializable]
public class BGURef
{
    public string BGUID;
    public string meshRef;
    public int rotID;
    public float weight;
    public List<string>[] Neighbors = new List<string>[6];
    
    public List<string> NEGX;
    public List<string> POSX;
    public List<string> NEGY;
    public List<string> POSY;
    public List<string> NEGZ;
    public List<string> POSZ;
    
    public BGURef(BaseGridUnitVirtual newBGU){
        BGUID       = newBGU.BGUID;
        meshRef     = newBGU.meshRef;
        rotID       = newBGU.rotationID;
        weight      = newBGU.weight;
        Neighbors = new List<string>[6];

        Neighbors[4] = POSZ = new List<string>(newBGU.sockets.posZ.validNeighbors);
        Neighbors[5] = NEGZ = new List<string>(newBGU.sockets.negZ.validNeighbors);
        Neighbors[2] = NEGX = new List<string>(newBGU.sockets.negX.validNeighbors);
        Neighbors[0] = POSX = new List<string>(newBGU.sockets.posX.validNeighbors);
        Neighbors[1] = POSY = new List<string>(newBGU.sockets.posY.validNeighbors);
        Neighbors[3] = NEGY = new List<string>(newBGU.sockets.negY.validNeighbors);
    }
    public BGURef(BGURef newBGU){
        BGUID       = newBGU.BGUID;
        meshRef     = newBGU.meshRef;
        rotID       = newBGU.rotID;
        weight      = newBGU.weight;
        Neighbors = new List<string>[6];

        Neighbors[4] = POSZ = new List<string>(newBGU.POSZ);
        Neighbors[5] = NEGZ = new List<string>(newBGU.NEGZ);
        Neighbors[2] = NEGX = new List<string>(newBGU.NEGX);
        Neighbors[0] = POSX = new List<string>(newBGU.POSX);
        Neighbors[1] = POSY = new List<string>(newBGU.POSY);
        Neighbors[3] = NEGY = new List<string>(newBGU.NEGY);
    }

}

[System.Serializable]
public class GridLiteral
{
    public Mesh mesh;
    public int[] coord = new int[2];
    public Material mat;
}

[System.Serializable]
public struct face{
    //is this face reflectionally symmetrical
    public bool sym;
    //if not symetrical set polarity 
    public bool polarity;
    public string FID;
    public HashSet<string> validNeighbors;
    /*public face(){
        validNeighbors = new HashSet<string>();
        sym = false;
        polarity = false; 
    }*/
    public face(face newFace){
        validNeighbors = new HashSet<string>();
        FID = newFace.FID;
        sym = newFace.sym;
        polarity = newFace.polarity;
    }
    public face(string NFID){
        validNeighbors = new HashSet<string>();
        sym = false;
        polarity = false;
        FID = NFID; 
    }
}

[System.Serializable]
public struct Sockets{
    public face posX; 
    public face negX;
    public face posY; 
    public face negY;   
    public face posZ; 
    public face negZ; 
    public Sockets(face posX, face negX, face posY, face negY, face posZ, face negZ){
        this.posX = new face(posX);
        this.negX = new face(negX);
        this.posY = new face(posY);
        this.negY = new face(negY);
        this.posZ = new face(posZ);
        this.negZ = new face(negZ);
    }
    public Sockets(Sockets newSockets){
        this.posX = new face(newSockets.posX);
        this.negX = new face(newSockets.negX);
        this.posY = new face(newSockets.posY);
        this.negY = new face(newSockets.negY);
        this.posZ = new face(newSockets.posZ);
        this.negZ = new face(newSockets.negZ);
    }
}
