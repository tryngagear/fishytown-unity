using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class MapGenJsonReader : MonoBehaviour
{
    public TextAsset JsonMapText, JsonDictList;
    public ListThatIsAClass<BGURef> bguRefList =  new ListThatIsAClass<BGURef>();
    public ListThatIsAClass<KVPair<string, string>> itemDict;
    //public Dictionary<string, string> itemDict = new Dictionary<string, string>();

     [ContextMenu("ReadJson")]
    public void ReadJson(){
        bguRefList = JsonUtility.FromJson<ListThatIsAClass<BGURef>>(JsonMapText.text);
        //ListThatIsAClass<KVPair<string, string>> meshList = JsonUtility.FromJson<ListThatIsAClass<KVPair<string,string>>>(JsonDictList.text);
        itemDict = JsonUtility.FromJson<ListThatIsAClass<KVPair<string,string>>>(JsonDictList.text);
        //foreach(KVPair<string, string> kvp in meshList.list){
            //itemDict[kvp.Key] = kvp.Val;//(GameObject)AssetDatabase.LoadAssetAtPath(kvp.Val, typeof(GameObject));
        //    Debug.Log("K: "+kvp.Key+"V: "+kvp.Val);
            //Instantiate((GameObject)AssetDatabase.LoadAssetAtPath(kvp.Val, typeof(GameObject)),new Vector3(0,0,0), Quaternion.identity);
        //}
    } 
}
