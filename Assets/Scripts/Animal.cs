using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Animal : HexGridObject
{
    private Coroutine moveCoroutine;

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

                    MoveTo(bestCells[0].Position);
                }
                else if (bestCells.Count > 1)
                {
                    // if there are multiple cells with the same score, pick one at random
                    int index = Random.Range(0, bestCells.Count);

                    currentCell.occupied = false;
                    bestCells[index].occupied = true;

                    MoveTo(bestCells[index].Position);
                }
            }
            else
            {
                if (currentCell.distanceToEdge == 1)
                {
                    // find neighbor that doesn't exist in the board
                    List<Vector2Int> unoccupiedDirections = new List<Vector2Int>();
                    foreach (Vector2Int direction in Board.GetDirections(currentCell))
                    {
                        if (!board.cells.ContainsKey(currentCell.Position + direction))
                        {
                            unoccupiedDirections.Add(direction);
                        }
                    }

                    if (unoccupiedDirections.Count > 0)
                    {
                        Vector2Int direction = unoccupiedDirections[Random.Range(0, unoccupiedDirections.Count)];
                        currentCell.occupied = false;

                        MoveTo(currentCell.Position + direction);
                    }
                    else
                    {
                        Debug.LogError("Somehow there are no unoccupied directions wat");
                    }

                    board.ResetBoard();
                }
                else if (currentCell.distanceToEdge <= 0)
                {
                    board.HandleWin();
                }
            }
        }
    }

    public void MoveTo(Vector2Int to)
    {
        SetPosition(to);
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        moveCoroutine = StartCoroutine(MoveAnimation(to));
    }

    private IEnumerator MoveAnimation(Vector2Int to)
    {
        Vector2 fromPos = transform.anchoredPosition;
        Vector2 toPos = HexToAnchored(to);
        Vector2 movement = (toPos - fromPos) / 5;

        for (int i = 0; i < 5; i++)
        {
            transform.anchoredPosition += movement;
            yield return new WaitForSeconds(0.08f);
        }
    }

    //private IEnumerator GlitchAnimation()
    //{

    //}
}
