using System.Collections;
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
    }

    public void GeneratePillar(Vector2Int playerPosition)
    {
        if (ThreeDSceneLogic.Instance == null) { return; }
        ThreeDSceneLogic board = ThreeDSceneLogic.Instance;

        // minmax stuff
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
