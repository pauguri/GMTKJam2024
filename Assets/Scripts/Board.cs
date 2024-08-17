using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public readonly Dictionary<Vector2Int, Cell> cells = new Dictionary<Vector2Int, Cell>();

    [SerializeField] private Animal animal;

    void Start()
    {
        PrepareBoard();
    }

    private void PrepareBoard()
    {
        Cell[] cellObjects = GetComponentsInChildren<Cell>();

        // populate the dictionary with the cells
        foreach (Cell cell in cellObjects)
        {
            cells.Add(new Vector2Int(cell.x, cell.y), cell);
            cell.board = this;
        }

        // select random cells to block
        for (int i = 0; i < 15; i++)
        {
            Cell cell;
            do
            {
                int index = Random.Range(0, cellObjects.Length);
                cell = cellObjects[index];
            } while (cell.blocked || (cell.x == 0 && cell.y == 0));
            cell.SetBlocked(true);
        }
    }

    public void CalculatePathfinding()
    {
        // assign distance 1 to cells on the edge
        foreach (Cell cell in cells.Values)
        {
            if (!cell.blocked && (Mathf.Abs(cell.x) == 5 || Mathf.Abs(cell.y) == 5))
            {
                cell.distanceToEdge = 1;
                //cell.possibleRoutes = 2;
            }
            else
            {
                cell.distanceToEdge = 0;
                //cell.possibleRoutes = 0;
            }
        }

        // assign increasing distance to the rest of the cells
        bool changed = false;
        int iterations = 0;
        do
        {
            changed = false;
            iterations++;

            foreach (Cell cell in cells.Values)
            {
                if (cell.distanceToEdge == 0 && !cell.blocked)
                {
                    //int accPossibleRoutes = 0;
                    foreach (Cell neighbor in GetNeighbors(cell))
                    {
                        if (!neighbor.blocked && neighbor.distanceToEdge == iterations)
                        {
                            // print("cell " + cell.x + ", " + cell.y + " neighbor " + neighbor.x + ", " + neighbor.y + " has number " + neighbor.distanceToEdge);
                            cell.distanceToEdge = iterations + 1;
                            changed = true;
                            break;

                            //accPossibleRoutes += neighbor.possibleRoutes;
                        }
                    }

                    //if (accPossibleRoutes > 0)
                    //{
                    //cell.distanceToEdge = iterations + 1;
                    //cell.possibleRoutes = accPossibleRoutes;
                    //changed = true;
                    //}
                }
            }
        } while (changed);

        animal.CalculateNextMove(this);
    }

    public Cell[] GetNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();
        foreach (Vector2Int direction in GetDirections(cell))
        {
            Vector2Int position = new Vector2Int(cell.x + direction.x, cell.y + direction.y);
            if (cells.ContainsKey(position))
            {
                neighbors.Add(cells[position]);
            }
        }

        return neighbors.ToArray();
    }

    public static Vector2Int[] GetDirections(Cell cell)
    {
        if (cell.y % 2 == 0)
        {
            return new Vector2Int[]
            {
                new Vector2Int(-1, 0),
                new Vector2Int(0, 1),
                new Vector2Int(1, 1),
                new Vector2Int(1, 0),
                new Vector2Int(1, -1),
                new Vector2Int(0, -1)
            };
        }
        else
        {
            return new Vector2Int[]
            {
                new Vector2Int(-1, 0),
                new Vector2Int(-1, 1),
                new Vector2Int(0, 1),
                new Vector2Int(1, 0),
                new Vector2Int(0, -1),
                new Vector2Int(-1, -1)
            };
        }
    }

}
