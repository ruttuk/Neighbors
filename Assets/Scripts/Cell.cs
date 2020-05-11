using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cell
{
    public Vector2 coordinates;
    public Cell[] neighbors;
    public bool shaded;

    private GameObject cellObject;

    public Cell(int x, int y, bool shaded)
    {
        coordinates.x = x;
        coordinates.y = y;

        // north, south, east, west.
        neighbors = new Cell[4];

        this.shaded = shaded;
    }

    public void SetCellDisplay(Color color, string value)
    {
        cellObject.GetComponent<Image>().color = color;
        cellObject.GetComponentInChildren<TextMeshProUGUI>().SetText(value);
    }

    public void SetCellObject(GameObject cellObject)
    {
        this.cellObject = cellObject;
    }
}
