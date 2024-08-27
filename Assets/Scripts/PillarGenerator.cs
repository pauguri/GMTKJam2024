using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PillarGenerator : MonoBehaviour
{
    [SerializeField] private GameObject pillarPrefab;
    private bool playerIsSurrounded = false;

    public void PrepareBoard()
    {
        if (ThreeDSceneLogic.Instance == null) { return; }

        // generate initially blocked pillars
        if (GameManager.Instance != null && GameManager.Instance.blockedCells.Count > 0)
        {
            Vector2Int[] blockedCells = GameManager.Instance.blockedCells.ToArray();
            // Vector2Int[] blockedCells = new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0), new Vector2Int(4, 0), new Vector2Int(5, 0), new Vector2Int(-2, 0), new Vector2Int(-3, 0), new Vector2Int(-4, 0) };
            if (blockedCells.Length > 0)
            {
                foreach (Vector2Int position in blockedCells)
                {
                    if (ThreeDSceneLogic.Instance.cells.ContainsKey(position))
                    {
                        Cell cell = ThreeDSceneLogic.Instance.cells[position];
                        cell.blocked = true;
                        StartCoroutine(InstantiatePillar(cell, false, false));
                    }
                }
            }
        }

        ThreeDSceneLogic.Instance.CalculateDistancesToEdge();
    }

    public void GeneratePillar(Vector2Int playerPosition)
    {
        if (ThreeDSceneLogic.Instance == null || playerIsSurrounded) { return; }
        ThreeDSceneLogic board = ThreeDSceneLogic.Instance;

        if (board.cells.ContainsKey(playerPosition))
        {
            Cell playerCell = board.cells[playerPosition];

            // calculate fastest path to edge
            Cell currentCell = playerCell;
            List<Cell> pathCells = new List<Cell>();

            while (currentCell.distanceToEdge > 1)
            {
                Cell nextCell = board.CalculateNextMove(currentCell);
                if (nextCell.Position == currentCell.Position) { break; }

                print(nextCell.Position.ToString() + " -> " + nextCell.distanceToEdge);
                pathCells.Add(nextCell);
                currentCell = nextCell;
            }

            if (pathCells.Count > 0)
            {
                Cell chosenCell = pathCells[Random.Range(0, pathCells.Count)];
                chosenCell.blocked = true;
                StartCoroutine(InstantiatePillar(chosenCell));
                print("-> " + chosenCell.Position.ToString());
            }
            else
            {
                // if no path is found, player is either surrounded or on the edge
                // choose a random neighbor to block
                Cell[] neighbors = board.GetNeighbors(playerCell);
                neighbors = Array.FindAll(neighbors, n => !n.blocked);
                if (neighbors.Length > 0)
                {
                    Cell chosenCell = neighbors[Random.Range(0, neighbors.Length)];
                    chosenCell.blocked = true;
                    StartCoroutine(InstantiatePillar(chosenCell));
                    print("-> " + chosenCell.Position.ToString());
                }
            }

            board.CalculateDistancesToEdge();
        }
    }

    public void ResetPillars()
    {
        StopAllCoroutines();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        playerIsSurrounded = false;

        PrepareBoard();
    }

    private IEnumerator InstantiatePillar(Cell cell, bool animate = true, bool checkSurrounded = true)
    {
        GameObject pillar = Instantiate(pillarPrefab, new Vector3(cell.x * 100 + (cell.y % 2 == 0 ? 25 : -25), 700, cell.y * 85), Quaternion.identity);
        pillar.transform.SetParent(transform);
        if (pillar.TryGetComponent(out Pillar pillarComponent))
        {
            pillarComponent.Init(animate);
        }

        if (animate)
        {
            yield return cell.BlockedAnimation();
        }

        if (checkSurrounded && ThreeDSceneLogic.Instance.IsPlayerSurrounded())
        {
            playerIsSurrounded = true;
            yield return new WaitForSeconds(3f);
            ThreeDSceneLogic.Instance.HandleGetSurrounded();
        }
    }
}
