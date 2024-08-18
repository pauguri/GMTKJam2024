using System.Collections;
using TMPro;
using UnityEngine;

public class MirroredIntroManager : MonoBehaviour
{
    private bool buttonPressed = false;
    private bool buttonEnabled = false;

    [SerializeField] private TextMeshProUGUI title1;
    [SerializeField] private TextMeshProUGUI title2;
    [SerializeField] private TextMeshProUGUI instructions;

    private void Start()
    {
        buttonEnabled = false;
        // SoundManager.Instance.ToggleBeepReverb(true);
        StartCoroutine(ShowTitle());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && buttonEnabled && !buttonPressed)
        {
            buttonPressed = true;
            SoundManager.Instance.PlayBeep(0);
            GameManager.Instance.Start3DPhase();
        }
    }

    private IEnumerator ShowTitle()
    {
        yield return new WaitForSeconds(2f);

        title1.gameObject.SetActive(true);
        SoundManager.Instance.PlayBeep(2, true, Random.Range(0.2f, 0.25f));

        yield return new WaitForSeconds(2f);

        title2.gameObject.SetActive(true);
        SoundManager.Instance.PlayBeep(2, true, Random.Range(0.2f, 0.25f));
        buttonEnabled = true;

        yield return new WaitForSeconds(3f);

        instructions.gameObject.SetActive(true);
        SoundManager.Instance.PlayBeep(1, true, 0.2f);
    }
}
