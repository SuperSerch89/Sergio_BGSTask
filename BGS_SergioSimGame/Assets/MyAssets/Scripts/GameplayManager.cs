using NicoUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class GameplayManager : LevelManager
{
    public static new GameplayManager Instance {
        get { return (GameplayManager)Singleton<LevelManager>.Instance; }
    }

    [SerializeField] private StorePanel storePanel;
    private Shop currentShop;

    public override void StartScene()
    {
        MouseController.Instance.Initialize();
    }

    /// <summary>
    /// Calls the Store panel to setup the store with the selected shop items
    /// </summary>
    /// <param name="shopData"></param>
    public void ShopOpening(Shop shop)
    {
        currentShop= shop;
        storePanel.Initialize(currentShop.CurrentShopData);
    }
    public void ShopClosing(ShopData modifiedShopData)
    {
        currentShop.UpdateShopData(modifiedShopData);
        MouseController.Instance.ChangeState(MouseControllerState.Moving);
    }
}
