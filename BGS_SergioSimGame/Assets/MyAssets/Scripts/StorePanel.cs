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
    [SerializeField] private GameObject descriptionScrapPanel;
    [SerializeField] private Transform savagableItemsParent;
    [SerializeField] private TMP_InputField quantityInputFieldSalvage;
    [SerializeField] private TextMeshProUGUI dustToSalvage;
    [SerializeField] private TextMeshProUGUI cosmicToSalvage;
    [SerializeField] private TextMeshProUGUI dustTotalSalvage;
    [SerializeField] private TextMeshProUGUI cosmicTotalSalvage;
    [SerializeField] private GameObject salvageSuccess;
    [SerializeField] private GameObject salvageFailed;
    [SerializeField] private GameObject salvageButtonGameObj;
    private StoreItems selectedItemIdToSavage = StoreItems.Nothing;

    [Header("Inventory Materials")]
    [SerializeField] private TextMeshProUGUI currentDustText;
    [SerializeField] private TextMeshProUGUI currentCrystalsText;


    private ItemType currentItemType;
    private UnityAction CloseStoreCallback;
    private ShopData shopDataModified;
    private Dictionary<StoreItems, ShopItemData> storeMenuDictionary = new Dictionary<StoreItems, ShopItemData>();
    private Dictionary<StoreItems, ShopItemData> storeMenuDictionarySalvagable = new Dictionary<StoreItems, ShopItemData>();

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
        InitializeScrapMenu(shopData.shopSavagableItems);
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
                craftMenuPanel.SetActive(true);
                scrapMenuPanel.SetActive(false);
                break;
            case ShopState.Salvaging:
                craftTabButton.interactable = true;
                salvageTabButton.interactable = false;
                craftMenuPanel.SetActive(false);
                scrapMenuPanel.SetActive(true);
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
    private void InitializeScrapMenu(List<SalvagableItem> salvagableItems)
    {
        foreach (SalvagableItem salvagableItem in salvagableItems)
        {
            ItemData itemData = AllItemsData.Instance.GetItemData(salvagableItem.storeItem);
            if (itemData == null) continue;
            StoreProduct storeProduct = Instantiate(productPrefab, savagableItemsParent).GetComponent<StoreProduct>();
            Material dustSalvagable = salvagableItem.materials.Find(theMaterial => theMaterial.material == MaterialType.LunarDust);
            Material crystalsSalvagable = salvagableItem.materials.Find(theMaterial => theMaterial.material == MaterialType.CosmicCrystals);

            string dustQuantity = dustSalvagable != null ? dustSalvagable.count.ToString() : null;
            string crystalsQuantity = crystalsSalvagable != null ? crystalsSalvagable.count.ToString() : null;

            ShopItemData newShopItemData = new ShopItemData();
            newShopItemData.itemData = itemData;
            newShopItemData.dustMaterialNeeded = dustSalvagable != null ? dustSalvagable.count : 0;
            newShopItemData.crystalsMaterialNeeded = crystalsSalvagable != null ? crystalsSalvagable.count : 0;
            newShopItemData.quantityLeft = GameManager.Instance.GetItemCount(itemData.storeItem);//Getting the players inventory quantity of this salvagable item
            storeMenuDictionarySalvagable.Add(itemData.storeItem, newShopItemData);

            storeProduct.Setup(itemData.storeItem, itemData.icon, itemData.name, dustQuantity, crystalsQuantity, newShopItemData.quantityLeft.ToString(), this);
        }
        descriptionScrapPanel.SetActive(false);
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
        foreach (Transform child in savagableItemsParent)
            Destroy(child.gameObject);
        storeMenuDictionarySalvagable.Clear();
    }

    public void ItemPressed(StoreItems pressedId, StoreProduct storeProduct)
    {
        if (currentState == ShopState.Salvaging)
        {
            ItemPressedSalvagable(pressedId, storeProduct);
            return;
        }

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
    public void ItemPressedSalvagable(StoreItems pressedId, StoreProduct storeProduct)
    {
        if (!descriptionScrapPanel.activeSelf) descriptionScrapPanel.SetActive(true);

        dustToSalvage.text = storeMenuDictionarySalvagable[pressedId].dustMaterialNeeded.ToString();
        cosmicToSalvage.text = storeMenuDictionarySalvagable[pressedId].crystalsMaterialNeeded.ToString();
        quantityInputField.text = "";
        dustTotalSalvage.text = dustToSalvage.text;
        cosmicTotalSalvage.text = cosmicToSalvage.text;
        selectedItemIdToSavage = pressedId;

        if (storeMenuDictionarySalvagable[pressedId].quantityLeft == 0)
            salvageButtonGameObj.SetActive(false);
        else
            salvageButtonGameObj.SetActive(true);
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
    public void ConfirmSelectedScrapItem()
    {
        int dustTotalSalvageInt = int.Parse(dustTotalSalvage.text);
        int crystalTotalSalvageInt = int.Parse(cosmicTotalSalvage.text);
        int quantityToModify = 1;
        if (!int.TryParse(quantityInputFieldSalvage.text, out quantityToModify))
            quantityToModify = 1;
        //if (GameManager.Instance.GetMaterialCount(MaterialType.LunarDust) < dustTotalSalvageInt || GameManager.Instance.GetMaterialCount(MaterialType.CosmicCrystals) < crystalTotalSalvageInt)
        //{
        //    purchaseFailed.SetActive(true);
        //    return;
        //}
        //Else proceed with the salvaging
        GameManager.Instance.ChangeInventoryData(dustTotalSalvageInt, crystalTotalSalvageInt, storeMenuDictionarySalvagable[selectedItemIdToSavage].itemData.storeItem, -quantityToModify);
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
    public void OnScrapQuantityValueChanged(string quantityString)
    {
        int quantity = 0;
        if (!int.TryParse(quantityString, out quantity))
            quantity = 0;

        //Check if input quantity is much higher than available item quantity in store
        if (quantity > storeMenuDictionarySalvagable[selectedItemIdToSavage].quantityLeft)
        {
            quantity = storeMenuDictionarySalvagable[selectedItemIdToSavage].quantityLeft;
            quantityInputFieldSalvage.text = quantity.ToString();
        }
        else//Don't let input be 0
            if (quantity == 0)
        {
            quantity = 1;
            quantityInputFieldSalvage.text = quantity.ToString();
        }

        dustTotalSalvage.text = (quantity * storeMenuDictionarySalvagable[selectedItemIdToSavage].dustMaterialNeeded).ToString();
        cosmicTotalSalvage.text = (quantity * storeMenuDictionarySalvagable[selectedItemIdToSavage].crystalsMaterialNeeded).ToString();
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
    public void OpenCraftMenu()
    {
        ChangeState(ShopState.Crafting);
    }
    public void OpenScrapMenu()
    {
        ChangeState(ShopState.Salvaging);
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

