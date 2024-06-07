using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelmetInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private InteractionType interactionType;
    [SerializeField] private HelmetTypes helmetType;
    [SerializeField] private SpriteRenderer helmetRenderer;

    private Helmets helmetsManager;
    public InteractionType InteractionType { get { return interactionType; } }


    public void Setup(HelmetTypes newHelmetType, Sprite sprite, Helmets newHelmetsManager)
    {
        helmetType = newHelmetType;
        helmetRenderer.sprite = sprite;
        helmetsManager = newHelmetsManager;
    }

    public void Perform()
    {
        helmetsManager.HelmetInteracted(helmetType);
    }
}
