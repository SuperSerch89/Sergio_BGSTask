using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class StorePanel : MonoBehaviour
{
    [SerializeField] private GameObject storePanel;
    [Header("Craft Menu")]
    [SerializeField] private GameObject craftMenuPanel;
    [SerializeField] private Transform productsParent;
    [SerializeField] private GameObject productPrefab;
    [SerializeField] private GameObject purchaseSuccess;
    [SerializeField] private GameObject purchaseFailed;
    [SerializeField] private TextMeshProUGUI currentDustText;
    [SerializeField] private TextMeshProUGUI currentCrystalsText;
    private StoreItems selectedItemId = StoreItems.Nothing;
    [Header("Scrap Menu")]
    [SerializeField] private GameObject scrapMenuPanel;

    private ItemType currentItemType;
    private UnityAction CloseStoreCallback;

    private void Start()
    {
        storePanel.SetActive(false);
    }

    public void Activate(ItemType itemType, UnityAction onCloseStore)
    {
        CloseStoreCallback = null;
        CloseStoreCallback += onCloseStore;
        storePanel.SetActive(true);
        UpdateMaterialsGUI();
        OpenStore(itemType);
    }

    private void UpdateMaterialsGUI()
    {
        
    }

    public void OpenStore(ItemType itemType)
    {
        currentItemType = itemType;
        ResetValues();
    }
    private void ResetValues()
    {
        
    }

    public void ItemPressed(StoreItems pressedId, StoreProduct storeProduct)
    {

    }
}

public enum StoreItems
{
    Nothing =0,
    //***** Upgrades
    UpgradeOxygen,
    UpgradeJetpack,
    UpgradeBite,
    //***** Helmets
    HelmetDefault = 20,
    HelmetBlue = 20,
}

public enum ItemType
{
    Upgrades,
    Scrap
}