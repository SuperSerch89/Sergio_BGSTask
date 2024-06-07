using NicoUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameDataSO startingGameData;
    [SerializeField] [ReadOnlyAttribute] private GameData gameData;

    public GameData GameData { get { return gameData; } }

    private void Start()
    {
        CreateNewGameData();
        CallStartCurrentSceneManager();
    }

    private void CallStartCurrentSceneManager()
    {
        LevelManager.Instance.StartScene();
    }
    private void CreateNewGameData()
    {
        gameData = new GameData();
        gameData = Instantiate(startingGameData).gameData;
    }
    public int GetMaterialCount(MaterialType searchedMaterial)
    {
        Material materialFound= gameData.inventoryData.materials.Find(theMaterial => theMaterial.material == searchedMaterial);
        if (materialFound != null)
            return materialFound.count;
        else return 0;
    }
    public int GetItemCount(StoreItems searchedItem)
    {
        Item itemFound = gameData.inventoryData.items.Find(theItem => theItem.storeItem == searchedItem);
        if (itemFound != null)
            return itemFound.count;
        else return 0;
    }
    public void ChangeInventoryData(int dustToAdd, int crystalsToAdd, StoreItems storeItemToChange, int storeItemQuantityToAdd)
    {
        Material dustMaterialFound = gameData.inventoryData.materials.Find(theMaterial => theMaterial.material == MaterialType.LunarDust);
        if(dustMaterialFound == null) 
        {
            dustMaterialFound = new Material();
            dustMaterialFound.material = MaterialType.LunarDust;
            gameData.inventoryData.materials.Add(dustMaterialFound);
        }
        dustMaterialFound.count += dustToAdd;

        Material crystalMaterialFound = gameData.inventoryData.materials.Find(theMaterial => theMaterial.material == MaterialType.CosmicCrystals);
        if (crystalMaterialFound == null)
        {
            crystalMaterialFound = new Material();
            crystalMaterialFound.material = MaterialType.CosmicCrystals;
            gameData.inventoryData.materials.Add(crystalMaterialFound);
        }
        crystalMaterialFound.count += crystalsToAdd;

        Item itemFound = gameData.inventoryData.items.Find(theItem => theItem.storeItem == storeItemToChange);
        if (itemFound == null)
        {
            itemFound = new Item();
            itemFound.storeItem = storeItemToChange;
            gameData.inventoryData.items.Add(itemFound);
        }

        itemFound.count += storeItemQuantityToAdd;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
