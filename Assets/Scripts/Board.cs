using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private readonly Dictionary<Vector2Int, Cell> cells = new Dictionary<Vector2Int, Cell>();

    void Start()
    {
        PrepareBoard();
    }

    void Update()
    {

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

        // select 5 random cells to block
        for (int i = 0; i < 5; i++)
        {
            Cell cell;
            do
            {
                int index = Random.Range(0, cellObjects.Length);
                cell = cellObjects[index];
            } while (cell.blocked || (cell.x == 0 && cell.y == 0));
            cell.SetBlocked(true);
        }

        // assign distance 1 to cells on the edge
        foreach (Cell cell in cellObjects)
        {
            if (Mathf.Abs(cell.x) == 5 || Mathf.Abs(cell.y) == 5)
            {
                cell.distanceToEdge = 1;
            }
        }

        // assign increasing distance to the rest of the cells
        //bool changed = false;
        //do
        //{
        //    changed = false;
        //    foreach (Cell cell in cellObjects)
        //    {
        //        if (cell.distanceToEdge == 0)
        //        {
        //            int minDistance = 6;
        //            foreach (Cell neighbor in GetNeighbors(cell))
        //            {
        //                if (neighbor.distanceToEdge < minDistance)
        //                {
        //                    minDistance = neighbor.distanceToEdge;
        //                }
        //            }
        //            if (minDistance < 6)
        //            {
        //                cell.distanceToEdge = minDistance + 1;
        //                changed = true;
        //            }
        //        }
        //    }
        //} while (changed);
    }
}
