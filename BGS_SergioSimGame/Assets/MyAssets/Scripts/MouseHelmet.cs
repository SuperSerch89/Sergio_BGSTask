using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class MouseHelmet : MonoBehaviour
{
    [SerializeField] private SpriteResolver spriteResolver;

    public void SwitchHelmet(HelmetTypes helmetType)
    {
        if (spriteResolver != null)
        {
            spriteResolver.SetCategoryAndLabel("Helmets", helmetType.ToString());
        }
    }
}

public enum HelmetTypes
{
    Orange,
    Purple,
    Pink,
    Sky
}