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
    private bool isDead = false;

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

        // TODO: wait until player moves to begin generating pillars
        pillarGenerator.BeginPillarGeneration();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Player entered trigger " + name);

            // handle player winning
            playerController.inputActive = false;
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
        StartCoroutine(DeathSequence());
        SoundManager.Instance.PlayBeep(5);
    }

    public void HandleGetSurrounded()
    {
        if (isDead)
        {
            return;
        }
        isDead = true;

        deadText.text = "<color=#2861FF>you</color> got trapped";
        StartCoroutine(DeathSequence());
        SoundManager.Instance.PlayLoseSound();
    }

    private IEnumerator DeathSequence()
    {
        playerController.inputActive = false;
        deadOverlay.SetActive(true);
        pillarGenerator.ResetPillars();

        //SoundManager.Instance.PlayBeep(4, true, 0.1f);

        // GameObject portableBeep = Instantiate(SoundManager.Instance.portableBeepPrefab, playerController.transform.position, Quaternion.identity);
        // PortableBeep beep = portableBeep.GetComponent<PortableBeep>();
        // beep.PlayBeep(5);

        yield return new WaitForSeconds(2f);

        // Destroy(portableBeep);
        SoundManager.Instance.PlayBeep(0);
        playerController.ResetPosition();
        playerController.inputActive = true;
        deadOverlay.SetActive(false);
        pillarGenerator.BeginPillarGeneration();
        isDead = false;
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
