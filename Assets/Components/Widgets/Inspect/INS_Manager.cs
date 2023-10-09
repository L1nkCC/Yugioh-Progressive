using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class INS_Manager : MonoBehaviour
{
    INS_Image image;
    INS_Description description;
    INS_StarSpawner starSpawner;
    INS_StatLabel statLabel;
    INS_ArrowManager arrowManager;
    INS_PendulumScales scales;
    INS_Outline outline;
    INS_Attribute attribute;

    private void Awake()
    {
        image = GetComponentInChildren<INS_Image>();
        description = GetComponentInChildren<INS_Description>();
        starSpawner = GetComponentInChildren<INS_StarSpawner>();
        statLabel = GetComponentInChildren<INS_StatLabel>();
        arrowManager = GetComponentInChildren<INS_ArrowManager>();
        scales = GetComponentInChildren<INS_PendulumScales>();
        outline = GetComponentInChildren<INS_Outline>();
        attribute = GetComponentInChildren<INS_Attribute>();
    }


    public void DisplayCard(Card card, bool standardLayout = true)
    {
        if (!standardLayout)
        {
            image.SetImage(card);
            description.SetDescription(card);
        }
        else
        {
            image.SetImage(card, API_TextureHandler.CardImageOption.cropped);
            description.SetDescription(card);
            starSpawner.SetStars(card);
            statLabel.SetAttackAndDefense(card);
            arrowManager.SetArrows(card);
            scales.SetScales(card);
            outline.SetOutlineColor(card);
            attribute.SetAttribute(card);
        }
    }
}
