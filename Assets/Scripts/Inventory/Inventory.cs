using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "ScriptableObjects/Inventory", order = 1)]
public class Inventory : ScriptableObject
{
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();
    public int maxItems = 40;

    public void AddItem(InventoryItem item){
        var indices = Enumerable.Range(0, inventoryItems.Count)
        .Where(i => inventoryItems[i].itemName == item.itemName);
        if(indices != null){
            foreach(int index in indices){
                bool cont = inventoryItems[index].IncrementStack();
                if(cont){ return;}
            }
        }
        inventoryItems.Add(item);
    }

    public void RemoveItem(InventoryItem item){
        var indices = Enumerable.Range(0, inventoryItems.Count)
        .Where(i => inventoryItems[i].itemName == item.itemName);
        if(indices != null){
            foreach(int index in indices){
                bool cont = inventoryItems[index].DecrementStack();
                if(cont){ return;}
            }
        inventoryItems.Remove(item);
        }
    }

}
