using System.Collections;
using UnityEngine;

public class GroundCell : HexGridObject
{
    [SerializeField] private GameObject telegraphImage;

    public override void Awake()
    {
        base.Awake();

        SetPosition(AnchoredToHex(transform.anchoredPosition.x, transform.parent.GetComponent<RectTransform>().anchoredPosition.y));
    }

    public IEnumerator BlockedAnimation()
    {
        GameObject portableBeep = Instantiate(SoundManager.Instance.portableBeepPrefab, transform.position, Quaternion.identity);
        PortableBeep beep = portableBeep.GetComponent<PortableBeep>();

        for (int i = 0; i < 2; i++)
        {
            telegraphImage.SetActive(true);
            beep.PlayBeep(1, true, 0.2f);
            yield return new WaitForSeconds(0.8f);
            telegraphImage.SetActive(false);
            yield return new WaitForSeconds(0.8f);
        }

        Destroy(portableBeep);
    }
}
