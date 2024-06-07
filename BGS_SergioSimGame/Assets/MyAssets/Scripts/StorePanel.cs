using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StorePanel : MonoBehaviour
{
    [SerializeField] private ShopState currentState;
    [SerializeField] private GameObject storePanel;
    [SerializeField] private Button craftTabButton;
    [SerializeField] private Button salvageTabButton;


    [Header("Craft Menu")]
    [SerializeField] private GameObject craftMenuPanel;
    [SerializeField] private Transform productsParent;
    [SerializeField] private GameObject productPrefab;
    private StoreItems selectedItemId = StoreItems.Nothing;

    [Header("Craft Menu Description Panel")]
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private TextMeshProUGUI productName;
    [SerializeField] private Image productIcon;
    [SerializeField] private TextMeshProUGUI productDescription;
    [SerializeField] private TextMeshProUGUI dustCost;
    [SerializeField] private TextMeshProUGUI cosmicCost;
    [SerializeField] private TMP_InputField quantityInputField;
    [SerializeField] private TextMeshProUGUI dustTotalCost;
    [SerializeField] private TextMeshProUGUI cosmicTotalCost;
    [SerializeField] private GameObject purchaseSuccess;
    [SerializeField] private GameObject purchaseFailed;
    [SerializeField] private GameObject craftButtonGameObj;

    [Header("Scrap Menu")]
    [SerializeField] private GameObject scrapMenuPanel;
    [Header("Inventory Materials")]
    [SerializeField] private TextMeshProUGUI currentDustText;
    [SerializeField] private TextMeshProUGUI currentCrystalsText;


    private ItemType currentItemType;
    private UnityAction CloseStoreCallback;
    private ShopData shopDataModified;
    private Dictionary<StoreItems, ShopItemData> storeMenuDictionary = new Dictionary<StoreItems, ShopItemData>();

    private void Awake()
    {
        storePanel.SetActive(false);
    }

    /// <summary>
    /// Setups the UI to show the shop specified items
    /// </summary>
    /// <param name="shopData"></param>
    public void Initialize(ShopData shopData)
    {
        shopDataModified = shopData;
        ResetValues();
        storePanel.SetActive(true);
        InitializeCraftMenu(shopData.shopCraftableItems);
        UpdateInventoryMaterialsGUI();
        ChangeState(ShopState.Crafting);
    }
    public void ChangeState(ShopState newShopState)
    {
        currentState = newShopState;
        switch(currentState)
        {
            case ShopState.Crafting:
                craftTabButton.interactable = false;
                salvageTabButton.interactable = true;
                break;
            case ShopState.Salvaging:
                craftTabButton.interactable = true;
                salvageTabButton.interactable = false;
                break;
        }
    }
    

    private void InitializeCraftMenu(List<ShopItem> shopItems)
    {
        foreach(ShopItem shopItem in shopItems)
        {
            ItemData itemData= AllItemsData.Instance.GetItemData(shopItem.items.storeItem);
            if (itemData == null) continue;
            StoreProduct storeProduct = Instantiate(productPrefab, productsParent).GetComponent<StoreProduct>();
            Material dustNeeded = shopItem.materials.Find(theMaterial => theMaterial.material == MaterialType.LunarDust);
            Material crystalsNeeded = shopItem.materials.Find(theMaterial => theMaterial.material == MaterialType.CosmicCrystals);

            string dustCost = dustNeeded != null ? dustNeeded.count.ToString() : null;
            string crystalsCost = crystalsNeeded != null ? crystalsNeeded.count.ToString() : null;
            
            ShopItemData newShopItemData = new ShopItemData();
            newShopItemData.itemData = itemData;
            newShopItemData.dustMaterialNeeded = dustNeeded != null ? dustNeeded.count : 0;
            newShopItemData.crystalsMaterialNeeded = crystalsNeeded != null ? crystalsNeeded.count : 0;
            newShopItemData.quantityLeft = shopItem.items.count;
            storeMenuDictionary.Add(itemData.storeItem, newShopItemData);

            storeProduct.Setup(itemData.storeItem, itemData.icon, itemData.name, dustCost, crystalsCost, shopItem.items.count.ToString(), this);
        }
        descriptionPanel.SetActive(false);
        purchaseSuccess.SetActive(false);
        purchaseFailed.SetActive(false);
    }


    private void UpdateInventoryMaterialsGUI()
    {
        currentDustText.text = GameManager.Instance.GetMaterialCount(MaterialType.LunarDust).ToString();
        currentCrystalsText.text = GameManager.Instance.GetMaterialCount(MaterialType.CosmicCrystals).ToString();
    }

    private void ResetValues()
    {
        foreach (Transform child in productsParent)
            Destroy(child.gameObject);
        storeMenuDictionary.Clear();
    }

    public void ItemPressed(StoreItems pressedId, StoreProduct storeProduct)
    {
        if(!descriptionPanel.activeSelf) descriptionPanel.SetActive(true);

        productName.text = storeMenuDictionary[pressedId].itemData.name;
        productIcon.sprite = storeMenuDictionary[pressedId].itemData.icon;
        productDescription.text = storeMenuDictionary[pressedId].itemData.description;
        dustCost.text = storeMenuDictionary[pressedId].dustMaterialNeeded.ToString();
        cosmicCost.text = storeMenuDictionary[pressedId].crystalsMaterialNeeded.ToString();
        quantityInputField.text = "";
        dustTotalCost.text = dustCost.text;
        cosmicTotalCost.text = cosmicCost.text;
        selectedItemId = pressedId;

        if (storeMenuDictionary[pressedId].quantityLeft == 0)
            craftButtonGameObj.SetActive(false);
        else
            craftButtonGameObj.SetActive(true);
    }
    /// <summary>
    /// Checks if enough materials are held to craft specified quantity of the item.
    /// If true, updates store item value and updates player inventory
    /// </summary>
    public void ConfirmSelectedItem()
    {
        int dustTotalCostInt = int.Parse(dustTotalCost.text);
        int crystalTotalCostInt = int.Parse(cosmicTotalCost.text);
        int quantityToModify = 1;
        if (!int.TryParse(quantityInputField.text, out quantityToModify))
            quantityToModify = 1;
        if (GameManager.Instance.GetMaterialCount(MaterialType.LunarDust) < dustTotalCostInt || GameManager.Instance.GetMaterialCount(MaterialType.CosmicCrystals) < crystalTotalCostInt)
        {
            purchaseFailed.SetActive(true);
            return;
        }
        //Else proceed with the crafting
        ShopItem shopItemToModify= shopDataModified.shopCraftableItems.Find(theItem => theItem.items.storeItem == storeMenuDictionary[selectedItemId].itemData.storeItem);
        shopItemToModify.items.count -= quantityToModify;
        GameManager.Instance.ChangeInventoryData(-dustTotalCostInt, -crystalTotalCostInt, storeMenuDictionary[selectedItemId].itemData.storeItem, quantityToModify);
        purchaseSuccess.SetActive(true);
    }

    public void OnCraftQuantityValueChanged(string quantityString)
    {
        int quantity = 0;
        if (!int.TryParse(quantityString, out quantity))
            quantity = 0;

        //Check if input quantity is much higher than available item quantity in store
        if (quantity > storeMenuDictionary[selectedItemId].quantityLeft)
        {
            quantity = storeMenuDictionary[selectedItemId].quantityLeft;
            quantityInputField.text = quantity.ToString();
        }
        else//Don't let input be 0
            if(quantity == 0)
        {
            quantity = 1;
            quantityInputField.text = quantity.ToString();
        }

        dustTotalCost.text = (quantity * storeMenuDictionary[selectedItemId].dustMaterialNeeded).ToString();
        cosmicTotalCost.text = (quantity * storeMenuDictionary[selectedItemId].crystalsMaterialNeeded).ToString();
    }
    public void ClosePanel()
    {
        GameplayManager.Instance.ShopClosing(shopDataModified);
        storePanel.SetActive(false);
    }
    public void CloseCraftSuccessWindow()
    {
        Initialize(shopDataModified);
    }
}

public enum StoreItems
{
    Nothing =0,
    //***** Upgrades
    UpgradeOxygen = 1,
    UpgradeJetpack,
    UpgradeBite,
    //***** Helmets
    HelmetDefault = 20,
    HelmetBlue,
    //***** Consumables
    Cheese = 50,
    //***** PureScrap
    AlienArtifact = 100,
}

public enum MaterialType
{
    Nothing = 0,
    //*****
    LunarDust = 1,
    CosmicCrystals
}

public enum ItemType
{
    Upgrades,
    Scrap
}

public enum ShopState
{
    Crafting,
    Salvaging
}

