using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ThreeDSceneLogic : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PillarGenerator pillarGenerator;
    [SerializeField] private GameObject deadOverlay;
    [SerializeField] private TextMeshProUGUI deadText;
    [SerializeField] private PauseMenu pauseMenu;
    private bool isDead = false;

    [SerializeField] private GameObject movementHint;
    private Coroutine movementHintCoroutine;
    private bool movementHintShown = false;

    public readonly Dictionary<Vector2Int, GroundCell> groundCells = new Dictionary<Vector2Int, GroundCell>();

    public static ThreeDSceneLogic Instance;

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

    private void Start()
    {
        GroundCell[] groundCellObjects = FindObjectsOfType<GroundCell>();
        foreach (GroundCell cell in groundCellObjects)
        {
            groundCells.Add(new Vector2Int(cell.x, cell.y), cell);
        }

        if (!movementHintShown)
        {
            movementHintCoroutine = StartCoroutine(ShowMovementHint());
        }
    }

    private IEnumerator ShowMovementHint()
    {
        yield return new WaitForSeconds(5f);

        movementHintShown = true;
        movementHint.SetActive(true);
    }

    public void EndTutorialTime()
    {
        if (movementHintCoroutine != null)
        {
            StopCoroutine(movementHintCoroutine);
        }
        movementHint.SetActive(false);

        pillarGenerator.BeginPillarGeneration();
    }

    private void Update()
    {
        if (isDead)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isDead = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                SoundManager.Instance.PlayBeep(0, false, 1, true);
                GameManager.Instance.Start2DPhase();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                SoundManager.Instance.PlayBeep(0);
                HideDeathScreen();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Player entered trigger " + name);

            // handle player winning
            playerController.inputActive = false;
            pauseMenu.canPause = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SoundManager.Instance.PlayBeep(4);
            GameManager.Instance.StartEnding();
        }
    }

    public void HandleGetCrushed()
    {
        if (isDead)
        {
            return;
        }
        isDead = true;

        deadText.text = "<color=#2861FF>you</color> got crushed";
        SoundManager.Instance.PlayBeep(5);
        ShowDeathScreen();
    }

    public void HandleGetSurrounded()
    {
        if (isDead)
        {
            return;
        }
        isDead = true;

        deadText.text = "<color=#2861FF>you</color> got trapped";
        SoundManager.Instance.PlayLoseSound();
        ShowDeathScreen();
    }

    private void ShowDeathScreen()
    {
        playerController.inputActive = false;
        pauseMenu.canPause = false;
        deadOverlay.SetActive(true);
        pillarGenerator.ResetPillars();
    }

    private void HideDeathScreen()
    {
        playerController.ResetPosition();
        playerController.inputActive = true;
        pauseMenu.canPause = true;
        pillarGenerator.BeginPillarGeneration();
        isDead = false;
        deadOverlay.SetActive(false);
    }

    public void CheckPlayerTrapped()
    {
        // check what cell the player is closest to
        Vector2Int closestPosition = Vector2Int.zero;
        Vector3 closestWorldPos = Vector3.positiveInfinity;

        foreach (GroundCell cell in groundCells.Values)
        {
            Vector2 cellAnchoredPos = HexGridObject.HexToAnchored(cell.Position);
            Vector3 cellWorldPos = new Vector3(cellAnchoredPos.x, 0, cellAnchoredPos.y);

            if (Vector3.Distance(cellWorldPos, playerController.transform.position) < Vector3.Distance(closestWorldPos, playerController.transform.position))
            {
                closestPosition = cell.Position;
                closestWorldPos = cellWorldPos;
            }
        }

        // check if the closest cell is surrounded
        if (GameManager.Instance.surroundedCells.Contains(closestPosition))
        {
            HandleGetSurrounded();
        }
    }
}
