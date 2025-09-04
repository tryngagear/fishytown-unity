using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class MapGridCompiler : MonoBehaviour
{
    [SerializeField] public BaseGridUnit[] protoTypeList;
    [SerializeField] private string FileOutputName;
    [SerializeField] private string DictFileName;
    [SerializeField] private ListThatIsAClass<BGURef> refList;
    [SerializeField] private ListThatIsAClass<KVPair<string, string>> dictRef;
    [SerializeField] private List<BaseGridUnitVirtual> typeData;
    [SerializeField] private RotationalAxis rotationalAxis;
    //BaseGridUnitVirtual AemptyBGU;
    //BaseGridUnitVirtual BemptyBGU;

    private void Start() {
        
    }

    [ContextMenu("Generate Json Prototypes")]
    public void GeneratePrototypes(){
        typeData.Clear();
        refList.list.Clear();
        dictRef.list.Clear();

        BaseGridUnitVirtual AemptyBGU = new BaseGridUnitVirtual("EmptyA", "NAN", -1, 0.01f, MapGenUtils.SelfSocket("NAN_A"));
        BaseGridUnitVirtual BemptyBGU = new BaseGridUnitVirtual("EmptyB", "NAN", -1, 0.01f, MapGenUtils.SelfSocket("NAN_B"));
        dictRef.list.Add(new KVPair<string, string>("NAN", string.Empty));
        typeData.Add(AemptyBGU);
        typeData.Add(BemptyBGU);
        foreach(BaseGridUnit BGU in protoTypeList){
            foreach(int i in BGU.rotationList){
                string newBGUID = BGU.BGUID + "_" + i;
                string newMeshRef = BGU.meshRef;
                int newRotID = i;
                float newWeight = BGU.weight;
                Sockets newSockets = new Sockets(BGU.sockets); 
                newSockets = MapGenUtils.RotateSockets(newSockets, i, rotationalAxis);
                BaseGridUnitVirtual newBGU = new BaseGridUnitVirtual(newBGUID, newMeshRef, newRotID, newWeight, newSockets);
                typeData.Add(newBGU);
            }

            KVPair<string, string> kvp = new KVPair<string,string>(BGU.meshRef,AssetDatabase.GetAssetPath(BGU.refObj)); 
            dictRef.list.Add(kvp);           
        }
        for(int i = 0; i < typeData.Count; i++){
            for(int o = i; o < typeData.Count; o++){
                CheckAdjacency( typeData[i], typeData[o], AemptyBGU, BemptyBGU);
            }
        }
        //refList.list.Add(new BGURef(AemptyBGU));
        //refList.list.Add(new BGURef(BemptyBGU));
        foreach(var td in typeData){
            refList.list.Add(new BGURef(td));
        }
        Debug.Log(Application.persistentDataPath);
        System.IO.File.WriteAllText(Application.persistentDataPath + FileOutputName, JsonUtility.ToJson(refList, true));
        System.IO.File.WriteAllText(Application.persistentDataPath + DictFileName, JsonUtility.ToJson(dictRef, true));
    }

    public void CheckAdjacency( BaseGridUnitVirtual A, BaseGridUnitVirtual B, BaseGridUnitVirtual Aempty, BaseGridUnitVirtual Bempty){
        //check adjacency in X
        bool rotX = (rotationalAxis != RotationalAxis.X || A.rotationID == B.rotationID) || A.rotationID == -1 ? true:false;
        bool rotY = (rotationalAxis != RotationalAxis.Y || A.rotationID == B.rotationID) || A.rotationID == -1 ? true:false;
        bool rotZ = (rotationalAxis != RotationalAxis.Z || A.rotationID == B.rotationID) || A.rotationID == -1 ? true:false;

        if(rotX){
            if(GridSymetric(A.sockets.posX, B.sockets.negX)){
                A.sockets.posX.validNeighbors.Add(B.BGUID); 
                B.sockets.negX.validNeighbors.Add(A.BGUID); 
            }else if(A.sockets.posX.FID == "NAN_A"){A.sockets.posX.validNeighbors.Add("EmptyA");Aempty.sockets.negX.validNeighbors.Add(A.BGUID);}
            else if (A.sockets.posX.FID == "NAN_B"){A.sockets.posX.validNeighbors.Add("EmptyB");Bempty.sockets.negX.validNeighbors.Add(A.BGUID);}
            if(GridSymetric(A.sockets.negX, B.sockets.posX)){
                A.sockets.negX.validNeighbors.Add(B.BGUID); 
                B.sockets.posX.validNeighbors.Add(A.BGUID);
            }else if(A.sockets.negX.FID == "NAN_A"){A.sockets.negX.validNeighbors.Add("EmptyA");Aempty.sockets.posX.validNeighbors.Add(A.BGUID);}
            else if (A.sockets.negX.FID == "NAN_B"){A.sockets.negX.validNeighbors.Add("EmptyB");Bempty.sockets.posX.validNeighbors.Add(A.BGUID);}
        }
        //check adjacency in Y
        if(rotY){
            if(GridSymetric(A.sockets.posY, B.sockets.negY)){
                A.sockets.posY.validNeighbors.Add(B.BGUID); 
                B.sockets.negY.validNeighbors.Add(A.BGUID); 
            }else if(A.sockets.posY.FID == "NAN_A"){A.sockets.posY.validNeighbors.Add("EmptyA");Aempty.sockets.negY.validNeighbors.Add(A.BGUID);}
            else if (A.sockets.posY.FID == "NAN_B"){A.sockets.posY.validNeighbors.Add("EmptyB");Bempty.sockets.negY.validNeighbors.Add(A.BGUID);}
            if(GridSymetric(A.sockets.negY, B.sockets.posY)){
                A.sockets.negY.validNeighbors.Add(B.BGUID); 
                B.sockets.posY.validNeighbors.Add(A.BGUID); 
            }else if(A.sockets.negY.FID == "NAN_A"){A.sockets.negY.validNeighbors.Add("EmptyA");Aempty.sockets.posY.validNeighbors.Add(A.BGUID);}
            else if (A.sockets.negY.FID == "NAN_B"){A.sockets.negY.validNeighbors.Add("EmptyB");Bempty.sockets.posY.validNeighbors.Add(A.BGUID);}
        }
        //check adjacency in Z
        if(rotZ){
            if(GridSymetric(A.sockets.posZ,B.sockets.negZ)){
                A.sockets.posZ.validNeighbors.Add(B.BGUID);
                B.sockets.negZ.validNeighbors.Add(A.BGUID); 
            }else if(A.sockets.posZ.FID == "NAN_A"){A.sockets.posZ.validNeighbors.Add("EmptyA");Aempty.sockets.negZ.validNeighbors.Add(A.BGUID);}
            else if (A.sockets.posZ.FID == "NAN_B"){A.sockets.posZ.validNeighbors.Add("EmptyB");Bempty.sockets.negZ.validNeighbors.Add(A.BGUID);}
            if(GridSymetric(A.sockets.negZ, B.sockets.posZ)){
                A.sockets.negZ.validNeighbors.Add(B.BGUID); 
                B.sockets.posZ.validNeighbors.Add(A.BGUID); 
            }else if(A.sockets.negZ.FID == "NAN_A"){A.sockets.negZ.validNeighbors.Add("EmptyA");Aempty.sockets.posZ.validNeighbors.Add(A.BGUID);}
            else if (A.sockets.negZ.FID == "NAN_B"){A.sockets.negZ.validNeighbors.Add("EmptyB");Bempty.sockets.posZ.validNeighbors.Add(A.BGUID);}
        }
    }

    public bool GridSymetric(face A, face B){
        if(A.FID == "NAN_A" || A.FID == "NAN_B")
            return false;
        if(A.sym == false && B.sym == false){
        if(A.FID == B.FID) return (A.polarity != B.polarity) ? true:false;
        }
        return A.FID == B.FID ? true:false;
    }

}
