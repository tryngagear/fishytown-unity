using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InvSlotUI : MonoBehaviour
{
    [SerializeField] private InvItemSlot _assSlot;
    //[SerializeField] private InvItemData _invItemData;
    [SerializeField] private Image _itemSprite;
    [SerializeField] private TextMeshProUGUI _itemCount;

    private Button button;

    public InvItemSlot AssSlot => _assSlot;
    public InventoryUI ParentDisplay{get; private set;}

    private void Awake() {
        ClearSlot();

        button = GetComponent<Button>();
        button?.onClick.AddListener(OnUISlotClick);

        ParentDisplay = transform.parent.GetComponent<InventoryUI>();
    }

    public void Init(InvItemSlot slot){
        _assSlot = slot;
        UpdateUISlot(slot);
    }

    public void UpdateUISlot(InvItemSlot slot){
        if(slot.itemData != null){
            _itemSprite.sprite = slot.itemData.sprite;
            _itemSprite.color = Color.white;
            if(slot.stackSize > 1) _itemCount.text = slot.stackSize.ToString();
            else _itemCount.text = "";
        }
        else
            ClearSlot();
    }

    public void UpdateUISlot(){
        if(_assSlot != null) UpdateUISlot(_assSlot);
    }

    public void ClearSlot(){
        _assSlot.ClearSlot();
        _itemSprite.sprite = null;
        _itemSprite.color = Color.clear;
        _itemCount.text = "";
    }

    public void OnUISlotClick(){
        ParentDisplay?.SlotClicked(this);
    }

}
