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
        for (int i = 0; i < 2; i++)
        {
            telegraphImage.SetActive(true);
            yield return new WaitForSeconds(0.8f);
            telegraphImage.SetActive(false);
            yield return new WaitForSeconds(0.8f);
        }
    }
}
