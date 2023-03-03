using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItem", menuName = "ScriptableObjects/InventoryItem", order = 1)]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public bool isStackable;
    public int stackSize;
    public bool placeable;

    private const int stackLimit = 15;
    public bool IncrementStack(){
        if(isStackable && stackSize < stackLimit){
            stackSize++;
            return true;
        }
        return false;
    } 
    public bool DecrementStack(){
        if(isStackable && stackSize > 0){
            stackSize--;
            return true;
        }
        return false;
    } 
}
