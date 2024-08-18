using System.Collections;
using System.Linq;
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
                StartCoroutine(GeneratePillar(position, false));
            }
        }
    }

    public void BeginPillarGeneration()
    {
        positions = GameManager.Instance.clickedCells.ToArray();
        // positions = new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(1, 1), new Vector2Int(2, 1), new Vector2Int(3, 1), new Vector2Int(4, 1), new Vector2Int(5, 1), new Vector2Int(-2, 1), new Vector2Int(-3, 1), new Vector2Int(-4, 1) };
        if (positions.Length == 0)
        {
            Debug.LogError("No positions to generate pillars");
            return;
        }

        StartCoroutine(GeneratePillars());
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

    private IEnumerator GeneratePillars()
    {
        do
        {
            yield return new WaitForSeconds(5f);

            Vector2Int position = positions[0];
            positions = positions.Skip(1).ToArray();

            StartCoroutine(GeneratePillar(position));

        } while (positions.Length > 0);

        yield return new WaitForSeconds(3f);

        ThreeDSceneLogic.Instance.CheckPlayerTrapped();
    }

    private IEnumerator GeneratePillar(Vector2Int position, bool animate = true)
    {
        GameObject pillar = Instantiate(pillarPrefab, new Vector3(position.x * 100 + (position.y % 2 == 0 ? 25 : -25), 700, position.y * 85), Quaternion.identity);
        pillar.transform.SetParent(transform);
        if (pillar.TryGetComponent(out Pillar pillarComponent))
        {
            pillarComponent.Init(animate);
        }

        if (animate && ThreeDSceneLogic.Instance.groundCells.ContainsKey(position))
        {
            GroundCell groundCell = ThreeDSceneLogic.Instance.groundCells[position];
            yield return groundCell.BlockedAnimation();
        }

    }
}
