using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    [SerializeField] private InteractionType interactionType;
    [SerializeField] private ShopDataSO startingShopData;
    [SerializeField] [ReadOnlyAttribute] private ShopData currentShopData;

    public InteractionType InteractionType { get { return interactionType; } }
    public ShopData CurrentShopData {  get { return currentShopData; } }



    //private int skin=1;
    //public MouseHelmet helmet;

    private void Awake()
    {
        CreateNewShopData();
    }

    private void CreateNewShopData()
    {
        currentShopData = new ShopData();
        currentShopData = Instantiate(startingShopData).shopData;
    }
    public void Perform()
    {
        //Debug.Log("Opening shop");
        //if(skin == 1)
        //    helmet.SwitchHelmet(HelmetTypes.Orange);
        //else
        //    helmet.SwitchHelmet(HelmetTypes.Purple);
        //skin *= -1;
        GameplayManager.Instance.ShopOpening(this);
    }
    public void UpdateShopData(ShopData modifiedShopData)
    {
        currentShopData = modifiedShopData;
    }
}
