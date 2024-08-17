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
        if (GameManager.Instance == null || GameManager.Instance.clickedCells.Count == 0)
        {
            Debug.LogError("No cells clicked");
            return;
        }

        // generate initially blocked cells
        Vector2Int[] blockedCells = GameManager.Instance.blockedCells.ToArray();
        if (blockedCells.Length > 0)
        {
            foreach (Vector2Int position in blockedCells)
            {
                GeneratePillar(position);
            }
        }

        positions = GameManager.Instance.clickedCells.ToArray();
        StartCoroutine(GeneratePillars());
    }

    private IEnumerator GeneratePillars()
    {
        do
        {
            yield return new WaitForSeconds(3);

            Vector2Int position = positions[0];
            positions = positions.Skip(1).ToArray();

            GeneratePillar(position);

        } while (positions.Length > 0);
    }

    private void GeneratePillar(Vector2Int position)
    {
        Instantiate(pillarPrefab, new Vector3(position.x * 100 + (position.y % 2 == 0 ? 25 : -25), 0, position.y * 85), Quaternion.identity);
    }
}
