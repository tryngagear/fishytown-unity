using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItem", menuName = "ScriptableObjects/InventoryItem", order = 1)]
public class InvItemData : ScriptableObject
{
    public int IID;
    public string itemName;
    [TextArea(4, 4)]
    public string itemDesc;
    public Sprite sprite;
    public int maxSize;
}
