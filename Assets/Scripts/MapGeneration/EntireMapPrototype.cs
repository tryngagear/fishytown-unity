using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
public class EntireMapPrototype : MonoBehaviour 
{
    [SerializeField] public AcrePrototype[] acres = new AcrePrototype[1];
    public List<BGURef> bguRefList;
    private const int l = 4, w = 16, h = 16; //individual acre dimensions
    private const int mL = 1, mH = 1; //entire map dimensions
    private readonly char[,] AcreIDList = {{'a','1'}};
    [SerializeField] public MapGenJsonReader MGJR;
    [SerializeField] public Dictionary<string, string> objectMap = new Dictionary<string, string>();
    [SerializeField] public GameObject MissDictError, NoTileError;
    public int seed = 0;
    private void Start() {
        GenerateAcre();
        MGJR.ReadJson();
    }
    private void init(){
        if(seed == 0)
            seed = (int)System.DateTime.Now.Ticks;
    }

    [ContextMenu("Generate Acre")]
    private void GenerateAcre(){
        init();
        bguRefList = MGJR.bguRefList.list;
        foreach(var kvp in MGJR.itemDict.list) objectMap[kvp.Key] = kvp.Val;
        for(int i = 0; i < acres.Length; i++){
            char[] AcreID = {(char)i, 'a'}; 
            acres[i] = new AcrePrototype(bguRefList, h, w, l, AcreID);
            acres[i].BeginCollapse(0);
            PlaceAcre(acres[i], AcreID);
        }
    }

    private void PlaceAcre(AcrePrototype acre, char[] AcreID){
        var acreParent = new GameObject();
        acreParent.name = "a1";
        for(int x = 0; x < w; x++){
            for(int y = 0; y < l; y++){
                GameObject gameObject = null;
                for(int z = 0; z < h; z++){
                    if(acre.wave[x][y][z].IsCollapsed()){
                        if(objectMap.ContainsKey(acre.wave[x][y][z].PossibleTiles[0].meshRef)){
                            if(objectMap[acre.wave[x][y][z].PossibleTiles[0].meshRef] == string.Empty) continue;
                            gameObject =  (GameObject)AssetDatabase.LoadAssetAtPath(objectMap[acre.wave[x][y][z].PossibleTiles[0].meshRef], typeof(GameObject));
                        }else gameObject = MissDictError;
                    }else gameObject = NoTileError;
                        var go = Instantiate(gameObject, new Vector3(x,y,z), gameObject.transform.rotation);
                        go.name = "AcreID:"+acreParent.name+"_X:"+x.ToString()+"Y:"+ y.ToString()+"Z:"+z.ToString() + gameObject.name;
                        go.transform.parent = acreParent.transform;
                }
            }
        }
    }
}
