using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public readonly Dictionary<Vector2Int, Cell> cells = new Dictionary<Vector2Int, Cell>();
    [NonSerialized] public bool enableInput = true;

    [SerializeField] private Animal animal;
    [SerializeField] private int[] blockedCells = new int[] { 10, 15, 20 };

    [Space]
    [SerializeField] private ShaderEffect_CorruptedVram glitchEffect;

    void Start()
    {
        // hide cell rows and animal
        foreach (CanvasGroup row in GetComponentsInChildren<CanvasGroup>())
        {
            row.alpha = 0;
        }
        animal.gameObject.SetActive(false);
        enableInput = false;

        Cell[] cellObjects = GetComponentsInChildren<Cell>();
        // populate the dictionary with the cells
        foreach (Cell cell in cellObjects)
        {
            cells.Add(new Vector2Int(cell.x, cell.y), cell);
            cell.board = this;
        }

        PrepareBoard();
        StartCoroutine(AnimateInBoard(1f, () => enableInput = true));
    }

    private void PrepareBoard()
    {
        // clear GameManager lists
        GameManager.Instance.blockedCells.Clear();
        GameManager.Instance.clickedCells.Clear();
        GameManager.Instance.surroundedCells.Clear();

        // make 0,0 cell occupied
        cells[new Vector2Int(0, 0)].occupied = true;

        // select random cells to block
        int blockedCountIndex = Mathf.Min(GameManager.Instance.round, blockedCells.Length - 1);

        for (int i = 0; i < blockedCells[blockedCountIndex]; i++)
        {
            Cell cell;
            do
            {
                int index = Random.Range(0, cells.Values.Count);
                cell = cells.Values.ElementAt(index);
            } while (cell.blocked || (cell.x == 0 && cell.y == 0));
            cell.SetBlocked(true, false);
            GameManager.Instance.blockedCells.Add(cell.Position);
        }
    }

    public void CalculatePathfinding()
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

        // find surrounded cells
        foreach (Cell cell in cells.Values)
        {
            if (cell.distanceToEdge == 0 && !cell.blocked)
            {
                GameManager.Instance.surroundedCells.Add(cell.Position);
            }
        }

        animal.CalculateNextMove(this);
    }

    public void ResetBoard()
    {
        enableInput = false;

        StartCoroutine(AnimateOutBoard(1f, () =>
        {
            foreach (Cell cell in cells.Values)
            {
                cell.SetBlocked(false, false);
                cell.occupied = false;
            }
            GameManager.Instance.round++;
            PrepareBoard();
            StartCoroutine(AnimateInBoard(1f, () => enableInput = true));
        }));
    }


    private IEnumerator AnimateInBoard(float delay = 0f, Action callback = null)
    {
        yield return new WaitForSeconds(delay);

        CanvasGroup[] rows = GetComponentsInChildren<CanvasGroup>();
        int middleRow = Mathf.CeilToInt(rows.Length / 2);

        for (int i = 0; i < rows.Length; i++)
        {
            rows[i].alpha = 1;

            if (i == middleRow)
            {
                animal.gameObject.SetActive(true);
                animal.SetPosition(0, 0);
                animal.transform.anchoredPosition = HexGridObject.HexToAnchored(0, 0);
            }

            yield return new WaitForSeconds(0.08f);
        }

        callback?.Invoke();
    }

    private IEnumerator AnimateOutBoard(float delay = 0f, Action callback = null)
    {
        yield return new WaitForSeconds(delay);

        SoundManager.Instance.PlayLoseSound();
        animal.gameObject.SetActive(false);
        foreach (CanvasGroup row in GetComponentsInChildren<CanvasGroup>())
        {
            row.alpha = 0;

            yield return new WaitForSeconds(0.08f);
        }

        callback?.Invoke();
    }
    public void HandleWin()
    {
        enableInput = false;

        StartCoroutine(GlitchOutBoard(1f));
    }

    private IEnumerator GlitchOutBoard(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        SoundManager.Instance.PlayBeep(4);

        yield return new WaitForSeconds(2f);

        SoundManager.Instance.PlayGlitchSound();

        yield return new WaitForSeconds(1f);

        glitchEffect.enabled = true;

        float timeElapsed = 0f;
        glitchEffect.shift = 0;

        while (glitchEffect.shift < 5)
        {
            glitchEffect.shift = Mathf.Lerp(0, 5, timeElapsed / 2f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        SoundManager.Instance.PlayBeep(0);
        SoundManager.Instance.StopGlitchSound();
        GameManager.Instance.Start3DIntro();
    }

    public Cell[] GetNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();
        foreach (Vector2Int direction in GetDirections(cell))
        {
            Vector2Int position = new Vector2Int(cell.x + direction.x, cell.y + direction.y);
            if (cells.ContainsKey(position))
            {
                neighbors.Add(cells[position]);
            }
        }

        return neighbors.ToArray();
    }

    public static Vector2Int[] GetDirections(Cell cell)
    {
        if (cell.y % 2 == 0)
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
