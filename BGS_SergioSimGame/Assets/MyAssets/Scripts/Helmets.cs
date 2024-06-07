using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmets : MonoBehaviour
{
    [SerializeField] private Transform helmetPositionsParent;
    [SerializeField][ReadOnlyAttribute] private List<Transform> helmetPositions = new List<Transform>();
    [SerializeField] private GameObject helmetPrefab;
    [SerializeField] List<HelmetItem> possibleHelmets = new List<HelmetItem>();

    //private int skin=1;
    //public MouseHelmet helmet;

    private void Start()
    {
        foreach (Transform child in helmetPositionsParent)
            helmetPositions.Add(child);
        InitializeHelmets();
    }

    public void InitializeHelmets()
    {
        int positionCount = 0;
        foreach(HelmetItem helmetItem in possibleHelmets)
        {
            int ownedHelmets= GameManager.Instance.GetItemCount(helmetItem.item);
            if (ownedHelmets > 0)
            {
                if (helmetPositions.Count <= positionCount) 
                {
                    Debug.LogWarning("Not enough posiitons to place helmets.");
                    return; 
                }
                HelmetInteractable newHelmet= Instantiate(helmetPrefab, helmetPositions[positionCount]).GetComponent<HelmetInteractable>();
                newHelmet.Setup(helmetItem.type, AllItemsData.Instance.GetItemData(helmetItem.item).icon, this);
                positionCount++;
            }
        }
    }

    public void HelmetInteracted(HelmetTypes helmetToEquip)
    {
        MouseController.Instance.EquipHelmet(helmetToEquip);
    }
}

[System.Serializable]
public class HelmetItem
{
    public HelmetTypes type;
    public StoreItems item;
}