using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Cell : HexGridObject
{
    // [NonSerialized] public Board board;

    [NonSerialized] public Image image;
    [NonSerialized] public Color normalColor;
    public Color blockedColor = Color.red;
    public GameObject telegraphImage;
    [Space]
    [SerializeField] private float telegraphDuration = 0.32f;
    [SerializeField] private bool playTelegraphBeep = true;
    [SerializeField] private int telegraphBeepPreset = 1;
    [SerializeField] private float telegraphBeepPitch = 1f;
    [SerializeField] private bool playBlockedBeep = true;
    [SerializeField] private int blockedBeepPreset = 3;
    [SerializeField] private float blockedBeepPitch = 1f;

    [NonSerialized] public bool blocked = false;
    [NonSerialized] public bool occupied = false;
    [NonSerialized] public int distanceToEdge = 0;

    public override void Awake()
    {
        base.Awake();

        RectTransform parentRow = transform.parent.GetComponent<RectTransform>();
        SetPosition(AnchoredToHex(transform.anchoredPosition.x + parentRow.anchoredPosition.x, parentRow.anchoredPosition.y));

        image = GetComponent<Image>();
        normalColor = image.color;
    }

    public void SetBlocked(bool state, bool animate = true)
    {
        blocked = state;
        image.raycastTarget = !state;

        if (blocked)
        {
            if (animate)
            {
                StartCoroutine(BlockedAnimation());
            }
            else
            {
                image.color = blockedColor;
            }
        }
        else
        {
            image.color = normalColor;
        }
    }

    public IEnumerator BlockedAnimation()
    {
        //board.enableInput = false;

        for (int i = 0; i < 2; i++)
        {
            telegraphImage.SetActive(true);
            if (playTelegraphBeep)
                SoundManager.Instance.PlayBeep(telegraphBeepPreset, true, telegraphBeepPitch);
            yield return new WaitForSeconds(telegraphDuration / 4);
            telegraphImage.SetActive(false);
            yield return new WaitForSeconds(telegraphDuration / 4);
        }
        image.color = blockedColor;
        if (playBlockedBeep)
            SoundManager.Instance.PlayBeep(blockedBeepPreset, true, blockedBeepPitch);

        //board.enableInput = true;
    }
}
