using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeProperty : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

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
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (GameManager.instance.IsInRange(GameManager.instance.GetStepTime()))
        {
            //Debug.Log("ChangeColor to color one");
            lineRenderer.material.SetColor(propertyName, secondColorToChangeTo);
            isColorTwoChosen = false;
        }
        else
        {
            //Debug.Log("ChangeColor to color two");
            lineRenderer.material.SetColor(propertyName, colorToChangeTo);
            isColorTwoChosen = true;
        }
    }
}