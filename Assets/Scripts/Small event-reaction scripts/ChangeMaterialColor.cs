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

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void ChangeColor()
    {

        if (!changeBetweenTwoColors)
        {
            meshRenderer.material.SetColor(propertyName, colorToChangeTo);
            return;
        }

        if (isColorTwoChosen)
        {
            //Debug.Log("ChangeColor to color one");
            meshRenderer.material.SetColor(propertyName, colorToChangeTo);
            isColorTwoChosen = false;
        }
        else
        {
            //Debug.Log("ChangeColor to color two");
            meshRenderer.material.SetColor(propertyName, secondColorToChangeTo);
            isColorTwoChosen = true;
        }
    }
}