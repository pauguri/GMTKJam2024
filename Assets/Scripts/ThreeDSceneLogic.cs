using System.Collections;
using UnityEngine;

public class ThreeDSceneLogic : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PillarGenerator pillarGenerator;
    [SerializeField] private GameObject crushedOverlay;
    private bool isBeingCrushed = false;

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
        if (isBeingCrushed)
        {
            return;
        }
        isBeingCrushed = true;

        StartCoroutine(CrushedSequence());
    }
    private IEnumerator CrushedSequence()
    {
        playerController.inputActive = false;
        crushedOverlay.SetActive(true);
        pillarGenerator.ResetPillars();
        SoundManager.Instance.PlayBeep(5);
        //SoundManager.Instance.PlayBeep(4, true, 0.1f);

        // GameObject portableBeep = Instantiate(SoundManager.Instance.portableBeepPrefab, playerController.transform.position, Quaternion.identity);
        // PortableBeep beep = portableBeep.GetComponent<PortableBeep>();
        // beep.PlayBeep(5);

        yield return new WaitForSeconds(2f);

        // Destroy(portableBeep);
        SoundManager.Instance.PlayBeep(0, true, 0.4f);
        playerController.ResetPosition();
        playerController.inputActive = true;
        crushedOverlay.SetActive(false);
        pillarGenerator.BeginPillarGeneration();
        isBeingCrushed = false;
    }
}
