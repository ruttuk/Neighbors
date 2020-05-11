using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomGrid : MonoBehaviour
{
    [SerializeField]
    private CellButton cellTextObject;

    // Approximately what percentage of the grid will be shaded cells.
    [SerializeField]
    [Range(0f, 0.5f)]
    private float shadedCellDensity = 0.15f;

    private Cell[,] cells;
    private Cell playerCell;

    private const int maxMovementPointsPerTurn = 6;
    private const int gridSize = 12;
    private const float cellSize = 100f;

    private Color emptyCellColor = Color.white;
    private string emptyCellText = "-";

    private Color shadedCellColor = Color.gray;
    private string shadedCellText = "*";

    private Color playerCellColor = Color.cyan;
    private string playerCellText = "X";

    private Color validCellColor = Color.green;

    // On any given turn, this will store the valid paths from current player position.
    private Dictionary<Vector2, Path> validPaths = new Dictionary<Vector2, Path>();

    void Awake()
    {
        InitializeCells();
    }

    void Start()
    {
        DrawCells();
        SetStartingPlayerPosition();
        DisplayPossibleMoves();
    }

    /** Set the player cell to the cell coordinate (called from CellButton on click) **/
    public void SetPlayerFromClick(Vector2 cellCoordinate)
    {
        if (validPaths.ContainsKey(cellCoordinate))
        {
            validPaths.Clear();
            ResetCells();

            playerCell = cells[(int)cellCoordinate.x, (int)cellCoordinate.y];
            playerCell.SetCellDisplay(playerCellColor, playerCellText);

            DisplayPossibleMoves();
        }
        else
        {
            Debug.Log("Invalid Cell!");
        }
    }

    /** Initialize each cell to be randomly shaded. Set the neighbors for each cell. **/
    private void InitializeCells()
    {
        bool shaded;
        cells = new Cell[gridSize, gridSize];
        System.Random prng = new System.Random();

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                shaded = prng.Next(0, 99999) % Mathf.RoundToInt(1 / shadedCellDensity) == 0;
                cells[i, j] = new Cell(i, j, shaded);
            }
        }

        // set neighbors after we've created all cells.
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                SetNeighbors(i, j);
            }
        }

    }

    /** Instantiate the gameboard. **/
    private void DrawCells()
    {
        Vector3 pos = Vector3.zero;

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                pos.x =  cellSize + i * cellSize;
                pos.y =  cellSize + j * cellSize;

                GameObject spawnedCell = Instantiate(cellTextObject.gameObject, pos, Quaternion.identity, transform);

                // Give each cell a reference to its in-game object (button).
                cells[i, j].SetCellObject(spawnedCell);

                // Display the contents of the cell.
                if(cells[i, j].shaded)
                {
                    cells[i, j].SetCellDisplay(shadedCellColor, shadedCellText);
                }
                else
                {
                    cells[i, j].SetCellDisplay(emptyCellColor, emptyCellText);
                }
                // Set each cell button coordinate.
                spawnedCell.GetComponent<CellButton>().SetCoordinate(i, j);
            }
        }
    }

    /** Determine the neighbors for each cell. If the cell is on the edge of the grid, an invalid neighbor will be set to null. **/
    private void SetNeighbors(int x, int y)
    {
        Cell north, west, east, south;

        north = west = east = south = null;

        // north will be at location [i + 1, j]
        if (x + 1 < gridSize)
        {
            north = cells[x + 1, y];
        }

        // south will be at location [i - 1, j]
        if (x - 1 >= 0)
        {
            south = cells[x - 1, y];
        }

        // east will be at location [i, j + 1]
        if (y + 1 < gridSize)
        {
            east = cells[x, y + 1];
        }

        // west will be at location [i, j - 1]
        if (y - 1 >= 0)
        {
            west = cells[x, y - 1];
        }

        cells[x, y].neighbors[0] = north;
        cells[x, y].neighbors[1] = south;
        cells[x, y].neighbors[2] = east;
        cells[x, y].neighbors[3] = west;
    }

    /** Set starting position of player. Start at the middle. **/
    private void SetStartingPlayerPosition()
    {
        int x, y;

        x = y = gridSize / 2;

        if(!cells[x, y].shaded)
        {
            playerCell = cells[x, y];
            playerCell.SetCellDisplay(playerCellColor, playerCellText);
        }
        else
        {
            for(int i = x; i < gridSize; i++)
            {
                for(int j = y; j < gridSize; j++)
                {
                    if(!cells[i, j].shaded)
                    {
                        playerCell = cells[i, j];
                        playerCell.SetCellDisplay(playerCellColor, playerCellText);
                        i = j = gridSize;
                    }
                }
            }
        }
    }

    /** Resets all cells to display original values.**/
    private void ResetCells()
    {
        for(int i = 0; i < gridSize; i++)
        {
            for(int j = 0; j < gridSize; j++)
            {
                // Display the contents of the cell.
                if (cells[i, j].shaded)
                {
                    cells[i, j].SetCellDisplay(shadedCellColor, shadedCellText);
                }
                else
                {
                    cells[i, j].SetCellDisplay(emptyCellColor, emptyCellText);
                }
            }
        }
    }

    /** Show all possible moves and the smallest possible cost to get there on any given turn. **/
    private void DisplayPossibleMoves()
    {
        int startingCost = 0;
        Vector2[] startingMoves = new Vector2[0];
        Vector2 currentMove = playerCell.coordinates;
        Path startingPath = new Path(startingMoves, currentMove, startingCost);

        Traverse(playerCell, startingPath, maxMovementPointsPerTurn);
    }

    /** Recursively traverse the cells starting from the current cell. **/
    private void Traverse(Cell currentCell, Path currentPath, int movementPoints)
    {
        if(movementPoints <= 0)
        {
            // we've reached the end...
            // check if this cell is already added in the dictionary.
            // if it is, then we can't do any better since we've already used the max movement points.
            // otherwise, add to the dictionary
            if (!validPaths.ContainsKey(currentCell.coordinates))
            {
                validPaths.Add(currentCell.coordinates, currentPath);
                currentCell.SetCellDisplay(validCellColor, currentPath.cost.ToString());
            }
        }
        else
        {
            // we still have movement points remaining!
            // ----------------------------------------
            // let's see if the current cell is already added in the dictionary...
            if(validPaths.ContainsKey(currentCell.coordinates))
            {
                // if so, then we will replace it ONLY if the current movement cost is less than the one currently added.
                if(validPaths[currentCell.coordinates].cost > currentPath.cost)
                {
                    validPaths[currentCell.coordinates] = currentPath;
                    currentCell.SetCellDisplay(validCellColor, currentPath.cost.ToString());
                }
            } // don't add the starting cell.
            else if(currentPath.cost > 0)
            {
                validPaths.Add(currentCell.coordinates, currentPath);
                currentCell.SetCellDisplay(validCellColor, currentPath.cost.ToString());
            }
            // continue on traversing.
            for (int i = 0; i < 4; i++)
            {
                // we can only traverse a neighbor if it meets 3 conditions:
                // 1. it is not null. 
                // 2. it is not shaded.
                // 3. it is not already in our current path. (i.e. do not allow backtracking)
                if(currentCell.neighbors[i] != null && !currentCell.neighbors[i].shaded && !currentPath.ContainsMove(currentCell.neighbors[i].coordinates))
                {
                    // each time we traverse another cell, we create a new path.
                    Traverse(currentCell.neighbors[i], new Path(currentPath.moves, currentCell.coordinates, currentPath.cost + 1), movementPoints - 1);
                }
            }
        }
    }
}
