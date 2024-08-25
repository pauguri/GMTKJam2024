using System.Collections;
using TMPro;
using UnityEngine;

public class ThreeDSceneLogic : Board
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

    public override void Start()
    {
        base.Start();

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
    }

    public void HandlePlayerChangeCell(Vector2Int newPosition)
    {
        if (Mathf.Abs(newPosition.x) > 5 || Mathf.Abs(newPosition.y) > 5)
        {
            HandleWin();
            return;
        }

        pillarGenerator.GeneratePillar(newPosition);
        print("player moved to " + newPosition);
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

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        print("Player entered trigger " + name);

    //        // handle player winning
    //        playerController.inputActive = false;
    //        pauseMenu.canPause = false;
    //        Cursor.lockState = CursorLockMode.None;
    //        Cursor.visible = true;
    //        SoundManager.Instance.PlayBeep(4);
    //        GameManager.Instance.StartEnding();
    //    }
    //}

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
        playerController.ResetPosition(-60f);
        playerController.inputActive = true;
        pauseMenu.canPause = true;
        isDead = false;
        deadOverlay.SetActive(false);
    }

    public void HandleWin()
    {
        playerController.inputActive = false;
        pauseMenu.canPause = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SoundManager.Instance.PlayBeep(4);
        GameManager.Instance.StartEnding();
    }
}
