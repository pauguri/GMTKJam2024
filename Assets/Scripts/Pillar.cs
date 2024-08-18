using System.Collections;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    // Start is called before the first frame update
    public void Init(bool animate)
    {
        if (!animate)
        {
            transform.localScale = new Vector3(1, Random.Range(0.8f, 1.1f), 1);
            return;
        }

        StartCoroutine(SpawnAnimation());
    }

    private IEnumerator SpawnAnimation()
    {
        transform.position += Vector3.up * 700;

        float timeElapsed = 0f;
        while (transform.position.y > 0)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(700, 0, timeElapsed / 2.5f), transform.position.z);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        GameObject portableBeep = Instantiate(SoundManager.Instance.portableBeepPrefab, transform.position, Quaternion.identity);
        PortableBeep beep = portableBeep.GetComponent<PortableBeep>();
        beep.PlayBeep(5);

        yield return new WaitForSeconds(1f);

        Destroy(portableBeep);
    }
}
