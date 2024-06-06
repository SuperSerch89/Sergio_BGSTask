using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    [SerializeField] private InteractionType interactionType;
    public InteractionType InteractionType { get { return interactionType; } }

    private int skin=1;
    public MouseHelmet helmet;
    public void Perform()
    {
        Debug.Log("Opening shop");
        if(skin == 1)
            helmet.SwitchHelmet(HelmetTypes.Orange);
        else
            helmet.SwitchHelmet(HelmetTypes.Purple);
        skin *= -1;
    }
}
