using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public readonly Dictionary<Vector2Int, Cell> cells = new Dictionary<Vector2Int, Cell>();

    public virtual void Start()
    {
        Cell[] cellObjects = FindObjectsOfType<Cell>();
        // populate the dictionary with the cells
        foreach (Cell cell in cellObjects)
        {
            cells.Add(cell.Position, cell);
            //cell.board = this;
        }
    }

    public void CalculateDistancesToEdge()
    {
        // assign distance 1 to cells on the edge
        foreach (Cell cell in cells.Values)
        {
            if (!cell.blocked && (Mathf.Abs(cell.x) == 5 || Mathf.Abs(cell.y) == 5))
            {
                cell.distanceToEdge = 1;
            }
            else
            {
                cell.distanceToEdge = 0;
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
                    foreach (Cell neighbor in GetNeighbors(cell))
                    {
                        if (!neighbor.blocked && neighbor.distanceToEdge == iterations)
                        {
                            // print("cell " + cell.x + ", " + cell.y + " neighbor " + neighbor.x + ", " + neighbor.y + " has number " + neighbor.distanceToEdge);
                            cell.distanceToEdge = iterations + 1;
                            changed = true;
                            break;
                        }
                    }
                }
            }
        } while (changed);
    }

    public Cell[] GetNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();
        foreach (Vector2Int direction in GetDirections(cell.Position))
        {
            Vector2Int neighborPosition = new Vector2Int(cell.x + direction.x, cell.y + direction.y);
            if (cells.ContainsKey(neighborPosition))
            {
                neighbors.Add(cells[neighborPosition]);
            }
        }
        return neighbors.ToArray();
    }

    public Vector2Int[] GetNeighbors(Vector2Int position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        foreach (Vector2Int direction in GetDirections(position))
        {
            Vector2Int neighborPosition = new Vector2Int(position.x + direction.x, position.y + direction.y);
            neighbors.Add(neighborPosition);
        }
        return neighbors.ToArray();
    }

    public static Vector2Int[] GetDirections(Vector2Int position)
    {
        if (position.y % 2 == 0)
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
