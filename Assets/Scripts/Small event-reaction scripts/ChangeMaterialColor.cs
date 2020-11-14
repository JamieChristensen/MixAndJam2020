using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialColor : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer meshRenderer;

    [SerializeField]
    private string propertyName = "_BaseColor";

    [SerializeField]
    [ColorUsage(true, true)]
    private Color colorToChangeTo;


    [Tooltip("Only used if changeBetweenTwoColors is true")]
    [SerializeField]
    [ColorUsage(true, true)]
    private Color secondColorToChangeTo;


    public bool changeBetweenTwoColors = false;
    private bool isColorTwoChosen = false;

    public void ChangeColor()
    {
        if (!changeBetweenTwoColors)
        {
            meshRenderer.material.SetColor("_BaseColor", colorToChangeTo);
            return;
        }

        if (isColorTwoChosen)
        {
            meshRenderer.material.SetColor("_BaseColor", colorToChangeTo);
            isColorTwoChosen = false;
        }
        else
        {
            meshRenderer.material.SetColor("_BaseColor", secondColorToChangeTo);
            isColorTwoChosen = true;
        }
    }
}