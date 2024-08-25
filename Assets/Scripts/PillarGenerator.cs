using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarGenerator : MonoBehaviour
{
    private Vector2Int[] positions;
    [SerializeField] private GameObject pillarPrefab;

    // Start is called before the first frame update
    void Start()
    {
        PrepareBoard();
    }

    public void PrepareBoard()
    {
        if (ThreeDSceneLogic.Instance == null) { return; }

        // generate initially blocked pillars
        Vector2Int[] blockedCells = GameManager.Instance.blockedCells.ToArray();
        // Vector2Int[] blockedCells = new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0), new Vector2Int(4, 0), new Vector2Int(5, 0), new Vector2Int(-2, 0), new Vector2Int(-3, 0), new Vector2Int(-4, 0) };
        if (blockedCells.Length > 0)
        {
            foreach (Vector2Int position in blockedCells)
            {
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
                StartCoroutine(InstantiatePillar(chosenCell.Position));

                // recalculate distances to edge (to see if player has been trapped)
                board.CalculateDistancesToEdge();
                if (playerCell.distanceToEdge <= 0)
                {
                    board.HandleGetSurrounded();
                }
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

    private IEnumerator InstantiatePillar(Vector2Int position, bool animate = true)
    {
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

    }
}
