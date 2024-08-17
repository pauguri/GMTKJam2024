using UnityEngine;

public class Pillar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(1, Random.Range(0.8f, 1.1f), 1);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
