using System.Collections.Generic;
using UnityEngine;

public class Animal : HexGridObject
{
    void Start()
    {
        SetPosition(0, 0);
    }

    void Update()
    {

    }

    public void CalculateNextMove(Board board)
    {
        if (board.cells.ContainsKey(Position))
        {
            Cell currentCell = board.cells[Position];
            if (currentCell.distanceToEdge > 1)
            {
                // find the cell with the lowest distance to the edge
                int lowestDistance = int.MaxValue;
                List<Cell> bestCells = new List<Cell>();

                foreach (Cell neighbor in board.GetNeighbors(currentCell))
                {
                    if (neighbor.distanceToEdge <= 0 || neighbor.blocked)
                    {
                        continue;
                    }

                    if (neighbor.distanceToEdge < lowestDistance)
                    {
                        bestCells.Clear();
                        lowestDistance = neighbor.distanceToEdge;
                        bestCells.Add(neighbor);
                    }
                    else if (neighbor.distanceToEdge == lowestDistance)
                    {
                        bestCells.Add(neighbor);
                    }
                }

                if (bestCells.Count == 1)
                {
                    currentCell.occupied = false;
                    bestCells[0].occupied = true;
                    HopTo(bestCells[0].Position);
                }
                else if (bestCells.Count > 1)
                {
                    // if there are multiple cells with the same score, pick one at random
                    int index = Random.Range(0, bestCells.Count);

                    currentCell.occupied = false;
                    bestCells[index].occupied = true;
                    HopTo(bestCells[index].Position);
                }
            }
            else
            {
                if (currentCell.distanceToEdge == 1)
                {
                    // we are on the edge, ESCAPE
                    print("Escaped!");
                }
                else if (currentCell.distanceToEdge <= 0)
                {
                    print("Trapped!");
                }
            }
        }
    }

    public void HopTo(Vector2Int position)
    {
        SetPosition(position);
    }
}
