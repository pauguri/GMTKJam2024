using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TwoDSceneLogic : Board
{
    [NonSerialized] public bool enableInput = true;

    [SerializeField] private GameObject board;
    [SerializeField] private Animal animal;
    [SerializeField] private int[] blockedCells = new int[] { 10, 15, 20 };

    [Space]
    [SerializeField] private ShaderEffect_CorruptedVram glitchEffect;
    [SerializeField] private PauseMenu pauseMenu;

    public static TwoDSceneLogic Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void Start()
    {
        // hide cell rows and animal
        foreach (CanvasGroup row in board.GetComponentsInChildren<CanvasGroup>())
        {
            row.alpha = 0;
        }
        animal.gameObject.SetActive(false);
        enableInput = false;
        pauseMenu.canPause = false;

        base.Start();

        PrepareBoard();
        StartCoroutine(AnimateInBoard(1f, () =>
        {
            enableInput = true;
            pauseMenu.canPause = true;
        }));
    }

    private void PrepareBoard()
    {
        // make 0,0 cell occupied
        cells[new Vector2Int(0, 0)].occupied = true;

        // select random cells to block
        GameManager.Instance.blockedCells.Clear();
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

    public void HandleAnimalTurn()
    {
        if (cells.ContainsKey(animal.Position))
        {
            CalculateDistancesToEdge();

            Cell currentCell = cells[animal.Position];
            // animal is on the edge and is about to leave
            if (currentCell.distanceToEdge == 1)
            {
                // find neighbor that doesn't exist in the board
                List<Vector2Int> unoccupiedDirections = new List<Vector2Int>();
                foreach (Vector2Int direction in GetDirections(currentCell.Position))
                {
                    if (!cells.ContainsKey(currentCell.Position + direction))
                    {
                        unoccupiedDirections.Add(direction);
                    }
                }

                if (unoccupiedDirections.Count > 0)
                {
                    Vector2Int direction = unoccupiedDirections[Random.Range(0, unoccupiedDirections.Count)];
                    currentCell.occupied = false;

                    animal.MoveTo(currentCell.Position + direction);
                }
                else
                {
                    Debug.LogError("Somehow there are no unoccupied directions wat");
                }

                ResetBoard();
            }
            // animal is trapped
            else if (currentCell.distanceToEdge <= 0)
            {
                HandleWin();
            }
            else
            {
                Vector2Int nextPosition = CalculateNextMove(currentCell).Position;
                animal.MoveTo(nextPosition);
            }
        }
    }

    public void ResetBoard()
    {
        enableInput = false;
        pauseMenu.canPause = false;

        StartCoroutine(AnimateOutBoard(1f, () =>
        {
            foreach (Cell cell in cells.Values)
            {
                cell.SetBlocked(false, false);
                cell.occupied = false;
            }
            GameManager.Instance.round++;
            PrepareBoard();
            StartCoroutine(AnimateInBoard(1f, () =>
            {
                enableInput = true;
                pauseMenu.canPause = true;
            }));
        }));
    }


    private IEnumerator AnimateInBoard(float delay = 0f, Action callback = null)
    {
        yield return new WaitForSeconds(delay);

        CanvasGroup[] rows = board.GetComponentsInChildren<CanvasGroup>();
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
        foreach (CanvasGroup row in board.GetComponentsInChildren<CanvasGroup>())
        {
            row.alpha = 0;

            yield return new WaitForSeconds(0.08f);
        }

        callback?.Invoke();
    }
    public void HandleWin()
    {
        enableInput = false;
        pauseMenu.canPause = false;

        StartCoroutine(GlitchOutBoard(1f));
    }

    private IEnumerator GlitchOutBoard(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        SoundManager.Instance.PlayBeep(4);

        yield return new WaitForSeconds(2f);

        SoundManager.Instance.PlayGlitchSound(2f);

        glitchEffect.enabled = true;

        float timeElapsed = 0f;
        glitchEffect.shift = 0;

        while (glitchEffect.shift < 5)
        {
            glitchEffect.shift = Mathf.Lerp(0, 5, timeElapsed / 3f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        SoundManager.Instance.PlayBeep(0, false, 1, true);
        SoundManager.Instance.StopGlitchSound();
        GameManager.Instance.Start3DIntro();
    }
}
