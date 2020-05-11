using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CellButton : MonoBehaviour
{
    CustomGrid m_Grid;
    Button m_Button;
    Vector2 m_Coordinate;

    void Start()
    {
        m_Grid = FindObjectOfType<CustomGrid>();
        m_Button = GetComponent<Button>();
        m_Button.onClick.AddListener(delegate { m_Grid.SetPlayerFromClick(m_Coordinate); });
    }

    public void SetCoordinate(int x, int y)
    {
        m_Coordinate.x = x;
        m_Coordinate.y = y;
    }
}
