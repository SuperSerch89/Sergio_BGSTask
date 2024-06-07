using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreProduct : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject lunarCostParent;
    [SerializeField] private GameObject cosmicCostParent;
    [SerializeField] private TextMeshProUGUI lunarCostText;
    [SerializeField] private TextMeshProUGUI cosmicCostText;
    [SerializeField] private TextMeshProUGUI quantityText;

    [SerializeField][ReadOnlyAttribute] private StoreItems itemID;
    [SerializeField][ReadOnlyAttribute] private StorePanel itemPanel;

    public void Setup(StoreItems newItemId, Sprite sprite, string newName, string newLunarCostText, string newCosmicCostText, string newQuantityText, StorePanel newStorePanel)
    {
        itemID = newItemId;
        icon.sprite = sprite;
        nameText.text = newName;
        if(newLunarCostText == null)
            lunarCostParent.SetActive(false);
        else
        {
            lunarCostParent.SetActive(true);
            lunarCostText.text = newLunarCostText;
        }
        if (newCosmicCostText == null)
            cosmicCostParent.SetActive(false);
        else
        {
            cosmicCostParent.SetActive(true);
            cosmicCostText.text = newCosmicCostText;
        }
        quantityText.text = newQuantityText;
        gameObject.SetActive(true);
        itemPanel = newStorePanel;
    }

    public void UpdateQuantity(string newQuantity)
    {
        quantityText.text = newQuantity;
    }

    public void ItemPressed()
    {
        itemPanel.ItemPressed(itemID, this);
    }
}
