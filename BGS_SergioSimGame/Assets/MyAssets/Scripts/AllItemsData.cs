using NicoUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllItemsData : Singleton<AllItemsData>
{
    [SerializeField] private List<ItemData> items = new List<ItemData>();
    
    public ItemData GetItemData(StoreItems itemSearched)
    {
        ItemData itemFound= items.Find(item=> item.storeItem == itemSearched);
        if(itemFound == null) 
        {
            Debug.LogError($"Item {itemSearched} not found inside all itemsData list.");
        }
        return itemFound;
    }
}

[System.Serializable]
public class ItemData
{
    public StoreItems storeItem;
    public string name;
    public Sprite icon;
    public int maxPossibleHeld;
    public string description;
}