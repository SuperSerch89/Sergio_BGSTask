using System.Collections.Generic;
using UnityEngine.Profiling;

[System.Serializable]
public class GameData
{
    //Intended to also save game settings
    public InventoryData inventoryData;
}

[System.Serializable]
public class InventoryData
{
    public List<Material> materials = new List<Material>();
    public List<Item> items = new List<Item>();
}

[System.Serializable]
public class Item
{
    public StoreItems storeItem;
    public int count;
}

[System.Serializable]
public class Material
{
    public MaterialType material;
    public int count;
}

[System.Serializable]
public class ShopItem
{
    public Item items = new Item();
    public List<Material> materials = new List<Material>();
}

[System.Serializable]
public class SalvagableItem
{
    public StoreItems storeItem;
    public List<Material> materials = new List<Material>();
}

[System.Serializable]
public class ShopData
{
    public List<ShopItem> shopCraftableItems = new List<ShopItem>();
    public List<SalvagableItem> shopSavagableItems = new List<SalvagableItem>();
}

[System.Serializable]
public class ShopItemData
{
    public ItemData itemData = new ItemData();
    public int dustMaterialNeeded;
    public int crystalsMaterialNeeded;
    public int quantityLeft;
}