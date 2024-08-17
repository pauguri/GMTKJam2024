using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : HexGridObject, IPointerClickHandler
{
    [NonSerialized] public Board board;
    public bool blocked { get; private set; }
    [NonSerialized] public bool occupied = false;

    private Image image;
    private Color normalColor;
    [SerializeField] private Color blockedColor = Color.red;
    [SerializeField] private GameObject telegraphImage;

    [NonSerialized] public int distanceToEdge = 0;
    //[NonSerialized] public int possibleRoutes;
    //public float Score => distanceToEdge > 0 ? possibleRoutes / distanceToEdge : -1;
    // public float Score => distanceToEdge > 0 ? 1f / distanceToEdge : -1;

    public override void Awake()
    {
        base.Awake();

        SetPosition(AnchoredToHex(transform.anchoredPosition.x, transform.parent.GetComponent<RectTransform>().anchoredPosition.y));

        image = GetComponent<Image>();
        normalColor = image.color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //float distanceFromCenter = Vector2.Distance(eventData.position, transform.position);
        //if (distanceFromCenter < transform.rect.width / 2)
        //{
        //    print("Clicked on cell " + gameObject.name);
        //}

        if (board.enableInput && !blocked && !occupied)
        {
            SetBlocked(true);

            GameManager.Instance.clickedCells.Add(Position);
            board.CalculatePathfinding();
        }
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
            yield return new WaitForSeconds(0.08f);
            telegraphImage.SetActive(false);
            yield return new WaitForSeconds(0.08f);
        }
        image.color = blockedColor;

        //board.enableInput = true;
    }
}
