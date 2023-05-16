using System.Collections.Generic;
public struct Sockets{
    public string posX; 
    public string negX;  
    public string posY; 
    public string negY;
    public string posZ; 
    public string negZ;
}

public class NeighborList{
    public List<BaseGridUnit> posX = new List<BaseGridUnit>(); 
    public List<BaseGridUnit> negX = new List<BaseGridUnit>();  
    public List<BaseGridUnit> posY = new List<BaseGridUnit>(); 
    public List<BaseGridUnit> negY = new List<BaseGridUnit>();
    public List<BaseGridUnit> posZ = new List<BaseGridUnit>(); 
    public List<BaseGridUnit> negZ = new List<BaseGridUnit>();
}

public static class MapGenUtils{
    public static bool CheckAdjacency(BaseGridUnit A, BaseGridUnit B){
        //check face posX
        if(A.sockets.posX == B.sockets.posX)
            A.neighborList.posX.Add(B);
        if(A.sockets.posX == B.sockets.posX + "f")
            A.neighborList.posX.Add(B);
        return false;
    }
}