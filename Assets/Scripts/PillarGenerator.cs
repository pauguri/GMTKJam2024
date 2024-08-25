using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PillarGenerator : MonoBehaviour
{
    [SerializeField] private GameObject pillarPrefab;

    public void PrepareBoard()
    {
        if (ThreeDSceneLogic.Instance == null) { return; }

        // generate initially blocked pillars
        Vector2Int[] blockedCells = GameManager.Instance.blockedCells.ToArray();
        // Vector2Int[] blockedCells = new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0), new Vector2Int(4, 0), new Vector2Int(5, 0), new Vector2Int(-2, 0), new Vector2Int(-3, 0), new Vector2Int(-4, 0) };
        if (blockedCells.Length > 0)
        {
            print(blockedCells.Length);
            foreach (Vector2Int position in blockedCells)
            {
                print(position.ToString());
                if (ThreeDSceneLogic.Instance.cells.ContainsKey(position))
                {
                    ThreeDSceneLogic.Instance.cells[position].blocked = true;
                    StartCoroutine(InstantiatePillar(position, false));
                }
            }
        }

        ThreeDSceneLogic.Instance.CalculateDistancesToEdge();
    }

    public void GeneratePillar(Vector2Int playerPosition)
    {
        if (ThreeDSceneLogic.Instance == null) { return; }
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

                pathCells.Add(nextCell);
                currentCell = nextCell;
            }

            if (pathCells.Count > 0)
            {
                Cell chosenCell = pathCells[Random.Range(0, pathCells.Count)];
                chosenCell.blocked = true;

                // recalculate distances to edge
                board.CalculateDistancesToEdge();

                StartCoroutine(InstantiatePillar(chosenCell.Position, true, () =>
                {
                    if (playerCell.distanceToEdge <= 0)
                    {
                        board.HandleGetSurrounded();
                    }
                }));
            }
        }
    }

    public void ResetPillars()
    {
        StopAllCoroutines();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        PrepareBoard();
    }

    private IEnumerator InstantiatePillar(Vector2Int position, bool animate = true, Action onComplete = null)
    {
        print("Generating pillar");
        GameObject pillar = Instantiate(pillarPrefab, new Vector3(position.x * 100 + (position.y % 2 == 0 ? 25 : -25), 700, position.y * 85), Quaternion.identity);
        pillar.transform.SetParent(transform);
        if (pillar.TryGetComponent(out Pillar pillarComponent))
        {
            pillarComponent.Init(animate);
        }

        if (animate && ThreeDSceneLogic.Instance.cells.ContainsKey(position))
        {
            Cell groundCell = ThreeDSceneLogic.Instance.cells[position];
            yield return groundCell.BlockedAnimation();
        }

        yield return new WaitForSeconds(3f);

        onComplete?.Invoke();
    }
}
