using System.Collections;
using TMPro;
using UnityEngine;

public class EndingManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title1;
    [SerializeField] private TextMeshProUGUI title2;
    [SerializeField] private TextMeshProUGUI instructions;

    private void Start()
    {
        StartCoroutine(ShowTitle());
    }

    private IEnumerator ShowTitle()
    {
        yield return new WaitForSeconds(2f);

        title1.gameObject.SetActive(true);
        SoundManager.Instance.PlayBeep(2, true, Random.Range(0.8f, 1.2f));

        yield return new WaitForSeconds(2f);

        title2.gameObject.SetActive(true);
        SoundManager.Instance.PlayBeep(2, true, Random.Range(0.8f, 1.2f));

        yield return new WaitForSeconds(3f);

        instructions.gameObject.SetActive(true);
        SoundManager.Instance.PlayBeep(1, true, 0.8f);
    }
}
