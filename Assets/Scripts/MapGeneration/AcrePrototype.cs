using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveCell{
    public int x,y,z;
    private bool isCollapsed;
    public List<BGURef> PossibleTiles;
    float sumWeight;
    float sumLogWeight;
    int sumOne;
    public WaveCell(List<BGURef> bguRefList, int nx, int ny, int nz){
        x = nx; y = ny; z = nz;
        isCollapsed = false;
        PossibleTiles = new List<BGURef>();
        for(int t = 0; t < bguRefList.Count; t++){
            PossibleTiles.Add(new BGURef(bguRefList[t]));
        }
        sumOne = 0;
        sumWeight = 0;
        sumLogWeight = 0;
        foreach(BGURef pt in PossibleTiles){
            sumOne += 1;
            sumWeight += pt.weight;
            sumLogWeight  += pt.weight * Mathf.Log(pt.weight);
        }
    }
    public List<BGURef> GetPossibleTiles() => PossibleTiles;
    public void Collapse(){
        if (PossibleTiles.Count == 1){
            isCollapsed = true;
            sumWeight = sumLogWeight = 0;
            sumOne       = 0;
        }else{
            System.Random random = new System.Random();
            double[] distro = new double[sumOne];
            for(int d = 0; d < sumOne; d++){
                distro[d] = PossibleTiles[d].weight;
            }
            bool[] w = new bool[sumOne];
            int r = distro.MapGenRandom(random.NextDouble());
            sumWeight = sumLogWeight = 0;
            sumOne       = 0;
            var tileReal = new BGURef(PossibleTiles[r]);
            PossibleTiles.Clear();
            //foreach(BGURef pt in PossibleTiles) if (pt.BGUID != PossibleTiles[r].BGUID) this.Ban(pt.BGUID);
            //remove every other tile
            PossibleTiles.Add(tileReal);
            isCollapsed = true;
        }
    }
        public void CollapseTo(int i){
            var tileReal = new BGURef(PossibleTiles[i]);
            PossibleTiles.Clear();
            //foreach(BGURef pt in PossibleTiles) if (pt.BGUID != PossibleTiles[r].BGUID) this.Ban(pt.BGUID);
            //remove every other tile
            PossibleTiles.Add(tileReal);
            isCollapsed = true;
    }
    public bool IsCollapsed() => isCollapsed;
    public Vector3Int GetCoords() => new Vector3Int(x,y,z);
    public List<string> GetNeighbors(int dir){
        List<string> possibleNeighbors = new List<string>();
        foreach(BGURef pt in PossibleTiles){
            possibleNeighbors.AddRange(pt.Neighbors[dir]);
        }
        return possibleNeighbors;
    }
    public bool ValidateProbSpace(List<string> poss){
        bool changed = false;
        List<string> ct = new List<string>();
        foreach(BGURef pt in PossibleTiles){
            ct.Add(pt.BGUID);
        }
        foreach(string p in ct)
            if (!poss.Contains(p)){
                changed = true;
                Ban(p);
            }
        return changed;
    }
    public float GetEntropy(){
        if(sumOne == 0) return -1.0f;
        float entropy = Mathf.Log((float)sumWeight) - sumLogWeight/sumWeight;
        //Debug.Log("x:"+x+" y:"+y+" z:"+z + " sumOne:" + sumOne + " sumWeight:"+sumWeight + " sumLogWeight:" + sumLogWeight +" entropy:" + entropy);
        return entropy;
    }
    public bool Contains(string n){
        foreach(BGURef pt in PossibleTiles){
            if(n.Equals(pt.BGUID)) 
                return true;
        }
        return false;
    }

    public void Ban(string n){
        for(int p = 0; p < PossibleTiles.Count; p++){
            if(PossibleTiles[p].BGUID.Equals(n)){
                sumOne -= 1;
                sumWeight -= PossibleTiles[p].weight;
                sumLogWeight -= PossibleTiles[p].weight * Mathf.Log(PossibleTiles[p].weight);
                PossibleTiles.Remove(PossibleTiles[p]); 
                return; 
            }
        }
    }
}

public class AcrePrototype
{
    [SerializeField] char[] acreID = new char[2];
    int FMX, FMY, FMZ;
    int MAX_SIZE;//, STACK_POINTER;
    Stack<Vector3Int> stack3D;// = new Stack<Vector3Int>();
    //List<Vector3Int> stackHist;
    HashSet<Vector3Int> duplicateSet;// = new HashSet<Vector3Int>();
    public WaveCell[][][] wave;
    protected System.Random random;
    public List<BGURef> bguRefList;
    public AcrePrototype(List<BGURef> gridTiles, int height, int width, int length, char[] charID){
        FMX = width;
        FMY = length;
        FMZ = height;
        acreID = charID;
        bguRefList = new List<BGURef>(gridTiles);
    }
    void Init(){
        wave = new WaveCell[FMX][][];
        for(int x = 0; x < FMX; x++){
            wave[x] = new WaveCell[FMY][];
            for(int y = 0; y < FMY; y++){
                wave[x][y] = new WaveCell[FMZ];
                for(int z = 0; z < FMZ; z++){
                    var newList = new List<BGURef>(bguRefList);
                    wave[x][y][z] = new WaveCell(newList, x, y, z);
                }
            }
        }
        stack3D = new Stack<Vector3Int>();
        //stackHist = new List<Vector3Int>();
        duplicateSet = new HashSet<Vector3Int>();
        //STACK_POINTER = 0;
        MAX_SIZE = FMX * FMY * FMZ;
    }

    public void BeginCollapse(int seed){
        if(wave==null) Init();
		this.Clear();
        random = new System.Random(seed);
        int gen = 0;
        Propagate(new Vector3Int(0,0,0));
        while(gen < MAX_SIZE){
            Vector3Int nextCell = FindLowestEntropicPoint();
            //Debug.Log("Gen: " + gen +"Collapsed cell: " + nextCell);
            gen++;
            if (nextCell.x == -1 && nextCell.y == -1 && nextCell.z == -1) break;
            int i = nextCell.x * FMY * FMZ + nextCell.y * FMZ + nextCell.z;
            Debug.Log(i);
            wave[nextCell.x][nextCell.y][nextCell.z].Collapse();
            Propagate(nextCell);
		}  
    }
    Vector3Int FindLowestEntropicPoint(){
        double min = 1E+4;
        Vector3Int returnCell = new Vector3Int(-1,-1,-1);
        for (int x = 0; x < FMX; x++)
            for (int y = 0; y < FMY; y++)
                for (int z = 0; z < FMZ; z++){
                    float entropy = wave[x][y][z].GetEntropy();
                    float noise = (float)1E-6 * (float)random.NextDouble(); 
                    //Debug.Log("Entropy: "+entropy+ "Noise: "+noise);
                    if((entropy+noise) < min && entropy != -1f){
                        min = entropy+noise;
                        returnCell = wave[x][y][z].GetCoords(); 
                    }
                }
        return returnCell;
    }
    List<Vector3Int> GetNeighbors(Vector3Int index){
        List<Vector3Int> returnList = new List<Vector3Int>();
        for(int i = 0; i < 6; i++){
            int dx1 = DX[i], dy1 = DY[i], dz1 = DZ[i];
            int dx2 = index.x + dx1, dy2 = index.y + dy1, dz2 = index.z + dz1;
            if(OnBoundary(dx2, dy2, dz2)) continue; 
            returnList.Add(new Vector3Int(dx2, dy2, dz2));
        }
        return returnList;
    }

    bool ValidateProbSpace(Vector3Int index){
        bool changed = false;
        for(int i = 0; i < 6; i++){
            int dx1 = DX[i], dy1 = DY[i], dz1 = DZ[i];
            int dx2 = index.x + dx1, dy2 = index.y + dy1, dz2 = index.z + dz1;
            if(OnBoundary(dx2, dy2, dz2)) continue;
            List<string> ns = wave[dx2][dy2][dz2].GetNeighbors(i);
            bool ch = wave[index.x][index.y][index.z].ValidateProbSpace(ns);
            if(ch){
                changed = true;
            }
        }
        return changed;
    }
    void Propagate(Vector3Int index){
        List<Vector3Int> ns = GetNeighbors(index);
        for(int i = 0; i < ns.Count; i++){
            stack3D.Push(ns[i]);
        }
        while(stack3D.Count > 0){
            index = stack3D.Pop();
            WaveCell tCell = wave[index.x][index.y][index.z];
            bool changed = ValidateProbSpace(index);
            duplicateSet.Remove(index);
            if(tCell.GetPossibleTiles().Count == 1 && !tCell.IsCollapsed()){
                tCell.Collapse();
            }
            if (changed){
                ns = GetNeighbors(index);
                for(int i = 0; i < ns.Count; i++){
                    if(!wave[ns[i].x][ns[i].y][ns[i].z].IsCollapsed() && !duplicateSet.Contains(ns[i])){
                        stack3D.Push(ns[i]);
                        duplicateSet.Add(ns[i]);
                    }
                }
            }
        }    
        duplicateSet.Clear();
    }

	protected virtual void Clear()
	{
        var newList = new List<BGURef>(bguRefList);
		for (int x = 0; x < FMX; x++)
            for (int y = 0; y < FMY; y++)
                for (int z = 0; z < FMZ; z++)
                    wave[x][y][z] = new WaveCell(newList, x, y, z);
	}
    void Ban(int x, int y, int z, string n)//remove single option from possible options
    { 
        wave[x][y][z].Ban(n);
    }
    private bool OnBoundary(int x, int y, int z){
        return (x < 0 || y < 0 || z < 0 || x >= FMX || y >= FMY || z >= FMZ);
    }

    private bool IsCollapsed(){
        for(int x = 0; x < FMX; x++){
            for(int y = 0; y < FMY; y++){
                for(int z = 0; z < FMZ; z++){
                    if(!wave[x][y][z].IsCollapsed()){
                        return false;
                    }
                }
            }
        }
        return true;
    }

    protected static int[] DX = { -1, 0, 1, 0, 0, 0};
    protected static int[] DY = { 0, 1, 0, -1, 0, 0};
    protected static int[] DZ = { 0, 0, 0, 0, -1, 1};
    static int[] opposite = { 2, 3, 0, 1 , 4, 5};
}
