using System.Collections;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    [SerializeField] private GameObject crushTrigger;
    [SerializeField] private float spawnDuration = 3.5f;

    public void Init(bool animate)
    {
        if (!animate)
        {
            transform.localScale = new Vector3(1, Random.Range(0.8f, 1.1f), 1);
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            return;
        }

        StartCoroutine(SpawnAnimation());
    }

    private IEnumerator SpawnAnimation()
    {
        transform.position = new Vector3(transform.position.x, 700, transform.position.z);
        transform.localScale = new Vector3(1, 0, 1);
        crushTrigger.SetActive(true);

        float finalScale = Random.Range(0.8f, 1.1f);
        float timeElapsed = 0f;
        while (transform.position.y > 0)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(700, 0, timeElapsed / spawnDuration), transform.position.z);
            transform.localScale = new Vector3(1, Mathf.Lerp(0, finalScale, timeElapsed / spawnDuration), 1);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        crushTrigger.SetActive(false);
        GameObject portableBeep = Instantiate(SoundManager.Instance.portableBeepPrefab, transform.position, Quaternion.identity);
        PortableBeep beep = portableBeep.GetComponent<PortableBeep>();
        beep.PlayBeep(5);

        yield return new WaitForSeconds(1f);

        Destroy(portableBeep);
    }
}
